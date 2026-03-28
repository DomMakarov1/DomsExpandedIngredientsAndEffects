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
    public class AirhornMapEffect : Effect
    {
        public override void ApplyToNPC(NPC npc) { }
        public override void ClearFromNPC(NPC npc) { }
        public override void ApplyToPlayer(Player player) { }
        public override void ClearFromPlayer(Player player) { }
    }
}

namespace DomsExpandedIngredientsAndEffects.Effects.Descriptors
{
    public class AirhornMapEffectDef : CustomEffect
    {
        public override string ID    => "airhornmap";
        public override string Name  => "Airhorn";
        public override UnityEngine.Color Color => new UnityEngine.Color(1.0f, 0.5f, 0.0f);

        public override Effect CreateInstance() =>UnityEngine.ScriptableObject.CreateInstance<AirhornMapEffect>();
    }
}