using UnityEngine;
using DomsExpandedIngredientsAndEffects.Framework;
using DomsExpandedIngredientsAndEffects.Effects;

#if MONO
using ScheduleOne.Effects;
using ScheduleOne.NPCs;
using ScheduleOne.PlayerScripts;
using ScheduleOne.DevUtilities;
#else
using Il2CppScheduleOne.Effects;
using Il2CppScheduleOne.NPCs;

using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.DevUtilities;
#endif

namespace DomsExpandedIngredientsAndEffects.Effects
{
    public class MoonGravityEffect : Effect 
    {
        private const string STACK_KEY = "moongravity";

        public override void ApplyToPlayer(Player player)
        {
            if (player != null && player.IsOwner)
            {
                PlayerMovement.JumpMultiplier = 1.3f;    
                PlayerMovement.GravityMultiplier = 0.8f; 
            }
        }

        public override void ClearFromPlayer(Player player)
        {
            if (player != null && player.IsOwner)
            {
                PlayerMovement.JumpMultiplier = 1.0f;
                PlayerMovement.GravityMultiplier = 1.0f;
            }
        }

        public override void ApplyToNPC(NPC npc) { }
        public override void ClearFromNPC(NPC npc) { }
    }
}

namespace DomsExpandedIngredientsAndEffects.Effects.Descriptors
{
    public class MoonGravityEffectDef : CustomEffect
    {
        public override string  ID           => "moongravity";
        public override string  Name         => "Moon Gravity";
        public override Color   Color        => new Color(0.6f, 0.2f, 0.8f);
        public override Vector2 MixDirection => new Vector2(0, 1).normalized;
        public override float   MixMagnitude => 0.45f;
        public override float Addictiveness   => 0.35f;
        public override int   ValueChange     => 17;
        public override float ValueMultiplier => 1.1f;

        public override Effect CreateInstance() =>UnityEngine.ScriptableObject.CreateInstance<MoonGravityEffect>();
    }
}