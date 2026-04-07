using System;
using System.Collections.Generic;
using MelonLoader;
using UnityEngine;
using HarmonyLib;
using DomsExpandedIngredientsAndEffects.Framework;
using DomsExpandedIngredientsAndEffects.Ingredients;
using DomsExpandedIngredientsAndEffects.Effects.Descriptors;
using DomsExpandedIngredientsAndEffects.Patches;
using DomsExpandedIngredientsAndEffects.Utilities;
using DomsExpandedIngredientsAndEffects.Effects;

#if MONO
using ScheduleOne;
using ScheduleOne.PlayerScripts;
using ScheduleOne.ItemFramework;
using ScheduleOne.Product;
using ScheduleOne.DevUtilities;
using ScheduleOne.Effects;
using ScheduleOne.Effects.MixMaps;
using ScheduleOne.Levelling;
using ScheduleOne.Interaction;
using ScheduleOne.Messaging;
using ScheduleOne.NPCs;
using ScheduleOne.Map;
using ScheduleOne.UI;
using ScheduleOne.UI.Shop;
using ScheduleOne.UI.Phone.Delivery;
using ItemInstance = ScheduleOne.ItemFramework.ItemInstance;
#else
using Il2CppScheduleOne;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppInterop.Runtime.Injection;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.Product;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.UI;
using Il2CppScheduleOne.UI.Shop;
using Il2CppScheduleOne.UI.Phone.Delivery;
using Il2CppScheduleOne.Messaging;
using Il2CppScheduleOne.NPCs;
using Il2CppScheduleOne.Map;
using Il2CppScheduleOne.Levelling;
using Il2CppScheduleOne.Interaction;
using Il2CppScheduleOne.Effects;
using Il2CppScheduleOne.Effects.MixMaps;
using ItemInstance = Il2CppScheduleOne.ItemFramework.ItemInstance;
#endif

[assembly: MelonInfo(typeof(DomsExpandedIngredientsAndEffects.DomsCustomEffects), "Dom's Custom Effects", "1.0.0", "Dom")]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace DomsExpandedIngredientsAndEffects
{
    public class DomsCustomEffects : MelonMod
    {
        private static bool _injected = false;

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (_injected) return;
            if (!sceneName.ToLower().Contains("main") && !sceneName.ToLower().Contains("game")) return;
            OnInject();
        }

        public override void OnInitializeMelon()
        {
        #if !MONO
            ClassInjector.RegisterTypeInIl2Cpp<MoonDustMixerEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<MoonGravityEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<VoidEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<BunnyEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<AlienEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<MartianEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<SuperMartianEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<NebulaEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<PhantomEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<WraithEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<TallEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<InversionEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<VoluptuousEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<GoldRushEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<SirenEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<DivaEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<SingleEventUpsetEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<ChargedEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<GildedMartianEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<AirhornBaseEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<AirhornMapEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<TrollEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<FiestaEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<CreepyEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<MagicBaseEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<MagicJuiceMapEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<SuperMagicEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<MakeItRainEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<SuperChargeEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<SuperSuperChargeEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<ProfitBoostEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<LipstickMapEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<EarthyEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<FertilizerMapEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<RootBoundEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<OvergrownEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<BloomEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<CompostEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<SpicyEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<HotSauceMapEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<FiveAlarmEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<CapsaicinRushEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<DragonBreathEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<InfernoEffect>();
            // Lipstick chain
            ClassInjector.RegisterTypeInIl2Cpp<FemmeFataleEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<VenomKissEffect>();
            // Airhorn chain
            ClassInjector.RegisterTypeInIl2Cpp<ThunderclapEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<WhiteNoiseEffect>();
            // Cross-ingredient
            ClassInjector.RegisterTypeInIl2Cpp<WildflowerEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<BurningPassionEffect>();
            ClassInjector.RegisterTypeInIl2Cpp<UpheavalEffect>();
            // Magic juice special
            ClassInjector.RegisterTypeInIl2Cpp<PhantasmEffect>();
        #endif
        }

        private void OnInject()
        {
            try
            {
                var registry       = Singleton<Registry>.Instance;
                var productManager = NetworkSingleton<ProductManager>.Instance;

                if (registry == null || productManager == null)
                {
                    MelonLogger.Error("Mod: Registry or ProductManager not found!");
                    return;
                }

                var moonDust   = new MoonDustIngredient();   moonDust.Register(registry, productManager);
                var lipstick   = new LipstickIngredient();   lipstick.Register(registry, productManager);
                var airhorn    = new AirhornIngredient();    airhorn.Register(registry, productManager);
                var magicJuice = new MagicJuiceIngredient(); magicJuice.Register(registry, productManager);
                var fertilizer = new FertilizerIngredient(); fertilizer.Register(registry, productManager);
                var hotSauce   = new HotSauceIngredient();   hotSauce.Register(registry, productManager);

                foreach (var combo in new EffectCombination[]
                {
                    new EffectCombination("moongravity",      "foggy",          new VoidEffectDef()),
                    new EffectCombination("heliuminfusion",   "energizing",     new BunnyEffectDef()),
                    new EffectCombination("moongravity",      "toxic",          new AlienEffectDef()),
                    new EffectCombination("moongravity",      "moongravity",    new MartianEffectDef()),
                    new EffectCombination("martianeffect",    "alieneffect",    new SuperMartianEffectDef()),
                    new EffectCombination("heliuminfusion",   "sedating",       new NebulaEffectDef()),
                    new EffectCombination("heliuminfusion",   "sneaky",         new PhantomEffectDef()),
                    new EffectCombination("phantomeffect",    "heliuminfusion", new WraithEffectDef()),
                    new EffectCombination("voluptuouseffect", "energizing",     new GoldRushEffectDef()),
                    new EffectCombination("voluptuouseffect", "voideffect",     new SingleEventUpsetEffectDef()),
                    new EffectCombination("heliuminfusion",   "giraffying",     new TallEffectDef()),
                    new EffectCombination("talleffect",       "foggy",          new InversionEffectDef()),
                    new EffectCombination("voluptuouseffect", "calming",        new SirenEffectDef()),
                    new EffectCombination("voluptuouseffect", "balding",        new DivaEffectDef()),
                    new EffectCombination("goldrusheffect",   "martianeffect",  new GildedMartianEffectDef()),
                    new EffectCombination("voluptuouseffect", "electrifying",   new ChargedEffectDef()),
                    new EffectCombination("airhornbase",      "sneaky",         new TrollEffectDef()),
                    new EffectCombination("airhornbase",      "energizing",     new FiestaEffectDef()),
                    new EffectCombination("trolleffect",      "airhornbase",    new CreepyEffectDef()),
                    new EffectCombination("magicbase",        "magicbase",      new SuperMagicEffectDef()),
                    new EffectCombination("magicbase",        "calming",        new MakeItRainEffectDef()),
                    new EffectCombination("magicbase",        "energizing",     new SuperChargeEffectDef()),
                    new EffectCombination("supermagic",       "slippery",       new ProfitBoostEffectDef()),
                    new EffectCombination("supercharge",      "supermagic",     new SuperSuperChargeEffectDef()),

                    // ── Fertilizer chain ──────────────────────────────────
                    new EffectCombination("earthyeffect",  "calming",      new RootBoundEffectDef()),
                    new EffectCombination("rootbound",     "toxic",        new OvergrownEffectDef()),
                    new EffectCombination("overgrown",     "energizing",   new BloomEffectDef()),
                    new EffectCombination("bloom",         "foggy",        new CompostEffectDef()),

                    // ── Hot Sauce chain ───────────────────────────────────
                    new EffectCombination("spicyeffect",   "toxic",        new FiveAlarmEffectDef()),
                    new EffectCombination("fivealarm",     "energizing",   new CapsaicinRushEffectDef()),
                    new EffectCombination("capsaicinrush", "sneaky",       new DragonBreathEffectDef()),
                    new EffectCombination("dragonbreath",  "sedating",     new InfernoEffectDef()),

                    // ── Lipstick chain ────────────────────────────────────
                    new EffectCombination("voluptuouseffect", "foggy",     new FemmeFataleEffectDef()),
                    new EffectCombination("divaeffect",       "toxic",     new VenomKissEffectDef()),

                    // ── Airhorn chain ─────────────────────────────────────
                    new EffectCombination("airhornbase",   "toxic",        new ThunderclapEffectDef()),
                    new EffectCombination("fiestaeffect",  "sedating",     new WhiteNoiseEffectDef()),

                    // ── Cross-ingredient ──────────────────────────────────
                    new EffectCombination("earthyeffect",  "voluptuouseffect", new WildflowerEffectDef()),
                    new EffectCombination("voluptuouseffect", "spicyeffect",     new BurningPassionEffectDef()),
                    new EffectCombination("earthyeffect",  "airhornbase",      new UpheavalEffectDef()),

                    // ── Magic Juice special ───────────────────────────────
                    new EffectCombination("magicbase",     "toxic",        new PhantasmEffectDef()),
                })
                {
                    CustomMixRegistry.Register(combo);
                }

                CustomMixRegistry.Register(new MultiEffectCombination(
                    new string[] { "heliuminfusion", "foggy" },
                    "longfaced",
                    new InversionEffectDef()
                ));

                MelonLoader.MelonCoroutines.Start(AddIngredientsToShops());
                MelonLoader.MelonCoroutines.Start(SpawnMagicJuiceVendor());

                _injected = true;
                MelonLogger.Msg("Loaded successfully!");
            }
            catch (Exception ex)
            {
                MelonLoader.MelonLogger.Error($"Injection Error: {ex.Message}\n{ex.StackTrace}");
            }
        }

        public static Sprite LoadCustomIcon(string fileName)
        {
            try
            {
                string path = System.IO.Path.Combine(
                    System.IO.Directory.GetCurrentDirectory(),
                    "Mods", "DomsCustomEffects", "Icons", fileName);

                if (!System.IO.File.Exists(path))
                {
                    MelonLoader.MelonLogger.Warning($"[Icon] Not found: {path}");
                    return null;
                }

                byte[] data = System.IO.File.ReadAllBytes(path);
                Texture2D tex = new Texture2D(2, 2);
#if MONO
                tex.LoadImage(data);
#else
                UnityEngine.ImageConversion.LoadImage(tex, data);
#endif
                return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            }
            catch (Exception ex)
            {
                MelonLoader.MelonLogger.Error($"[Icon] Failed: {ex.Message}");
                return null;
            }
        }

        public static Texture2D LoadCustomTexture(string fileName)
        {
            try
            {
                string path = System.IO.Path.Combine(
                    System.IO.Directory.GetCurrentDirectory(),
                    "Mods", "DomsCustomEffects", "Icons", fileName);

                if (!System.IO.File.Exists(path)) return null;

                byte[] data = System.IO.File.ReadAllBytes(path);
                Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        #if MONO
                tex.LoadImage(data);
        #else
                UnityEngine.ImageConversion.LoadImage(tex, data);
        #endif
                return tex;
            }
            catch (Exception ex)
            {
                MelonLoader.MelonLogger.Error($"[Texture] Failed: {ex.Message}");
                return null;
            }
        }

        public static AudioClip LoadCustomSound(string fileName)
        {
            try
            {
                string path = System.IO.Path.Combine(
                    System.IO.Directory.GetCurrentDirectory(),
                    "Mods", "DomsCustomEffects", "Sounds", fileName);

                if (!System.IO.File.Exists(path))
                {
                    MelonLogger.Warning($"[Sound] Not found: {path}");
                    return null;
                }

                byte[] data = System.IO.File.ReadAllBytes(path);
                AudioClip clip = WavUtility.ToAudioClip(data, fileName);
                if (clip == null)
                    MelonLogger.Warning($"[Sound] Failed to decode {fileName} — try converting to .wav");
                return clip;
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[Sound] Failed: {ex.Message}");
                return null;
            }
        }

        private static System.Collections.IEnumerator AddIngredientsToShops()
        {
            yield return new WaitForSeconds(2f);

            var shops = ShopInterface.AllShops;

            SetRankRequirement(FertilizerIngredient.Definition, ERank.Hoodlum,  1);
            SetRankRequirement(HotSauceIngredient.Definition,  ERank.Peddler,  3);
            SetRankRequirement(MoonDustIngredient.Definition,  ERank.Hustler,  5);
            SetRankRequirement(LipstickIngredient.Definition,  ERank.Bagman,   3);
            SetRankRequirement(AirhornIngredient.Definition,   ERank.Enforcer, 1);

            var gasCodes = new[] { "gas_mart_central", "gas_mart_west" };

            foreach (var code in gasCodes)
            {
                ShopInterface shop = null;
                foreach (var s in shops) { if (s.ShopCode == code) { shop = s; break; } }
                if (shop == null)
                {
                    MelonLogger.Warning($"[Shop] Could not find shop: {code}");
                    continue;
                }

                AddIngredientToShop(shop, FertilizerIngredient.Definition, 3f,  true);
                AddIngredientToShop(shop, HotSauceIngredient.Definition,  5f,  true);
                AddIngredientToShop(shop, MoonDustIngredient.Definition,  6f,  true);
                AddIngredientToShop(shop, LipstickIngredient.Definition,  5f,  true);
                AddIngredientToShop(shop, AirhornIngredient.Definition,   5f,  true);
            }

            yield return null;
            InjectDeliveryShopEntries();
        }

        private static void InjectDeliveryShopEntries()
        {
            var deliveryApp = PlayerSingleton<DeliveryApp>.Instance;
            if (deliveryApp == null)
            {
                MelonLogger.Warning("[Shop] DeliveryApp not found.");
                return;
            }

            var deliveryShops = deliveryApp.GetComponentsInChildren<DeliveryShop>(true);
            foreach (var ds in deliveryShops)
            {
                if (ds.MatchingShop == null) continue;
                if (ds.MatchingShop.ShopCode != "gas_mart_central" &&
                    ds.MatchingShop.ShopCode != "gas_mart_west") continue;

                var prefabField    = typeof(DeliveryShop).GetField("ListingEntryPrefab",
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                var containerField = typeof(DeliveryShop).GetField("ListingContainer",
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                var entriesField   = typeof(DeliveryShop).GetField("listingEntries",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                var prefab    = prefabField?.GetValue(ds)    as ListingEntry;
                var container = containerField?.GetValue(ds) as RectTransform;
                var entries   = entriesField?.GetValue(ds)   as List<ListingEntry>;

                if (prefab == null || container == null || entries == null)
                {
                    MelonLogger.Warning($"[Shop] Could not get DeliveryShop fields for {ds.MatchingShop.ShopName}.");
                    continue;
                }

                foreach (var listing in ds.MatchingShop.Listings)
                {
                    if (!listing.CanBeDelivered) continue;
                    if (entries.Exists(e => e.MatchingListing?.Item?.ID == listing.Item?.ID)) continue;

                    var entry = UnityEngine.Object.Instantiate(prefab, container);
                    entry.Initialize(listing);
                    entry.onQuantityChanged.AddListener(new UnityEngine.Events.UnityAction(() => { }));
                    entries.Add(entry);
                }

                var contentsContainerField = typeof(DeliveryShop).GetField("ContentsContainer",
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                var contentsContainer = contentsContainerField?.GetValue(ds) as RectTransform;
                if (contentsContainer != null)
                {
                    int rows = Mathf.CeilToInt((float)entries.Count / 2f);
                    contentsContainer.sizeDelta = new Vector2(
                        contentsContainer.sizeDelta.x, 230f + rows * 60f);
                }
            }
        }

        private static void SetRankRequirement(PropertyItemDefinition def, ERank rank, int tier)
        {
            if (def == null) return;
            def.RequiresLevelToPurchase = true;
            def.RequiredRank = new FullRank(rank, tier);

            var lm = NetworkSingleton<LevelManager>.Instance;
            if (lm != null)
                lm.AddUnlockable(new Unlockable(new FullRank(rank, tier), def.Name, def.Icon));
        }

        private static void AddIngredientToShop(ShopInterface shop, PropertyItemDefinition def, float price, bool canBeDelivered = false)
        {
            if (def == null || shop == null) return;
            foreach (var l in shop.Listings) { if (l.Item?.ID == def.ID) return; }

            var listing = new ShopListing
            {
                name           = def.Name,
                Item           = def,
                LimitedStock   = false,
                DefaultStock   = -1,
                CanBeDelivered = canBeDelivered,
            };

            var overridePriceField   = typeof(ShopListing).GetField("OverridePrice",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var overriddenPriceField = typeof(ShopListing).GetField("OverriddenPrice",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            overridePriceField?.SetValue(listing, true);
            overriddenPriceField?.SetValue(listing, price);

            shop.Listings.Add(listing);
            listing.Initialize(shop);

            typeof(ShopInterface)
                .GetMethod("CreateListingUI",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.Invoke(shop, new object[] { listing });
        }

        public static ShopInterface _magicJuiceShop;
        private static Canvas _magicJuiceCanvas;
        private static RectTransform _magicJuiceContainer;
        private static System.Collections.IEnumerator SpawnMagicJuiceVendor()
        {
            yield return new WaitForSeconds(3f);

            ShopInterface sourceShop = null;
            foreach (var s in ShopInterface.AllShops) { if (s.ShopCode == "gas_mart_central") { sourceShop = s; break; } }
            if (sourceShop == null)
            {
                MelonLogger.Warning("[Vendor] Could not find source shop to clone.");
                yield break;
            }

            var shopGO = UnityEngine.Object.Instantiate(sourceShop.gameObject);
            shopGO.name = "MagicJuiceVendorShop";
            _magicJuiceShop = shopGO.GetComponent<ShopInterface>();
            _magicJuiceShop.ShopName = "The Alchemist Table";
            _magicJuiceShop.ShopCode = "magic_juice_vendor";
            _magicJuiceCanvas = shopGO.GetComponentInChildren<Canvas>(true);
            _magicJuiceContainer = _magicJuiceShop.Container;

            if (_magicJuiceShop.StoreNameLabel != null)
                _magicJuiceShop.StoreNameLabel.text = "The Alchemist Table";

            var shopScreenField = typeof(ShopInterface).GetField("shopScreen",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var ownScreen = shopGO.GetComponentInChildren<UIScreen>(true);
            if (shopScreenField != null && ownScreen != null)
            {
                shopScreenField.SetValue(_magicJuiceShop, ownScreen);
            }
            else
            {
                MelonLogger.Warning($"[Vendor] shopScreen fix failed — field:{shopScreenField != null} screen:{ownScreen != null}");
            }

            var cart = shopGO.GetComponentInChildren<Cart>(true);
            if (cart != null)
            {
                cart.Shop = _magicJuiceShop;
            }
            else
            {
                MelonLogger.Warning("[Vendor] Could not find Cart component on cloned shop.");
            }

            var listingContainer = _magicJuiceShop.ListingContainer;
            if (listingContainer != null)
                for (int i = listingContainer.childCount - 1; i >= 0; i--)
                    UnityEngine.Object.Destroy(listingContainer.GetChild(i).gameObject);

            var listingUIField = typeof(ShopInterface).GetField("listingUI",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var listingUIList = listingUIField?.GetValue(_magicJuiceShop)
                as System.Collections.Generic.List<ListingUI>;
            listingUIList?.Clear();

            _magicJuiceShop.Listings.Clear();

            yield return null;

            SetRankRequirement(MagicJuiceIngredient.Definition, ERank.Enforcer, 4);
            AddIngredientToShop(_magicJuiceShop, MagicJuiceIngredient.Definition, 200f, false);

            SpawnVendorStand();
        }

        private static void SpawnVendorStand()
        {
            var vendorPos = new Vector3(-15.70f, -5.04f, 173.82f);

            var vendorGO = new GameObject("MagicJuiceVendorStand");
            vendorGO.transform.position = vendorPos;
            vendorGO.transform.rotation = Quaternion.Euler(0f, 30f, 0f);

            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");

            var table = GameObject.CreatePrimitive(PrimitiveType.Cube);
            table.name = "VendorTable";
            table.transform.SetParent(vendorGO.transform);
            table.transform.localPosition = new Vector3(0f, 0.75f, 0f);
            table.transform.localRotation = Quaternion.identity;
            table.transform.localScale = new Vector3(1.2f, 0.1f, 0.7f);
            var tableMat = new Material(shader);
            tableMat.color = new Color(0.15f, 0.05f, 0.25f);
            table.GetComponent<Renderer>().material = tableMat;
            table.layer = LayerMask.NameToLayer("Default");
            table.GetComponent<Collider>().isTrigger = false;

            var legMat = new Material(shader);
            legMat.color = new Color(0.1f, 0.03f, 0.18f);

            Vector3[] legPositions =
            {
                new Vector3( 0.55f, 0.35f,  0.3f),
                new Vector3(-0.55f, 0.35f,  0.3f),
                new Vector3( 0.55f, 0.35f, -0.3f),
                new Vector3(-0.55f, 0.35f, -0.3f),
            };

            foreach (var pos in legPositions)
            {
                var leg = GameObject.CreatePrimitive(PrimitiveType.Cube);
                leg.transform.SetParent(vendorGO.transform);
                leg.transform.localPosition = pos;
                leg.transform.localRotation = Quaternion.identity;
                leg.transform.localScale = new Vector3(0.07f, 0.7f, 0.07f);
                leg.GetComponent<Renderer>().material = legMat;
                leg.layer = LayerMask.NameToLayer("Default");
                leg.GetComponent<Collider>().isTrigger = false;
            }

            var orb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            orb.name = "MagicOrb";
            orb.transform.SetParent(vendorGO.transform);
            orb.transform.localPosition = new Vector3(0f, 0.92f, 0f);
            orb.transform.localRotation = Quaternion.identity;
            orb.transform.localScale = Vector3.one * 0.22f;
            var orbMat = new Material(shader);
            orbMat.color = new Color(0.6f, 0f, 1f);
            orbMat.EnableKeyword("_EMISSION");
            orbMat.SetColor("_EmissionColor", new Color(0.5f, 0f, 0.9f) * 3f);
            orb.GetComponent<Renderer>().material = orbMat;
            UnityEngine.Object.Destroy(orb.GetComponent<Collider>());

            var intColliderGO = new GameObject("InteractionCollider");
            intColliderGO.transform.SetParent(vendorGO.transform);
            intColliderGO.transform.localPosition = new Vector3(0f, 0.55f, 0f);
            intColliderGO.transform.localRotation = Quaternion.identity;
            intColliderGO.layer = LayerMask.NameToLayer("Interaction");

            var col = intColliderGO.AddComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = new Vector3(1.2f, 1.4f, 0.7f);

            var intObj = intColliderGO.AddComponent<InteractableObject>();
            intObj.SetMessage("Browse The Alchemist Table");
            intObj.MaxInteractionRange = 3f;
            intObj.SetInteractableState(InteractableObject.EInteractableState.Default);
            intObj.onInteractStart.AddListener(new UnityEngine.Events.UnityAction(() =>
            {
                var lm = NetworkSingleton<LevelManager>.Instance;
                if (lm == null) return;

                bool meetsRank = lm.GetFullRank() >= new FullRank(ERank.Enforcer, 4);
                if (!meetsRank)
                {
                    MelonLoader.MelonCoroutines.Start(ShowRejectionMessage());
                    return;
                }

                OpenAlchemistShop();
            }));

            MelonLoader.MelonCoroutines.Start(AddVendorMapPOI(vendorGO));
        }

        private static void OpenAlchemistShop()
        {
            if (_magicJuiceShop == null) return;
            _magicJuiceShop.SetIsOpen(true);

            if (_magicJuiceCanvas != null) _magicJuiceCanvas.enabled = true;
            if (_magicJuiceContainer != null) _magicJuiceContainer.gameObject.SetActive(true);

            MelonLoader.MelonCoroutines.Start(WatchForShopClose());
        }

        private static void ForceCloseAlchemistShop()
        {
            if (_magicJuiceShop == null) return;

            _magicJuiceShop.SetIsOpen(false);

            if (_magicJuiceCanvas != null) _magicJuiceCanvas.enabled = false;
            if (_magicJuiceContainer != null) _magicJuiceContainer.gameObject.SetActive(false);
        }

        private static System.Collections.IEnumerator WatchForShopClose()
        {
            yield return new WaitForSeconds(0.3f);

            while (_magicJuiceShop != null && _magicJuiceShop.IsOpen)
            {
                if (GameInput.GetButtonDown(GameInput.ButtonCode.Escape) ||
                    GameInput.GetButtonDown(GameInput.ButtonCode.Back))
                {
                    ForceCloseAlchemistShop();
                    yield break;
                }

                var player = PlayerSingleton<PlayerMovement>.Instance;
                if (player != null)
                {
                    float dist = Vector3.Distance(
                        player.transform.position,
                        new Vector3(-15.70f, -5.04f, 173.82f));
                    if (dist > 8f)
                    {
                        ForceCloseAlchemistShop();
                        yield break;
                    }
                }

                yield return null;
            }

            if (_magicJuiceCanvas != null) _magicJuiceCanvas.enabled = false;
            if (_magicJuiceContainer != null) _magicJuiceContainer.gameObject.SetActive(false);
        }

        private static System.Collections.IEnumerator ShowRejectionMessage()
        {
            float elapsed = 0f;
            float duration = 0.9f;
            while (elapsed < duration)
            {
                Singleton<HUD>.Instance?.CrosshairText?.Show(
                    "Get outta here kid.", new Color32(255, 80, 80, 255));
                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        public static bool _alchemistMessageSent = false;

        private static System.Collections.IEnumerator AddVendorMapPOI(GameObject vendorGO)
        {
            yield return new WaitForSeconds(1f);

            var lm = NetworkSingleton<LevelManager>.Instance;
            if (lm == null) yield break;

            var existingPOI = UnityEngine.Object.FindObjectOfType<POI>();
            if (existingPOI == null)
            {
                MelonLogger.Warning("[Vendor] Could not find POI to clone.");
                yield break;
            }

            var poiGO = UnityEngine.Object.Instantiate(existingPOI.gameObject, vendorGO.transform);
            poiGO.transform.localPosition = new Vector3(0f, 2f, 0f);
            var poi = poiGO.GetComponent<POI>();
            poi.SetMainText("The Alchemist Table");

            poi.onUICreated.AddListener(() =>
            {
                MelonLoader.MelonCoroutines.Start(SetPOIIcon(poi));
            });

            bool meetsRank = lm.GetFullRank() >= new FullRank(ERank.Enforcer, 4);
            poi.enabled = meetsRank;
            poiGO.SetActive(meetsRank);

            if (meetsRank && !_alchemistMessageSent)
            {
                _alchemistMessageSent = true;
                CreateAlchemistConversation();
                SendAlchemistPhoneMessage();
            }

            lm.onRankChanged += new Action<FullRank, FullRank>((before, after) =>
            {
                bool show = after >= new FullRank(ERank.Enforcer, 4);
                poi.enabled = show;
                poiGO.SetActive(show);

                if (show && !_alchemistMessageSent)
                {
                    _alchemistMessageSent = true;
                    CreateAlchemistConversation();
                    SendAlchemistPhoneMessage();
                }
            });
        }

        private static System.Collections.IEnumerator SetPOIIcon(POI poi)
        {
            yield return null;

            if (poi.IconContainer == null)
            {
                MelonLogger.Warning("[Vendor] POI IconContainer is null.");
                yield break;
            }

            for (int i = poi.IconContainer.childCount - 1; i >= 0; i--)
                UnityEngine.Object.Destroy(poi.IconContainer.GetChild(i).gameObject);

            yield return null;

            var tex = LoadCustomTexture("MagicJuice.png");
            if (tex == null)
            {
                MelonLogger.Warning("[Vendor] Could not load MagicJuice.png for POI icon.");
                yield break;
            }

            var iconGO = new GameObject("AlchemistIcon");
            iconGO.transform.SetParent(poi.IconContainer, false);

            var rect = iconGO.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(20f, 20f);

            var img = iconGO.AddComponent<UnityEngine.UI.Image>();
            img.sprite = Sprite.Create(tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f));
            img.color = Color.white;
            img.preserveAspect = true;
        }

        private static void SendAlchemistUnlockNotification()
        {
            MelonLoader.MelonCoroutines.Start(SendAlchemistNotificationDelayed());
        }

        private static System.Collections.IEnumerator SendAlchemistNotificationDelayed()
        {
            yield return new WaitForSeconds(2f);

            var nm = Singleton<NotificationsManager>.Instance;
            if (nm == null)
            {
                MelonLogger.Warning("[Vendor] NotificationsManager not found.");
                yield break;
            }

            Sprite notifIcon = null;
            var tex = LoadCustomTexture("MagicJuice.png");
            if (tex != null)
                notifIcon = Sprite.Create(tex,
                    new Rect(0, 0, tex.width, tex.height),
                    new Vector2(0.5f, 0.5f));

            nm.SendNotification(
                "The Alchemist Table",
                "The alchemist table beckons you to pay a visit... You can find it on the map.",
                notifIcon,
                8f,
                true
            );
        }

        private static MSGConversation _alchemistConversation;

        private static void CreateAlchemistConversation()
        {
            try
            {
                NPC sourceNPC = null;
                foreach (var n in NPCManager.NPCRegistry) { if (n != null) { sourceNPC = n; break; } }
                if (sourceNPC == null)
                {
                    MelonLogger.Warning("[Alchemist] No NPC found to clone.");
                    return;
                }

                var fakeNPCGO = UnityEngine.Object.Instantiate(sourceNPC.gameObject);
                fakeNPCGO.name = "AlchemistFakeNPC";

                fakeNPCGO.transform.position = new Vector3(0f, -1000f, 0f);

                var fakeNPC = fakeNPCGO.GetComponent<NPC>();

                fakeNPC.FirstName = "The Alchemist Table";
                fakeNPC.hasLastName = false;
                fakeNPC.LastName = "";
                fakeNPC.ID = "the_alchemist_vendor";

                fakeNPCGO.SetActive(false);

                var tex = LoadCustomTexture("MagicJuice.png");
                if (tex != null)
                {
                    var sprite = Sprite.Create(tex,
                        new Rect(0, 0, tex.width, tex.height),
                        new Vector2(0.5f, 0.5f));

                    var mugshotField = typeof(NPC).GetField("MugshotSprite",
                        System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    mugshotField?.SetValue(fakeNPC, sprite);
                }

                fakeNPCGO.SetActive(true);
                fakeNPCGO.SetActive(false);

                _alchemistConversation = new MSGConversation(fakeNPC, "The Alchemist Table");
                _alchemistConversation.SetIsKnown(true);
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[Alchemist] Failed to create conversation: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private static void SendAlchemistPhoneMessage()
        {
            if (_alchemistConversation == null)
            {
                MelonLogger.Warning("[Alchemist] Conversation not ready.");
                return;
            }

            MelonLoader.MelonCoroutines.Start(SendAlchemistMessageDelayed());
        }

        private static System.Collections.IEnumerator SendAlchemistMessageDelayed()
        {
            yield return new WaitForSeconds(2f);

            try
            {
                var msg = new Message(
                    "The alchemist table beckons you to pay a visit... You can find it on the map.",
                    Message.ESenderType.Other,
                    _endOfGroup: true,
                    UnityEngine.Random.Range(int.MinValue, int.MaxValue)
                );

                _alchemistConversation.SendMessage(msg, notify: true, network: false);
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[Alchemist] Failed to send message: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}