using UnityEngine;
using DomsExpandedIngredientsAndEffects.Framework;
using DomsExpandedIngredientsAndEffects.Effects;

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
    public class MoonDustMixerEffect : Effect
    {
        public override void ApplyToNPC(NPC npc) { }
        public override void ClearFromNPC(NPC npc) { }
        public override void ApplyToPlayer(Player player) { }
        public override void ClearFromPlayer(Player player) { }
    }
}

namespace DomsExpandedIngredientsAndEffects.Effects.Descriptors
{
    public class HeliumInfusionEffect : CustomEffect
    {
        public override string ID    => "heliuminfusion";
        public override string Name  => "Helium Infusion";
        public override Color  Color => new Color(0.4f, 0.7f, 1.0f);
        public override float Addictiveness   => 0.1f;
        public override int   ValueChange     => 4;
        public override float ValueMultiplier => 1.1f;

        public override Effect CreateInstance() => UnityEngine.ScriptableObject.CreateInstance<MoonDustMixerEffect>();
    }
}