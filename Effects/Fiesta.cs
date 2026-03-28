using UnityEngine;
using DomsExpandedIngredientsAndEffects.Framework;
using DomsExpandedIngredientsAndEffects.Utilities;

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
    public class FiestaEffect : Effect
    {
        private static AudioClip _partyClip;

        public override void ApplyToPlayer(Player player)
        {
            if (player == null || !player.IsOwner) return;
            var fx = player.Avatar?.Effects;
            if (fx == null) return;

            PlayParty(player.transform.position);
            PlayerMovement.StaticMoveSpeedMultiplier = 1.2f;
            fx.OverrideEyeColor(new Color(1.0f, 0.5f, 0.0f), 0.4f);
            fx.SetGlowingOn(new Color(0.4f, 0.2f, 0.0f));

            fx.HeadPoofParticles?.Play();
        }

        public override void ClearFromPlayer(Player player)
        {
            if (player == null || !player.IsOwner) return;
            var fx = player.Avatar?.Effects;
            if (fx == null) return;

            PlayerMovement.StaticMoveSpeedMultiplier = 1.0f;
            fx.ResetEyeColor();
            fx.SetGlowingOff();
        }

        public override void ApplyToNPC(NPC npc)
        {
            if (npc == null) return;
            var fx = npc.Avatar?.Effects;
            if (fx == null) return;

            PlayParty(npc.transform.position);
            if (npc.Movement?.SpeedController != null)
                npc.Movement.SpeedController.SpeedMultiplier = 1.2f;
            fx.OverrideEyeColor(new Color(1.0f, 0.5f, 0.0f), 0.4f);
            fx.SetGlowingOn(new Color(0.4f, 0.2f, 0.0f));
            fx.HeadPoofParticles?.Play();
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
        }

        private static void PlayParty(Vector3 position)
        {
            if (_partyClip == null)
                _partyClip = DomsCustomEffects.LoadCustomSound("Party.wav");
            if (_partyClip == null)
            {
                MelonLoader.MelonLogger.Warning("[Fiesta] Party.wav not found.");
                return;
            }
            AudioSource.PlayClipAtPoint(_partyClip, position, SoundHelper.SFXVolume);
        }
    }
}

namespace DomsExpandedIngredientsAndEffects.Effects.Descriptors
{
    public class FiestaEffectDef : CustomEffect
    {
        public override string ID              => "fiestaeffect";
        public override string Name            => "Fiesta";
        public override Color  Color           => new Color(1.0f, 0.5f, 0.0f);
        public override float  Addictiveness   => 0.30f;
        public override int    ValueChange     => 18;
        public override float  ValueMultiplier => 1.2f;

        public override Effect CreateInstance() =>UnityEngine.ScriptableObject.CreateInstance<FiestaEffect>();
    }
}