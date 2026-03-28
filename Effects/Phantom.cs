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
    public class PhantomEffect : Effect
    {
        private const string STACK_KEY  = "phantom";
        private static readonly Color PhantomNavy = new Color(0.1f, 0.1f, 0.35f);

        public override void ApplyToPlayer(Player player)
        {
            if (player == null || !player.IsOwner) return;
            PlayerMovement.StaticMoveSpeedMultiplier = 1.2f;
            player.Avatar?.Effects?.SkinColorSmoother.AddOverride(PhantomNavy, 7, STACK_KEY);
        }

        public override void ClearFromPlayer(Player player)
        {
            if (player == null || !player.IsOwner) return;
            PlayerMovement.StaticMoveSpeedMultiplier = 1.0f;
            player.Avatar?.Effects?.SkinColorSmoother.RemoveOverride(STACK_KEY);
        }

        public override void ApplyToNPC(NPC npc)
        {
            if (npc == null) return;
            if (npc.Movement?.SpeedController != null)
                npc.Movement.SpeedController.SpeedMultiplier = 1.2f;
            npc.Avatar?.Effects?.SkinColorSmoother.AddOverride(PhantomNavy, 7, STACK_KEY);
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
    public class PhantomEffectDef : CustomEffect
    {
        public override string ID              => "phantomeffect";
        public override string Name            => "Phantom";
        public override Color  Color           => new Color(0.1f, 0.1f, 0.35f);
        public override float  Addictiveness   => 0.25f;
        public override int    ValueChange     => 22;
        public override float  ValueMultiplier => 1.2f;

        public override Effect CreateInstance() => UnityEngine.ScriptableObject.CreateInstance<PhantomEffect>();
    }
}