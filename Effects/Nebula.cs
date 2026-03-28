using UnityEngine;
using DomsExpandedIngredientsAndEffects.Framework;

#if MONO
using ScheduleOne.Effects;
using ScheduleOne.NPCs;
using ScheduleOne.PlayerScripts;
using ScheduleOne.AvatarFramework;
#else
using Il2CppScheduleOne.Effects;
using Il2CppScheduleOne.NPCs;
using Il2CppScheduleOne.PlayerScripts;

using Il2CppScheduleOne.AvatarFramework;
#endif

namespace DomsExpandedIngredientsAndEffects.Effects
{
    public class NebulaEffect : Effect
    {
        private const string STACK_KEY = "nebula";
        private static readonly Color NebulaPurple = new Color(0.6f, 0.4f, 0.8f);

        public override void ApplyToPlayer(Player player)
        {
            if (player == null || !player.IsOwner) return;
            PlayerMovement.StaticMoveSpeedMultiplier = 0.8f;
            PlayerMovement.GravityMultiplier         = 0.5f;
            player.Avatar?.Effects?.SkinColorSmoother.AddOverride(NebulaPurple, 7, STACK_KEY);
        }

        public override void ClearFromPlayer(Player player)
        {
            if (player == null || !player.IsOwner) return;
            PlayerMovement.StaticMoveSpeedMultiplier = 1.0f;
            PlayerMovement.GravityMultiplier         = 1.0f;
            player.Avatar?.Effects?.SkinColorSmoother.RemoveOverride(STACK_KEY);
        }

        public override void ApplyToNPC(NPC npc)
        {
            if (npc == null) return;
            if (npc.Movement?.SpeedController != null)
                npc.Movement.SpeedController.SpeedMultiplier = 0.8f;
            npc.Avatar?.Effects?.SkinColorSmoother.AddOverride(NebulaPurple, 7, STACK_KEY);
        }

        public override void ClearFromNPC(NPC npc)
        {
            if (npc == null) return;
            if (npc.Movement?.SpeedController != null)
                npc.Movement.SpeedController.SpeedMultiplier = 1.0f;
            npc.Avatar?.Effects?.SkinColorSmoother.RemoveOverride(STACK_KEY);
        }
    }
}

namespace DomsExpandedIngredientsAndEffects.Effects.Descriptors
{
    public class NebulaEffectDef : CustomEffect
    {
        public override string ID              => "nebulaeffect";
        public override string Name            => "Nebula";
        public override Color  Color           => new Color(0.6f, 0.4f, 0.8f);
        public override float  Addictiveness   => 0.20f;
        public override int    ValueChange     => 18;
        public override float  ValueMultiplier => 1.15f;

        public override Effect CreateInstance() =>UnityEngine.ScriptableObject.CreateInstance<NebulaEffect>();
    }
}