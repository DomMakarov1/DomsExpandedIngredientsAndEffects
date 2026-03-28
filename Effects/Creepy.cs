using UnityEngine;
using System.Collections;
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
    public class CreepyEffect : Effect
    {
        private static AudioClip _creepyClip;
        private static bool      _playerPlaying;

        public override void ApplyToPlayer(Player player)
        {
            if (player == null || !player.IsOwner) return;
            if (_playerPlaying) return;
            _playerPlaying = true;
            MelonLoader.MelonCoroutines.Start(PlayCreepyLoop(player.transform, true));
        }

        public override void ClearFromPlayer(Player player)
        {
            _playerPlaying = false;
        }

        public override void ApplyToNPC(NPC npc)
        {
            if (npc == null) return;
            MelonLoader.MelonCoroutines.Start(PlayCreepyLoop(npc.transform, false));
        }

        public override void ClearFromNPC(NPC npc) { }

        private static IEnumerator PlayCreepyLoop(Transform target, bool isPlayer)
        {
            if (_creepyClip == null)
                _creepyClip = DomsCustomEffects.LoadCustomSound("Creepy.wav");

            if (_creepyClip == null)
            {
                MelonLoader.MelonLogger.Warning("[Creepy] Creepy.wav not found.");
                yield break;
            }

            AudioSource.PlayClipAtPoint(_creepyClip, target.position, SoundHelper.MusicVolume);
        }
    }
}

namespace DomsExpandedIngredientsAndEffects.Effects.Descriptors
{
    public class CreepyEffectDef : CustomEffect
    {
        public override string ID              => "creepyeffect";
        public override string Name            => "Creepy";
        public override Color  Color           => new Color(0.15f, 0.0f, 0.2f);
        public override float  Addictiveness   => 0.20f;
        public override int    ValueChange     => 20;
        public override float  ValueMultiplier => 1.1f;

        public override Effect CreateInstance() => UnityEngine.ScriptableObject.CreateInstance<CreepyEffect>();
    }
}