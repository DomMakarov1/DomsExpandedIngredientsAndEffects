using UnityEngine;
using DomsExpandedIngredientsAndEffects.Framework;

#if MONO
using ScheduleOne.Effects;
using ScheduleOne.NPCs;
using ScheduleOne.PlayerScripts;
using ScheduleOne.AvatarFramework;
using Avatar = ScheduleOne.AvatarFramework.Avatar;
#else
using Il2CppScheduleOne.Effects;
using Il2CppScheduleOne.NPCs;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.AvatarFramework;
using Avatar = Il2CppScheduleOne.AvatarFramework.Avatar;

#endif

namespace DomsExpandedIngredientsAndEffects.Effects
{
    public class ChargedEffect : Effect
    {
        private const string STACK_KEY = "charged";
        private static readonly Color ElectricYellow = new Color(1.0f, 0.95f, 0.1f);

        public override void ApplyToPlayer(Player player)
        {
            if (player == null || !player.IsOwner) return;
            var fx = player.Avatar?.Effects;
            if (fx == null) return;

            PlayerMovement.StaticMoveSpeedMultiplier = 1.3f;
            fx.OverrideEyeColor(ElectricYellow, 0.6f);
            fx.SetGlowingOn(new Color(0.5f, 0.5f, 0.0f));
            fx.SetZapped(true);
        }

        public override void ClearFromPlayer(Player player)
        {
            if (player == null || !player.IsOwner) return;
            var fx = player.Avatar?.Effects;
            if (fx == null) return;

            PlayerMovement.StaticMoveSpeedMultiplier = 1.0f;
            fx.ResetEyeColor();
            fx.SetGlowingOff();
            fx.SetZapped(false);
        }

        public override void ApplyToNPC(NPC npc)
        {
            if (npc == null) return;
            var fx = npc.Avatar?.Effects;
            if (fx == null) return;

            if (npc.Movement?.SpeedController != null)
                npc.Movement.SpeedController.SpeedMultiplier = 1.3f;
            fx.OverrideEyeColor(ElectricYellow, 0.6f);
            fx.SetGlowingOn(new Color(0.5f, 0.5f, 0.0f));
            fx.SetZapped(true);
        }

        public override void ClearFromNPC(NPC npc)
        {
            if (npc == null) return;
            var fx = npc.Avatar?.Effects;
            if (fx == null) return;

            if (npc.Movement?.SpeedController != null)
                npc.Movement.SpeedController.SpeedMultiplier = 1.0f;
            fx.ResetEyeColor();
            fx.SetGlowingOff();
            fx.SetZapped(false);
        }
    }
}

namespace DomsExpandedIngredientsAndEffects.Effects.Descriptors
{
    public class ChargedEffectDef : CustomEffect
    {
        public override string ID              => "chargedeffect";
        public override string Name            => "Charged";
        public override Color  Color           => new Color(1.0f, 0.95f, 0.1f);
        public override float  Addictiveness   => 0.40f;
        public override int    ValueChange     => 35;
        public override float  ValueMultiplier => 1.4f;

        public override Effect CreateInstance() =>
            UnityEngine.ScriptableObject.CreateInstance<ChargedEffect>();
    }
}