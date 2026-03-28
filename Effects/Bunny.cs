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
    public class BunnyEffect : Effect
    {
        private const string STACK_KEY = "bunny";

        public override void ApplyToPlayer(Player player)
        {
            if (player == null || !player.IsOwner) return;
            PlayerMovement.JumpMultiplier    = 2.0f;
            PlayerMovement.GravityMultiplier = 1.0f;
        }

        public override void ClearFromPlayer(Player player)
        {
            if (player == null || !player.IsOwner) return;
            PlayerMovement.JumpMultiplier    = 1.0f;
            PlayerMovement.GravityMultiplier = 1.0f;
        }

        public override void ApplyToNPC(NPC npc) { }
        public override void ClearFromNPC(NPC npc) { }
    }
}

namespace DomsExpandedIngredientsAndEffects.Effects.Descriptors
{
    public class BunnyEffectDef : CustomEffect
    {
        public override string ID             => "bunnyeffect";
        public override string Name           => "Bunny";
        public override Color  Color          => new Color(1.0f, 0.75f, 0.8f);
        public override float  Addictiveness  => 0.15f;
        public override int    ValueChange    => 20;
        public override float  ValueMultiplier => 1.2f;

        public override Effect CreateInstance() =>UnityEngine.ScriptableObject.CreateInstance<BunnyEffect>();
    }
}