using HarmonyLib;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

#if MONO
using ScheduleOne.UI.Stations;
using ScheduleOne.UI;
using ScheduleOne.Product;
using ScheduleOne.Quests;
using ScheduleOne.DevUtilities;
using ScheduleOne.Effects;
using ScheduleOne.ItemFramework;
using ScheduleOne.ObjectScripts;
using ScheduleOne.StationFramework;
using ScheduleOne.Networking;
using ScheduleOne.Money;
using ScheduleOne;
using Effect = ScheduleOne.Effects.Effect;
using EQuality = ScheduleOne.ItemFramework.EQuality;
using QualityItemInstance = ScheduleOne.ItemFramework.QualityItemInstance;
using StationRecipe = ScheduleOne.StationFramework.StationRecipe;
using EDrugType = ScheduleOne.Product.EDrugType;
#else
using Il2CppScheduleOne.UI.Stations;
using Il2CppScheduleOne.UI;
using Il2CppScheduleOne.Quests;
using Il2CppScheduleOne.Product;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Effects;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.ObjectScripts;
using Il2CppScheduleOne.StationFramework;
using Il2CppScheduleOne.Networking;
using Il2CppScheduleOne.Money;
using Il2CppScheduleOne;
using Effect = Il2CppScheduleOne.Effects.Effect;
using EQuality = Il2CppScheduleOne.ItemFramework.EQuality;
using QualityItemInstance = Il2CppScheduleOne.ItemFramework.QualityItemInstance;
using StationRecipe = Il2CppScheduleOne.StationFramework.StationRecipe;
using EDrugType = Il2CppScheduleOne.Product.EDrugType;
#endif

namespace DomsExpandedIngredientsAndEffects.Patches;

[HarmonyPatch]
public class StabilityFix
{
    private static readonly BindingFlags _private = BindingFlags.NonPublic | BindingFlags.Instance;
    private static bool _isProcessing   = false;
    private static bool _mixNamedFired  = false;
    private static bool _mixNamedRunning = false;

    // ── Safe LINQ replacements ────────────────────────────────
    private static bool EffectListAny(IEnumerable<Effect> list, Func<Effect, bool> pred)
    {
        if (list == null) return false;
        foreach (var e in list) if (pred(e)) return true;
        return false;
    }

    private static Effect EffectListFind(IEnumerable<Effect> list, Func<Effect, bool> pred)
    {
        if (list == null) return null;
        foreach (var e in list) if (pred(e)) return e;
        return null;
    }

    private static int EffectListCount(IEnumerable<Effect> list, Func<Effect, bool> pred)
    {
        int n = 0;
        if (list == null) return n;
        foreach (var e in list) if (pred(e)) n++;
        return n;
    }

    private static void EffectListRemoveAll(List<Effect> list, Func<Effect, bool> pred)
    {
        if (list == null) return;
        for (int i = list.Count - 1; i >= 0; i--)
            if (pred(list[i])) list.RemoveAt(i);
    }

    private static Effect OutputEffectsFirstOrDefault(string id)
    {
        foreach (var e in CustomMixRegistry.OutputEffects)
            if (e?.ID == id) return e;
        return null;
    }

    // ── mixRecipes field lookup with IL2CPP fallback ──────────
    private static string _mixRecipesFieldName = null;

    private static List<StationRecipe> GetMixRecipes(ProductManager pm)
    {
        // Try cached field name first
        if (_mixRecipesFieldName != null)
        {
            var cached = typeof(ProductManager).GetField(_mixRecipesFieldName,
                BindingFlags.NonPublic | BindingFlags.Instance);
            if (cached != null)
            {
                var res = cached.GetValue(pm) as List<StationRecipe>;
                if (res != null) return res;
            }
        }

        // Try by name
        var field = AccessTools.Field(typeof(ProductManager), "mixRecipes");
        if (field != null)
        {
            var res = field.GetValue(pm) as List<StationRecipe>;
            if (res != null)
            {
                _mixRecipesFieldName = "mixRecipes";
                return res;
            }
        }

        // IL2CPP fallback — search all fields for List<StationRecipe>
        foreach (var f in typeof(ProductManager).GetFields(
            BindingFlags.NonPublic | BindingFlags.Instance))
        {
            if (f.FieldType == typeof(List<StationRecipe>))
            {
                var res = f.GetValue(pm) as List<StationRecipe>;
                if (res != null)
                {
                    _mixRecipesFieldName = f.Name;
                    MelonLoader.MelonLogger.Msg($"[STABILITY] Found mixRecipes field: {f.Name}");
                    return res;
                }
            }
        }

        MelonLoader.MelonLogger.Warning("[STABILITY] Could not find mixRecipes field.");
        return null;
    }

    // ── Fix 1 ─────────────────────────────────────────────────
    [HarmonyPatch(typeof(MixingStationCanvas), "MixingDone")]
    [HarmonyPrefix]
    public static bool MixingDonePrefix()
    {
        if (_isProcessing) return false;
        _isProcessing = true;
        MelonLoader.MelonCoroutines.Start(ReleaseLock());
        return true;
    }

    [HarmonyPatch(typeof(MixingStationCanvas), "MixingDone")]
    [HarmonyFinalizer]
    public static Exception MixingDoneFinalizer(Exception __exception)
    {
        if (__exception != null)
            MelonLoader.MelonLogger.Error($"[STABILITY] MixingDone crash: {__exception.Message}\n{__exception.StackTrace}");
        return null;
    }

    private static System.Collections.IEnumerator ReleaseLock()
    {
        yield return new WaitForSeconds(1.5f);
        _isProcessing = false;
    }

    // ── Fix 2 ─────────────────────────────────────────────────
    [HarmonyPatch(typeof(MixingStationCanvas), "Open")]
    [HarmonyPostfix]
    public static void OpenPostfix()
    {
        _mixNamedFired   = false;
        _mixNamedRunning = false;
    }

    // ── Fix 3 ─────────────────────────────────────────────────
    [HarmonyPatch(typeof(MixingStationCanvas), "MixNamed")]
    [HarmonyPrefix]
    public static bool MixNamedPrefix(MixingStationCanvas __instance, string mixName)
    {
        if (_mixNamedRunning) return false;
        _mixNamedRunning = true;

        var newMixScreen = Singleton<NewMixScreen>.Instance;
        if (newMixScreen != null)
            typeof(NewMixScreen).GetField("onMixNamed", BindingFlags.Public | BindingFlags.Instance)
                ?.SetValue(newMixScreen, null);

        MelonLoader.MelonCoroutines.Start(MixNamedDelayed(__instance, mixName));
        return false;
    }

    private static System.Collections.IEnumerator MixNamedDelayed(MixingStationCanvas canvas, string mixName)
    {
        if (canvas.MixingStation == null || canvas.MixingStation.CurrentMixOperation == null)
        {
            _mixNamedRunning = false;
            yield break;
        }

        string productID    = canvas.MixingStation.CurrentMixOperation.ProductID;
        string ingredientID = canvas.MixingStation.CurrentMixOperation.IngredientID;
        int    quantity     = canvas.MixingStation.CurrentMixOperation.Quantity;
        EQuality quality    = canvas.MixingStation.CurrentMixOperation.ProductQuality;

        NetworkSingleton<ProductManager>.Instance.FinishAndNameMix(productID, ingredientID, mixName);

        yield return null;
        yield return null;

        if (canvas.MixingStation.CurrentMixOperation != null &&
            canvas.MixingStation.CurrentMixOperation.IsOutputKnown(out var knownProduct))
        {
            canvas.MixingStation.TryCreateOutputItems();
        }
        else
        {
            try
            {
                ProductDefinition      baseProduct = Registry.GetItem<ProductDefinition>(productID);
                PropertyItemDefinition ingredient  = Registry.GetItem<PropertyItemDefinition>(ingredientID);

                if (baseProduct == null || ingredient == null || ingredient.Properties.Count == 0)
                {
                    _mixNamedRunning = false;
                    yield break;
                }

                var outputProperties = EffectMixCalculator.MixProperties(
                    baseProduct.Properties, ingredient.Properties[0], baseProduct.DrugType);

                ProductDefinition outputProduct = UnityEngine.Object.Instantiate(baseProduct);
                outputProduct.name = mixName;
                outputProduct.Name = mixName;
                outputProduct.Initialize(outputProperties);
                outputProduct.GenerateAppearanceSettings();
                outputProduct.ID = ProductManager.MakeIDFileSafe(mixName);

                if (!Registry.ItemExists(outputProduct.ID))
                    Singleton<Registry>.Instance.AddToRegistry(outputProduct);

                var pm = NetworkSingleton<ProductManager>.Instance;
                if (pm != null)
                {
                    if (!pm.AllProducts.Contains(outputProduct))
                        pm.AllProducts.Add(outputProduct);
                    if (!ProductManager.DiscoveredProducts.Contains(outputProduct))
                        ProductManager.DiscoveredProducts.Add(outputProduct);
                    if (!pm.ProductNames.Contains(mixName))
                        pm.ProductNames.Add(mixName);
                    pm.SetPrice(null, outputProduct.ID, outputProduct.MarketValue);
                    pm.CreateMixRecipe(null, baseProduct.ID, ingredient.ID, outputProduct.ID);
                }

                QualityItemInstance outputInstance = outputProduct.GetDefaultInstance(quantity) as QualityItemInstance;
                outputInstance.SetQuality(quality);
                canvas.MixingStation.OutputSlot.AddItem(outputInstance);
                canvas.MixingStation.SetMixOperation(null, null, 0);
            }
            catch (Exception ex)
            {
                MelonLoader.MelonLogger.Error($"[STABILITY] Manual output failed: {ex.Message}");
                _mixNamedRunning = false;
                yield break;
            }
        }

        float elapsed = 0f;
        while (canvas.MixingStation.OutputSlot.Quantity == 0)
        {
            elapsed += Time.deltaTime;
            if (elapsed >= 3f) break;
            yield return null;
        }

        canvas.MixingStation.DiscoveryBox.gameObject.SetActive(false);
        canvas.Canvas.enabled = true;
        canvas.Container.gameObject.SetActive(true);

        _mixNamedFired = true;
        var type = typeof(MixingStationCanvas);
        type.GetMethod("UpdateDisplayMode", _private)?.Invoke(canvas, null);
        type.GetMethod("UpdateInstruction", _private)?.Invoke(canvas, null);
        type.GetMethod("UpdatePreview",     _private)?.Invoke(canvas, null);
        type.GetMethod("UpdateBeginButton", _private)?.Invoke(canvas, null);

        _mixNamedRunning = false;
    }

    // ── Fix 4 ─────────────────────────────────────────────────
    [HarmonyPatch(typeof(MixingStationCanvas), "UpdateDisplayMode")]
    [HarmonyPostfix]
    public static void UpdateDisplayModePostfix(MixingStationCanvas __instance)
    {
        if (!_mixNamedFired) return;
        try
        {
            if (!__instance.TitleContainer.gameObject.activeSelf &&
                !__instance.MainContainer.gameObject.activeSelf &&
                !__instance.OutputSlotUI.gameObject.activeSelf)
            {
                __instance.TitleContainer.gameObject.SetActive(true);
                __instance.MainContainer.gameObject.SetActive(true);
                _mixNamedFired = false;
            }
        }
        catch { }
    }

    // ── Fix 5 ─────────────────────────────────────────────────
    [HarmonyPatch]
    public class UIErrorHandler
    {
        static MethodBase TargetMethod()
        {
            var type = AccessTools.TypeByName("ScheduleOne.UI.NewMixScreen")
                    ?? AccessTools.TypeByName("Il2CppScheduleOne.UI.NewMixScreen");
            return type?.GetMethods().FirstOrDefault(m => m.Name == "Open" && m.GetParameters().Length >= 2);
        }

        [HarmonyFinalizer]
        public static Exception Finalizer(Exception __exception)
        {
            if (__exception != null)
                MelonLoader.MelonLogger.Error($"[STABILITY] UI Crash: {__exception.Message}");
            return null;
        }
    }

    // ── Fix 6 ─────────────────────────────────────────────────
    [HarmonyPatch(typeof(MixOperation), nameof(MixOperation.IsOutputKnown))]
    public class MixOperation_IsOutputKnown_Patch
    {
        [HarmonyPrefix]
        public static bool Prefix(MixOperation __instance, ref ProductDefinition knownProduct, ref bool __result)
        {
            try
            {
                if (string.IsNullOrEmpty(__instance.ProductID) || string.IsNullOrEmpty(__instance.IngredientID))
                    return true;

                var pm = NetworkSingleton<ProductManager>.Instance;
                if (pm == null) return true;

                var recipe = pm.GetRecipe(__instance.ProductID, __instance.IngredientID);
                if (recipe != null && recipe.Product?.Item is ProductDefinition pd)
                {
                    knownProduct = pd;
                    __result     = true;
                    return false;
                }
            }
            catch (Exception ex)
            {
                MelonLoader.MelonLogger.Error($"[STABILITY] IsOutputKnown error: {ex.Message}");
            }
            return true;
        }
    }

    // ── Fix 7 ─────────────────────────────────────────────────
    [HarmonyPatch(typeof(ProductManager), nameof(ProductManager.GetRecipe), new Type[] { typeof(string), typeof(string) })]
    public class FixGetRecipePatch
    {
        [HarmonyPrefix]
        public static bool Prefix(ProductManager __instance, string product, string mixer, ref StationRecipe __result)
        {
            try
            {
                var mixRecipes = GetMixRecipes(__instance);
                if (mixRecipes == null) return true;

                foreach (var r in mixRecipes)
                {
                    if (r?.Ingredients == null || r.Ingredients.Count < 2) continue;
                    string id0 = r.Ingredients[0].Items?[0]?.ID;
                    string id1 = r.Ingredients[1].Items?[0]?.ID;
                    if ((id0 == product && id1 == mixer) || (id1 == product && id0 == mixer))
                    {
                        __result = r;
                        return false;
                    }
                }
            }
            catch { }
            return true;
        }
    }

    // ── Fix 8 ─────────────────────────────────────────────────
    [HarmonyPatch(typeof(ProductManager), nameof(ProductManager.GetRecipe), new Type[] {
#if MONO
        typeof(List<Effect>),
#else
        typeof(Il2CppSystem.Collections.Generic.List<Effect>),
#endif
        typeof(Effect) })]
    public class FixGetRecipeOverloadPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(ProductManager __instance,
#if MONO
            List<Effect> productProperties,
#else
            Il2CppSystem.Collections.Generic.List<Effect> productProperties,
#endif
            Effect mixerProperty, ref StationRecipe __result)
        {
            try
            {
                if (productProperties == null || mixerProperty == null) return true;

                var mixRecipes = GetMixRecipes(__instance);
                if (mixRecipes == null) return true;

                foreach (var mixRecipe in mixRecipes)
                {
                    if (mixRecipe?.Ingredients == null || mixRecipe.Ingredients.Count < 2) continue;

                    var item0 = mixRecipe.Ingredients[0].Items?[0] as PropertyItemDefinition;
                    var item1 = mixRecipe.Ingredients[1].Items?[0] as PropertyItemDefinition;
                    if (item0 == null || item1 == null) continue;

                    var baseProps = (mixRecipe.Ingredients[1].Items[0] is ProductDefinition) ? item1.Properties : item0.Properties;
                    var mixProps  = (mixRecipe.Ingredients[1].Items[0] is ProductDefinition) ? item0.Properties : item1.Properties;

                    if (baseProps == null || mixProps == null ||
                        baseProps.Count != productProperties.Count || mixProps.Count != 1) continue;

                    bool match = true;
                    foreach (var prop in productProperties)
                    {
                        if (prop == null) { match = false; break; }
                        bool found = false;
                        foreach (var bp in baseProps)
                            if (bp != null && bp.ID == prop.ID) { found = true; break; }
                        if (!found) { match = false; break; }
                    }

                    if (match && mixProps[0] != null && mixProps[0].ID == mixerProperty.ID)
                    {
                        __result = mixRecipe;
                        return false;
                    }
                }
            }
            catch { }
            return true;
        }
    }

    // ── Fix 9 ─────────────────────────────────────────────────
    [HarmonyPatch(typeof(ProductManager), nameof(ProductManager.GetKnownProduct))]
    public class FixGetKnownProductPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(ProductManager __instance, EDrugType type,
#if MONO
            List<Effect> properties,
#else
            Il2CppSystem.Collections.Generic.List<Effect> properties,
#endif
            ref ProductDefinition __result)
        {
            try
            {
                if (properties == null) return true;

                for (int i = __instance.AllProducts.Count - 1; i >= 0; i--)
                {
                    var p = __instance.AllProducts[i];
                    if (p?.DrugTypes == null || p.DrugTypes.Count == 0 || p.DrugTypes[0].DrugType != type) continue;
                    if (p.Properties == null || p.Properties.Count != properties.Count) continue;

                    bool allMatch = true;
                    foreach (var prop in properties)
                    {
                        if (prop == null) { allMatch = false; break; }
                        bool found = false;
                        foreach (var pp in p.Properties)
                            if (pp != null && pp.ID == prop.ID) { found = true; break; }
                        if (!found) { allMatch = false; break; }
                    }

                    if (allMatch)
                    {
                        __result = p;
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                MelonLoader.MelonLogger.Error($"[STABILITY] FixGetKnownProduct error: {ex.Message}");
            }
            return true;
        }
    }

    // ── Fix 10 ────────────────────────────────────────────────
    [HarmonyPatch(typeof(EffectMixCalculator), nameof(EffectMixCalculator.MixProperties))]
    public class DoubleMixUpgradePatch
    {
        [HarmonyPrefix]
        public static bool Prefix(
#if MONO
            List<Effect> __0,
            Effect __1, EDrugType __2, ref List<Effect> __result)
#else
            Il2CppSystem.Collections.Generic.List<Effect> __0,
            Effect __1, EDrugType __2, ref Il2CppSystem.Collections.Generic.List<Effect> __result)
#endif
        {
            try
            {
                var r = new List<Effect>();
                if (__0 != null)
                    foreach (var e in __0) if (e != null) r.Add(e);

                if (__1 == null)
                {
                    WriteBack(r, ref __result);
                    return false;
                }

                string heliumID      = "heliuminfusion";
                string moonGravityID = "moongravity";

                bool comboFired = false;
                foreach (var combo in CustomMixRegistry.Combinations)
                {
                    // Forward: product already has ExistingEffectID, mixing in IngredientEffectID
                    Effect existingMatch = EffectListFind(r, e => e?.ID == combo.ExistingEffectID);
                    if (existingMatch != null && __1.ID == combo.IngredientEffectID)
                    {
                        int index = r.IndexOf(existingMatch);
                        r[index] = combo.BuiltOutputEffect;
                        EffectListRemoveAll(r, e => e?.ID == combo.IngredientEffectID && e != combo.BuiltOutputEffect);
                        comboFired = true;
                        break;
                    }
                    // Reverse: product already has IngredientEffectID, mixing in ExistingEffectID
                    if (combo.ExistingEffectID != combo.IngredientEffectID)
                    {
                        Effect reverseMatch = EffectListFind(r, e => e?.ID == combo.IngredientEffectID);
                        if (reverseMatch != null && __1.ID == combo.ExistingEffectID)
                        {
                            int index = r.IndexOf(reverseMatch);
                            r[index] = combo.BuiltOutputEffect;
                            EffectListRemoveAll(r, e => e?.ID == combo.ExistingEffectID && e != combo.BuiltOutputEffect);
                            comboFired = true;
                            break;
                        }
                    }
                }

                if (!comboFired)
                {
                    foreach (var combo in CustomMixRegistry.MultiCombinations)
                    {
                        bool allPresent = true;
                        foreach (var id in combo.RequiredEffectIDs)
                            if (!EffectListAny(r, e => e?.ID == id)) { allPresent = false; break; }

                        bool ingredientIsBeingAdded  = __1.ID == combo.IngredientEffectID;
                        bool ingredientAlreadyOnDrug = EffectListAny(r, e => e?.ID == combo.IngredientEffectID);

                        if (allPresent && (ingredientIsBeingAdded || ingredientAlreadyOnDrug))
                        {
                            foreach (var id in combo.RequiredEffectIDs)
                                EffectListRemoveAll(r, e => e?.ID == id);
                            EffectListRemoveAll(r, e => e?.ID == combo.IngredientEffectID);
                            r.Add(combo.BuiltOutputEffect);
                            comboFired = true;
                            break;
                        }
                    }
                }

                if (comboFired)
                {
                    RunChainCheck(r);
                    WriteBack(r, ref __result);
                    return false;
                }

                if (__1.ID == moonGravityID || __1.ID == heliumID)
                {
                    if (EffectListAny(r, e => e != null && e.ID == heliumID))
                    {
                        var pm = NetworkSingleton<ProductManager>.Instance;
                        Effect moonGravInstance = null;

                        if (pm?.WeedMixMap?.Effects != null)
                            foreach (var entry in pm.WeedMixMap.Effects)
                                if (entry?.Property?.ID == moonGravityID)
                                { moonGravInstance = entry.Property; break; }

                        if (moonGravInstance == null && Singleton<PropertyUtility>.Instance?.AllProperties != null)
                            foreach (var prop in Singleton<PropertyUtility>.Instance.AllProperties)
                                if (prop?.ID == moonGravityID)
                                { moonGravInstance = prop; break; }

                        if (moonGravInstance != null)
                        {
                            EffectListRemoveAll(r, e => e != null && (e.ID == heliumID || e.ID == __1.ID));
                            r.Add(moonGravInstance);

                            int gravCount = EffectListCount(r, e => e != null && e.ID == moonGravityID);
                            if (gravCount >= 2)
                            {
                                foreach (var combo in CustomMixRegistry.Combinations)
                                {
                                    if (combo.ExistingEffectID == moonGravityID && combo.IngredientEffectID == moonGravityID)
                                    {
                                        EffectListRemoveAll(r, e => e != null && e.ID == moonGravityID);
                                        r.Add(combo.BuiltOutputEffect);
                                        break;
                                    }
                                }
                            }

                            RunChainCheck(r);
                            WriteBack(r, ref __result);
                            return false;
                        }
                    }
                }

                // No custom combo fired — let the original game method handle
                // effect addition and its own native combination logic
                return true;
            }
            catch (Exception ex)
            {
                MelonLoader.MelonLogger.Error($"[STABILITY] DoubleMixUpgrade error: {ex.Message}");
                return true;
            }
        }

        // After the original method runs (or after our Prefix sets the result),
        // apply RunChainCheck so any custom combos triggered by the final state are resolved
        [HarmonyPostfix]
        public static void Postfix(
#if MONO
            List<Effect> __0, Effect __1, EDrugType __2, ref List<Effect> __result)
#else
            Il2CppSystem.Collections.Generic.List<Effect> __0,
            Effect __1, EDrugType __2, ref Il2CppSystem.Collections.Generic.List<Effect> __result)
#endif
        {
            try
            {
                if (__result == null) return;
                var r = new List<Effect>();
                foreach (var e in __result) if (e != null) r.Add(e);
                RunChainCheck(r);
                WriteBack(r, ref __result);
            }
            catch (Exception ex)
            {
                MelonLoader.MelonLogger.Error($"[STABILITY] MixProperties Postfix error: {ex.Message}");
            }
        }

        private static void WriteBack(List<Effect> r,
#if MONO
            ref List<Effect> __result)
        {
            __result = r;
        }
#else
            ref Il2CppSystem.Collections.Generic.List<Effect> __result)
        {
            __result = new Il2CppSystem.Collections.Generic.List<Effect>();
            foreach (var e in r) __result.Add(e);
        }
#endif

        private static void RunChainCheck(List<Effect> result)
        {
            bool keepChecking = true;
            while (keepChecking)
            {
                keepChecking = false;

                foreach (var combo in CustomMixRegistry.Combinations)
                {
                    Effect existingMatch   = EffectListFind(result, e => e?.ID == combo.ExistingEffectID);
                    Effect ingredientMatch = EffectListFind(result, e => e?.ID == combo.IngredientEffectID);
                    if (existingMatch != null && ingredientMatch != null && existingMatch != ingredientMatch)
                    {
                        int index = result.IndexOf(existingMatch);
                        result[index] = combo.BuiltOutputEffect;
                        result.Remove(ingredientMatch);
                        keepChecking = true;
                        break;
                    }
                }

                if (keepChecking) continue;

                foreach (var combo in CustomMixRegistry.MultiCombinations)
                {
                    bool allPresent = true;
                    foreach (var id in combo.RequiredEffectIDs)
                        if (!EffectListAny(result, e => e?.ID == id)) { allPresent = false; break; }

                    Effect ingMatch = EffectListFind(result, e => e?.ID == combo.IngredientEffectID);
                    if (allPresent && ingMatch != null)
                    {
                        foreach (var id in combo.RequiredEffectIDs)
                            EffectListRemoveAll(result, e => e?.ID == id);
                        result.Remove(ingMatch);
                        result.Add(combo.BuiltOutputEffect);
                        keepChecking = true;
                        break;
                    }
                }
            }
        }
    }

    // ── Fix 11 ────────────────────────────────────────────────
    [HarmonyPatch(typeof(PropertyUtility), nameof(PropertyUtility.GetProperties), new Type[] {
#if MONO
        typeof(List<string>)
#else
        typeof(Il2CppSystem.Collections.Generic.List<string>)
#endif
    })]
    public class FixPropertyUtilityGetProperties
    {
        [HarmonyPostfix]
        public static void Postfix(
#if MONO
            List<string> __0,
            ref List<Effect> __result)
        {
            if (__0 == null || __result == null) return;
            var result = __result;
            if (result.Count == __0.Count) return;

            var pm = NetworkSingleton<ProductManager>.Instance;
            if (pm == null) return;

            foreach (string id in __0)
            {
                if (EffectListAny(result, e => e != null && e.ID == id)) continue;
                Effect missing = FindEffectInMixMaps(pm, id)
                    ?? FindEffectInIngredients(pm, id)
                    ?? OutputEffectsFirstOrDefault(id);
                if (missing != null) result.Add(missing);
                else MelonLoader.MelonLogger.Warning($"[STABILITY] Could not recover effect ID: {id}");
            }
            __result = result;
        }
#else
            Il2CppSystem.Collections.Generic.List<string> __0,
            ref Il2CppSystem.Collections.Generic.List<Effect> __result)
        {
            if (__0 == null || __result == null) return;

            var result = new List<Effect>();
            foreach (var e in __result) if (e != null) result.Add(e);
            if (result.Count == __0.Count) return;

            var pm = NetworkSingleton<ProductManager>.Instance;
            if (pm == null) return;

            foreach (string id in __0)
            {
                if (EffectListAny(result, e => e != null && e.ID == id)) continue;
                Effect missing = FindEffectInMixMaps(pm, id)
                    ?? FindEffectInIngredients(pm, id)
                    ?? OutputEffectsFirstOrDefault(id);
                if (missing != null) result.Add(missing);
                else MelonLoader.MelonLogger.Warning($"[STABILITY] Could not recover effect ID: {id}");
            }

            __result = new Il2CppSystem.Collections.Generic.List<Effect>();
            foreach (var e in result) __result.Add(e);
        }
#endif

        private static Effect FindEffectInMixMaps(ProductManager pm, string id)
        {
            var maps = new[] { pm.WeedMixMap, pm.MethMixMap, pm.CokeMixMap, pm.ShroomMixMap };
            foreach (var map in maps)
            {
                if (map?.Effects == null) continue;
                foreach (var entry in map.Effects)
                    if (entry?.Property?.ID == id) return entry.Property;
            }
            return null;
        }

        private static Effect FindEffectInIngredients(ProductManager pm, string id)
        {
            if (pm.ValidMixIngredients == null) return null;
            foreach (var ingredient in pm.ValidMixIngredients)
            {
                if (ingredient?.Properties == null) continue;
                foreach (var prop in ingredient.Properties)
                    if (prop?.ID == id) return prop;
            }
            return null;
        }
    }

    // ── Profit boost ──────────────────────────────────────────
    [HarmonyPatch(typeof(Contract), nameof(Contract.SubmitPayment))]
    public class ProfitBoostPatch
    {
        [HarmonyPrefix]
        public static void Prefix(Contract __instance, ref float bonusTotal)
        {
            if (!DomsExpandedIngredientsAndEffects.Effects.ProfitBoostEffect.IsActive) return;
            float bonus = __instance.Payment * 0.15f;
            bonusTotal += bonus;
        }
    }

    [HarmonyPatch(typeof(DealCompletionPopup), nameof(DealCompletionPopup.PlayPopup))]
    public class ProfitBoostPopupPatch
    {
        [HarmonyPrefix]
        public static void Prefix(float basePayment, ref List<Contract.BonusPayment> bonuses)
        {
            if (!DomsExpandedIngredientsAndEffects.Effects.ProfitBoostEffect.IsActive) return;
            float bonus = basePayment * 0.15f;
            bonuses = new List<Contract.BonusPayment>(bonuses);
            bonuses.Add(new Contract.BonusPayment("Magic Juice +15%", bonus));
        }
    }
}