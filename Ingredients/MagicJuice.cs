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
    public class MagicJuiceIngredient : CustomIngredient
    {
        public static PropertyItemDefinition Definition { get; private set; }
        public override string    ID            => "magicjuice";
        public override string    Name          => "Magic Juice";
        public override string    IconFileName  => "MagicJuice.png";

        public override CustomEffect MixEffect  => new MagicBaseEffectDef();
        public override CustomEffect MapEffect  => new MagicJuiceMapEffectDef();

        public override Vector2   MapPosition   => new Vector2(-3.0f, 1.0f);
        public override float     MapRadius     => 0.45f;
        public override EDrugType TargetDrugType => EDrugType.Marijuana;

        public new void Register(Registry registry, ProductManager productManager)
        {
            base.Register(registry, productManager);
            Definition = ItemDefinition;
        }
    }
}