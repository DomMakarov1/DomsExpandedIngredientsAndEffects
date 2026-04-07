using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DomsExpandedIngredientsAndEffects.Framework;

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
    internal static class AirhornVisuals
    {
        // Brief bright blue-white flash — screen snaps to near-white then fades over ~0.5s
        internal static IEnumerator ThunderFlash()
        {
            var go  = FertilizerVisuals.CreateScreenOverlay(new Color(0.75f, 0.90f, 1f, 0f));
            var img = go.GetComponentInChildren<UnityEngine.UI.Image>();

            // Snap in
            for (float t = 0f; t < 0.08f; t += Time.deltaTime)
            {
                if (img != null) img.color = new Color(0.75f, 0.90f, 1f, Mathf.Clamp01(t / 0.08f));
                yield return null;
            }
            if (img != null) img.color = new Color(0.75f, 0.90f, 1f, 1f);
            yield return new WaitForSeconds(0.05f);

            // Fade out
            for (float t = 0f; t < 0.45f; t += Time.deltaTime)
            {
                if (img != null) img.color = new Color(0.75f, 0.90f, 1f, Mathf.Clamp01(1f - t / 0.45f));
                yield return null;
            }

            if (go != null) UnityEngine.Object.Destroy(go);
        }
    }

    // Depth — airhornbase + electrifying
    // On apply: blinding blue-white screen flash. Persistent: faint electric-blue tint.
    public class ThunderclapEffect : Effect
    {
        private static readonly Dictionary<int, GameObject> _overlays = new Dictionary<int, GameObject>();

        public override void ApplyToPlayer(Player player)
        {
            if (player == null) return;
            int id = player.transform.GetInstanceID();
            if (_overlays.ContainsKey(id)) return;
            try
            {
                MelonLoader.MelonCoroutines.Start(AirhornVisuals.ThunderFlash());
                _overlays[id] = FertilizerVisuals.CreateScreenOverlay(new Color(0.10f, 0.25f, 0.85f, 0.08f));
            }
            catch (System.Exception ex) { MelonLoader.MelonLogger.Error($"[Thunderclap] Apply failed: {ex.Message}"); }
        }

        public override void ClearFromPlayer(Player player)
        {
            if (player == null) return;
            int id = player.transform.GetInstanceID();
            if (_overlays.TryGetValue(id, out var o)) { if (o != null) UnityEngine.Object.Destroy(o); _overlays.Remove(id); }
        }

        public override void ApplyToNPC(NPC npc) { }
        public override void ClearFromNPC(NPC npc) { }
    }

    // Depth — fiestaeffect + sedating
    // Heavy grey desaturation overlay + NPC movement slowed (like they're wading through static)
    public class WhiteNoiseEffect : Effect
    {
        private static readonly Dictionary<int, GameObject> _overlays  = new Dictionary<int, GameObject>();
        private static readonly Dictionary<int, float>      _npcSpeeds = new Dictionary<int, float>();

        public override void ApplyToPlayer(Player player)
        {
            if (player == null) return;
            int id = player.transform.GetInstanceID();
            if (_overlays.ContainsKey(id)) return;
            try { _overlays[id] = FertilizerVisuals.CreateScreenOverlay(new Color(0.55f, 0.55f, 0.55f, 0.32f)); }
            catch (System.Exception ex) { MelonLoader.MelonLogger.Error($"[WhiteNoise] Apply failed: {ex.Message}"); }
        }

        public override void ClearFromPlayer(Player player)
        {
            if (player == null) return;
            int id = player.transform.GetInstanceID();
            if (_overlays.TryGetValue(id, out var o)) { if (o != null) UnityEngine.Object.Destroy(o); _overlays.Remove(id); }
        }

        public override void ApplyToNPC(NPC npc)
        {
            if (npc == null) return;
            int id = npc.transform.GetInstanceID();
            try
            {
                var nav = npc.GetComponent<UnityEngine.AI.NavMeshAgent>();
                if (nav != null) { _npcSpeeds[id] = nav.speed; nav.speed *= 0.45f; }
            }
            catch (System.Exception ex) { MelonLoader.MelonLogger.Error($"[WhiteNoise] NPC slow failed: {ex.Message}"); }
        }

        public override void ClearFromNPC(NPC npc)
        {
            if (npc == null) return;
            int id = npc.transform.GetInstanceID();
            try
            {
                var nav = npc.GetComponent<UnityEngine.AI.NavMeshAgent>();
                if (nav != null && _npcSpeeds.TryGetValue(id, out float s)) { nav.speed = s; _npcSpeeds.Remove(id); }
            }
            catch (System.Exception ex) { MelonLoader.MelonLogger.Error($"[WhiteNoise] NPC restore failed: {ex.Message}"); }
        }
    }
}

namespace DomsExpandedIngredientsAndEffects.Effects.Descriptors
{
    public class ThunderclapEffectDef : CustomEffect
    {
        public override string ID              => "thunderclap";
        public override string Name            => "Thunderclap";
        public override Color  Color           => new Color(0.20f, 0.50f, 1.00f);
        public override float  Addictiveness   => 0.40f;
        public override int    ValueChange     => 20;
        public override float  ValueMultiplier => 1.20f;

        public override Effect CreateInstance() => UnityEngine.ScriptableObject.CreateInstance<ThunderclapEffect>();
    }

    public class WhiteNoiseEffectDef : CustomEffect
    {
        public override string ID              => "whitenoise";
        public override string Name            => "White Noise";
        public override Color  Color           => new Color(0.80f, 0.80f, 0.85f);
        public override float  Addictiveness   => 0.50f;
        public override int    ValueChange     => 30;
        public override float  ValueMultiplier => 1.35f;

        public override Effect CreateInstance() => UnityEngine.ScriptableObject.CreateInstance<WhiteNoiseEffect>();
    }
}
