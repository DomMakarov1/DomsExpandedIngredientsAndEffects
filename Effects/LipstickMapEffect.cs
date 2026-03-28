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
    public class LipstickMapEffect : Effect
    {
        public override void ApplyToNPC(NPC npc) { }
        public override void ClearFromNPC(NPC npc) { }
        public override void ApplyToPlayer(Player player) { }
        public override void ClearFromPlayer(Player player) { }
    }
}

namespace DomsExpandedIngredientsAndEffects.Effects.Descriptors
{
    public class LipstickMapEffectDef : CustomEffect
    {
        public override string ID    => "lipstickmap";
        public override string Name  => "Lipstick";
        public override Color  Color => new Color(0.9f, 0.1f, 0.3f);

        public override Effect CreateInstance() => UnityEngine.ScriptableObject.CreateInstance<LipstickMapEffect>();
    }
}