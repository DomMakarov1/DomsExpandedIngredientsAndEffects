using UnityEngine;
using DomsExpandedIngredientsAndEffects.Framework;
using DomsExpandedIngredientsAndEffects.Effects;
using DomsExpandedIngredientsAndEffects.Effects.Descriptors;

#if MONO
using ScheduleOne.Product;
using ScheduleOne.ItemFramework;
using ScheduleOne.DevUtilities;
using ScheduleOne;
#else
using Il2CppScheduleOne.Product;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne;
#endif

namespace DomsExpandedIngredientsAndEffects.Ingredients
{
    public class MoonDustIngredient : CustomIngredient
    {
        public static PropertyItemDefinition Definition { get; private set; }
        public override string       ID            => "moondust";
        public override string       Name          => "Moon Dust";
        public override string       IconFileName  => "MoonDust.png";

        public override CustomEffect MixEffect     => new HeliumInfusionEffect();
        public override CustomEffect MapEffect     => new MoonGravityEffectDef();

        public override Vector2      MapPosition   => new Vector2(0f, 3.5f);
        public override float        MapRadius     => 0.45f;
        public override EDrugType    TargetDrugType => EDrugType.Marijuana;

        public new void Register(Registry registry, ProductManager productManager)
        {
            base.Register(registry, productManager);
            Definition = ItemDefinition;
        }
    }
}