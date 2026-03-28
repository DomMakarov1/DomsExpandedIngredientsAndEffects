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
    public class TrollEffect : Effect
    {
        private static AudioClip _knockClip;

        public override void ApplyToPlayer(Player player)
        {
            if (player == null || !player.IsOwner) return;
            MelonLoader.MelonCoroutines.Start(KnockDelay(player.transform.position));
        }

        public override void ClearFromPlayer(Player player) { }

        public override void ApplyToNPC(NPC npc)
        {
            if (npc == null) return;
            MelonLoader.MelonCoroutines.Start(KnockDelay(npc.transform.position));
        }

        public override void ClearFromNPC(NPC npc) { }

        private static IEnumerator KnockDelay(Vector3 position)
        {
            yield return new WaitForSeconds(300f);

            if (_knockClip == null)
                _knockClip = DomsCustomEffects.LoadCustomSound("Knock.wav");

            if (_knockClip == null)
            {
                MelonLoader.MelonLogger.Warning("[Troll] Knock.wav not found in Sounds folder.");
                yield break;
            }

            AudioSource.PlayClipAtPoint(_knockClip, position, SoundHelper.SFXVolume);
        }
    }
}

namespace DomsExpandedIngredientsAndEffects.Effects.Descriptors
{
    public class TrollEffectDef : CustomEffect
    {
        public override string ID              => "trolleffect";
        public override string Name            => "Troll";
        public override Color  Color           => new Color(0.2f, 0.8f, 0.2f);
        public override float  Addictiveness   => 0.10f;
        public override int    ValueChange     => 8;
        public override float  ValueMultiplier => 1.05f;

        public override Effect CreateInstance() => UnityEngine.ScriptableObject.CreateInstance<TrollEffect>();
    }
}