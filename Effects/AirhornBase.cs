using UnityEngine;
using DomsExpandedIngredientsAndEffects.Framework;
using DomsExpandedIngredientsAndEffects.Utilities;

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
    public class AirhornBaseEffect : Effect
    {
        private static AudioClip _airhornClip;

        public override void ApplyToPlayer(Player player)
        {
            if (player == null || !player.IsOwner) return;
            PlayAirhorn(player.transform.position);
        }

        public override void ClearFromPlayer(Player player) { }

        public override void ApplyToNPC(NPC npc)
        {
            if (npc == null) return;
            PlayAirhorn(npc.transform.position);
        }

        public override void ClearFromNPC(NPC npc) { }

        private static void PlayAirhorn(Vector3 position)
        {
            if (_airhornClip == null)
                _airhornClip = DomsCustomEffects.LoadCustomSound("Airhorn.wav");

            if (_airhornClip == null)
            {
                MelonLoader.MelonLogger.Warning("[Airhorn] Airhorn.wav not found in Sounds folder.");
                return;
            }

            AudioSource.PlayClipAtPoint(_airhornClip, position, SoundHelper.SFXVolume);
        }
    }
}

namespace DomsExpandedIngredientsAndEffects.Effects.Descriptors
{
    public class AirhornBaseEffectDef : CustomEffect
    {
        public override string ID              => "airhornbase";
        public override string Name            => "Airhorn";
        public override Color  Color           => new Color(1.0f, 0.5f, 0.0f);
        public override float  Addictiveness   => 0.05f;
        public override int    ValueChange     => 5;
        public override float  ValueMultiplier => 1.0f;

        public override Effect CreateInstance() =>UnityEngine.ScriptableObject.CreateInstance<AirhornBaseEffect>();
    }
}