using UnityEngine;
using DomsExpandedIngredientsAndEffects.Framework;

#if MONO
using ScheduleOne.Effects;
using ScheduleOne.NPCs;
using ScheduleOne.PlayerScripts;
#else
using Il2CppScheduleOne.Effects;
using Il2CppScheduleOne.NPCs;
using Il2CppScheduleOne.PlayerScripts;
#endif

namespace DomsExpandedIngredientsAndEffects.Effects
{
    public class EarthyEffect : Effect
    {
        public override void ApplyToNPC(NPC npc) { }
        public override void ClearFromNPC(NPC npc) { }
        public override void ApplyToPlayer(Player player) { }
        public override void ClearFromPlayer(Player player) { }
    }

    public class FertilizerMapEffect : Effect
    {
        public override void ApplyToNPC(NPC npc) { }
        public override void ClearFromNPC(NPC npc) { }
        public override void ApplyToPlayer(Player player) { }
        public override void ClearFromPlayer(Player player) { }
    }
}

namespace DomsExpandedIngredientsAndEffects.Effects.Descriptors
{
    public class EarthyEffectDef : CustomEffect
    {
        public override string ID              => "earthyeffect";
        public override string Name            => "Earthy";
        public override Color  Color           => new Color(0.72f, 0.48f, 0.18f);
        public override float  Addictiveness   => 0.05f;
        public override int    ValueChange     => 6;
        public override float  ValueMultiplier => 1.0f;

        public override Effect CreateInstance() => UnityEngine.ScriptableObject.CreateInstance<EarthyEffect>();
    }

    public class FertilizerMapEffectDef : CustomEffect
    {
        public override string ID    => "fertilizermap";
        public override string Name  => "Fertilizer";
        public override Color  Color => new Color(0.72f, 0.48f, 0.18f);

        public override Effect CreateInstance() => UnityEngine.ScriptableObject.CreateInstance<FertilizerMapEffect>();
    }
}
