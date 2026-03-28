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
    public class DivaEffect : VoluptuousEffect
    {
        private const string DIVA_KEY = "diva";
        private static readonly Color DivaEye  = new Color(1.0f, 1.0f, 1.0f);
        private static readonly Color PlatBlonde = new Color(1.0f, 0.97f, 0.85f);

        protected override Color EyeGlamColor => DivaEye;
        protected override Color GlowColor    => new Color(0.3f, 0.3f, 0.3f);
        protected override Color LipColor     => Color.white;

        protected override void ApplyVisuals(Avatar avatar, AvatarEffects fx, bool isPlayer)
        {
            base.ApplyVisuals(avatar, fx, isPlayer);

            fx?.OverrideHairColor(PlatBlonde);
            fx?.OverrideEyeColor(DivaEye, 0.5f);

            if (isPlayer)
                PlayerMovement.StaticMoveSpeedMultiplier = 1.25f;
            else if (avatar != null)
            {
                var npc = avatar.GetComponentInParent<NPC>();
                if (npc?.Movement?.SpeedController != null)
                    npc.Movement.SpeedController.SpeedMultiplier = 1.25f;
            }
        }

        public override void ClearFromPlayer(Player player)
        {
            base.ClearFromPlayer(player);
            player?.Avatar?.Effects?.ResetHairColor();
            PlayerMovement.StaticMoveSpeedMultiplier = 1.0f;
        }

        public override void ClearFromNPC(NPC npc)
        {
            base.ClearFromNPC(npc);
            npc?.Avatar?.Effects?.ResetHairColor();
            if (npc?.Movement?.SpeedController != null)
                npc.Movement.SpeedController.SpeedMultiplier = 1.0f;
        }
    }
}

namespace DomsExpandedIngredientsAndEffects.Effects.Descriptors
{
    public class DivaEffectDef : CustomEffect
    {
        public override string ID              => "divaeffect";
        public override string Name            => "Diva";
        public override Color  Color           => new Color(1.0f, 0.97f, 0.85f);
        public override float  Addictiveness   => 0.35f;
        public override int    ValueChange     => 25;
        public override float  ValueMultiplier => 1.3f;

        public override Effect CreateInstance() => UnityEngine.ScriptableObject.CreateInstance<DivaEffect>();
    }
}