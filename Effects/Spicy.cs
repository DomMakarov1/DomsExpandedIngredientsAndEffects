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
    public class SpicyEffect : Effect
    {
        public override void ApplyToNPC(NPC npc) { }
        public override void ClearFromNPC(NPC npc) { }
        public override void ApplyToPlayer(Player player) { }
        public override void ClearFromPlayer(Player player) { }
    }

    public class HotSauceMapEffect : Effect
    {
        public override void ApplyToNPC(NPC npc) { }
        public override void ClearFromNPC(NPC npc) { }
        public override void ApplyToPlayer(Player player) { }
        public override void ClearFromPlayer(Player player) { }
    }
}

namespace DomsExpandedIngredientsAndEffects.Effects.Descriptors
{
    public class SpicyEffectDef : CustomEffect
    {
        public override string ID              => "spicyeffect";
        public override string Name            => "Spicy";
        public override Color  Color           => new Color(1.0f, 0.18f, 0.0f);
        public override float  Addictiveness   => 0.07f;
        public override int    ValueChange     => 10;
        public override float  ValueMultiplier => 1.05f;

        public override Effect CreateInstance() => UnityEngine.ScriptableObject.CreateInstance<SpicyEffect>();
    }

    public class HotSauceMapEffectDef : CustomEffect
    {
        public override string ID    => "hotsaucemap";
        public override string Name  => "Hot Sauce";
        public override Color  Color => new Color(1.0f, 0.18f, 0.0f);

        public override Effect CreateInstance() => UnityEngine.ScriptableObject.CreateInstance<HotSauceMapEffect>();
    }
}
