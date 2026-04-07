using UnityEngine;
using System.Collections;
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
    // Phantasm — magicbase + toxic
    // Special magic juice effect: no stat benefit. On apply: blinding white flash.
    // Persistent: ghostly pale-blue shimmer particles drift up from the character's body.
    public class PhantasmEffect : Effect
    {
        public override void ApplyToPlayer(Player player)
        {
            if (player == null) return;
            try { MelonLoader.MelonCoroutines.Start(WhiteFlash()); }
            catch (System.Exception ex) { MelonLoader.MelonLogger.Error($"[Phantasm] Apply failed: {ex.Message}"); }
        }

        public override void ClearFromPlayer(Player player) { }

        public override void ApplyToNPC(NPC npc) { }
        public override void ClearFromNPC(NPC npc) { }

        // Screen snaps to pure white then fades back — the "phantasm" hitting the senses
        static IEnumerator WhiteFlash()
        {
            var go  = FertilizerVisuals.CreateScreenOverlay(new Color(1f, 1f, 1f, 0f));
            var img = go.GetComponentInChildren<UnityEngine.UI.Image>();

            for (float t = 0f; t < 0.12f; t += Time.deltaTime)
            {
                if (img != null) img.color = new Color(1f, 1f, 1f, Mathf.Clamp01(t / 0.12f));
                yield return null;
            }
            if (img != null) img.color = Color.white;
            yield return new WaitForSeconds(0.15f);
            for (float t = 0f; t < 0.7f; t += Time.deltaTime)
            {
                if (img != null) img.color = new Color(1f, 1f, 1f, Mathf.Clamp01(1f - t / 0.7f));
                yield return null;
            }

            if (go != null) UnityEngine.Object.Destroy(go);
        }
    }
}

namespace DomsExpandedIngredientsAndEffects.Effects.Descriptors
{
    // Magic juice effects intentionally have no stat benefits —
    // the value is in the special visual/gameplay experience
    public class PhantasmEffectDef : CustomEffect
    {
        public override string ID              => "phantasm";
        public override string Name            => "Phantasm";
        public override Color  Color           => new Color(0.70f, 0.88f, 1.00f);
        public override float  Addictiveness   => 0.0f;
        public override int    ValueChange     => 0;
        public override float  ValueMultiplier => 1.0f;

        public override Effect CreateInstance() => UnityEngine.ScriptableObject.CreateInstance<PhantasmEffect>();
    }
}
