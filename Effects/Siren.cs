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
    public class SirenEffect : VoluptuousEffect
    {
        private const string SIREN_KEY = "siren_skin";
        private static readonly Color RoseGold = new Color(0.9f, 0.6f, 0.55f);
        private static readonly Color SirenEye = new Color(1.0f, 0.5f, 0.65f);

        protected override Color EyeGlamColor  => SirenEye;
        protected override Color GlowColor     => new Color(0.4f, 0.1f, 0.2f);
        protected override Color LipColor      => Color.white;

        protected override void ApplyVisuals(Avatar avatar, AvatarEffects fx, bool isPlayer)
        {
            base.ApplyVisuals(avatar, fx, isPlayer);
            fx?.SkinColorSmoother.AddOverride(RoseGold, 8, SIREN_KEY);
            if (isPlayer)
                PlayerMovement.StaticMoveSpeedMultiplier = 1.15f;
            else if (avatar != null)
            {
                var npcMovement = avatar.GetComponentInParent<NPC>();
                if (npcMovement?.Movement?.SpeedController != null)
                    npcMovement.Movement.SpeedController.SpeedMultiplier = 1.15f;
            }
        }

        public override void ClearFromPlayer(Player player)
        {
            base.ClearFromPlayer(player);
            player?.Avatar?.Effects?.SkinColorSmoother.RemoveOverride(SIREN_KEY);
            PlayerMovement.StaticMoveSpeedMultiplier = 1.0f;
        }

        public override void ClearFromNPC(NPC npc)
        {
            base.ClearFromNPC(npc);
            npc?.Avatar?.Effects?.SkinColorSmoother.RemoveOverride(SIREN_KEY);
            if (npc?.Movement?.SpeedController != null)
                npc.Movement.SpeedController.SpeedMultiplier = 1.0f;
        }
    }
}

namespace DomsExpandedIngredientsAndEffects.Effects.Descriptors
{
    public class SirenEffectDef : CustomEffect
    {
        public override string ID              => "sireneffect";
        public override string Name            => "Siren";
        public override Color  Color           => new Color(0.9f, 0.6f, 0.55f);
        public override float  Addictiveness   => 0.30f;
        public override int    ValueChange     => 20;
        public override float  ValueMultiplier => 1.25f;

        public override Effect CreateInstance() => UnityEngine.ScriptableObject.CreateInstance<SirenEffect>();
    }
}