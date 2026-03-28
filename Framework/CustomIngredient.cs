using UnityEngine;
using System;
using MelonLoader;

#if MONO
using ScheduleOne;
using ScheduleOne.ItemFramework;
using ScheduleOne.Product;
using ScheduleOne.DevUtilities;
using ScheduleOne.Effects;
using ScheduleOne.Effects.MixMaps;
#else
using Il2CppScheduleOne;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.Product;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.Effects;
using Il2CppScheduleOne.Effects.MixMaps;
#endif

namespace DomsExpandedIngredientsAndEffects.Framework
{
    public abstract class CustomIngredient
    {
        public abstract string    ID           { get; }
        public abstract string    Name         { get; }
        public abstract string    IconFileName { get; }
        public abstract CustomEffect MixEffect { get; }
        public abstract CustomEffect MapEffect { get; }
        public abstract Vector2   MapPosition    { get; }
        public abstract float     MapRadius      { get; }
        public abstract EDrugType TargetDrugType { get; }
        public PropertyItemDefinition ItemDefinition { get; private set; }
        public void Register(Registry registry, ProductManager productManager)
        {
            try
            {
                Effect mixEffect = MixEffect.Build();
                Effect mapEffect = MapEffect.Build();

                PropertyItemDefinition iodine = Registry.GetItem<PropertyItemDefinition>("iodine");
                if (iodine == null)
                {
                    MelonLogger.Error($"[{Name}] Could not find iodine template.");
                    return;
                }

                ItemDefinition             = UnityEngine.Object.Instantiate(iodine);
                ItemDefinition.name        = ID;
                ItemDefinition.Name        = Name;
                ItemDefinition.ID          = ID;
                #if MONO
                ItemDefinition.Properties = new System.Collections.Generic.List<Effect> { mixEffect };
                #else
                var props = new Il2CppSystem.Collections.Generic.List<Effect>();
                props.Add(mixEffect);
                ItemDefinition.Properties = props;
                #endif

                if (IconFileName != null)
                    ItemDefinition.Icon = DomsCustomEffects.LoadCustomIcon(IconFileName);

                registry.AddToRegistry(ItemDefinition);
                productManager.ValidMixIngredients.Add(ItemDefinition);

                MixerMap map = productManager.GetMixerMap(TargetDrugType);
                if (map != null)
                {
                    map.Effects.Add(new MixerMapEffect
                    {
                        Property = mapEffect,
                        Position = MapPosition,
                        Radius   = MapRadius
                    });
                }
                else
                {
                    MelonLogger.Warning($"[{Name}] MixerMap not found for {TargetDrugType}.");
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[{Name}] Registration failed: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}