using UnityEngine;
using DomsExpandedIngredientsAndEffects.Effects;

#if MONO
using ScheduleOne.Effects;
#else
using Il2CppScheduleOne.Effects;
#endif

namespace DomsExpandedIngredientsAndEffects.Framework
{
    public abstract class CustomEffect
    {
        public abstract string ID    { get; }
        public abstract string Name  { get; }
        public abstract Color  Color { get; }

        public virtual Vector2 MixDirection => Vector2.up;
        public virtual float   MixMagnitude => 0.45f;

        public virtual float Addictiveness   => 0.1f;
        public virtual int   ValueChange     => 0;
        public virtual float ValueMultiplier => 1.0f;

        public abstract Effect CreateInstance();

        public Effect Build()
        {
            Effect e       = CreateInstance();
            e.name         = ID;
            e.Name         = Name;
            e.ID           = ID;
            e.ProductColor = Color;
            e.LabelColor   = Color;
            e.MixDirection = MixDirection;
            e.MixMagnitude = MixMagnitude;
            e.Addictiveness   = Addictiveness;
            e.ValueChange     = ValueChange;
            e.ValueMultiplier = ValueMultiplier;

            return e;
        }

        protected static Effect CreateEffectInstance(System.Type type)
        {
            return (Effect)UnityEngine.ScriptableObject.CreateInstance(type.Name);
        }
    }
}