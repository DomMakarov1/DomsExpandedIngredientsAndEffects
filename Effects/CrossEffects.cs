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
    internal static class CrossEffectVisuals
    {
        // Pink/magenta body fire — persistent until container is destroyed (same pattern as InfernoEffect)
        internal static GameObject SpawnPinkFire(Transform root)
        {
            var container = new GameObject("CrossFX_PinkFire");
            container.transform.SetParent(root);
            container.transform.localPosition = new Vector3(0f, 0.1f, 0f);
            container.transform.localRotation = Quaternion.identity;

            var fireGO = new GameObject("PinkFire_Particles");
            fireGO.transform.SetParent(container.transform);
            fireGO.transform.localPosition = Vector3.zero;
            fireGO.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);

            var ps   = fireGO.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.duration        = 999f;
            main.loop            = true;
            main.startLifetime   = 1.2f;
            main.startSpeed      = 0.5f;
            main.startSize       = 0.03f;
            main.startColor      = new Color(1f, 0.2f, 0.6f);
            main.gravityModifier = -0.12f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.rateOverTime = 160f;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle     = 35f;
            shape.radius    = 0.35f;

            var col = ps.colorOverLifetime;
            col.enabled = true;
            var g = new Gradient();
            g.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(new Color(1f,   0.80f, 0.90f), 0f),
                    new GradientColorKey(new Color(1f,   0.20f, 0.60f), 0.4f),
                    new GradientColorKey(new Color(0.6f, 0.05f, 0.30f), 1f)
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(1f,   0f),
                    new GradientAlphaKey(0.8f, 0.5f),
                    new GradientAlphaKey(0f,   1f)
                }
            );
            col.color = new ParticleSystem.MinMaxGradient(g);

            // Warm pink point light
            var lightGO = new GameObject("PinkFire_Light");
            lightGO.transform.SetParent(container.transform);
            lightGO.transform.localPosition = new Vector3(0f, 1.0f, 0f);
            var lt = lightGO.AddComponent<Light>();
            lt.type      = LightType.Point;
            lt.color     = new Color(1f, 0.3f, 0.6f);
            lt.intensity = 2.5f;
            lt.range     = 3.5f;

            FertilizerVisuals.SetMaterial(fireGO);
            ps.Play();
            return container;
        }

        // Brief earthy brown dust flash — screen flashes a muddy brown then fades
        internal static IEnumerator DustFlash()
        {
            var go  = FertilizerVisuals.CreateScreenOverlay(new Color(0.45f, 0.28f, 0.08f, 0f));
            var img = go.GetComponentInChildren<UnityEngine.UI.Image>();

            for (float t = 0f; t < 0.15f; t += Time.deltaTime)
            {
                if (img != null) img.color = new Color(0.45f, 0.28f, 0.08f, Mathf.Clamp01(t / 0.15f) * 0.6f);
                yield return null;
            }
            yield return new WaitForSeconds(0.2f);
            for (float t = 0f; t < 0.5f; t += Time.deltaTime)
            {
                if (img != null) img.color = new Color(0.45f, 0.28f, 0.08f, Mathf.Clamp01(1f - t / 0.5f) * 0.6f);
                yield return null;
            }

            if (go != null) UnityEngine.Object.Destroy(go);
        }
    }

    // ── Cross-ingredient effects ──────────────────────────────────────────────

    // Wildflower — earthyeffect + voluptuouseffect (Fertilizer + Lipstick)
    // Nature's beauty: flower petals + warm green-gold tint
    public class WildflowerEffect : Effect
    {
        private static readonly Dictionary<int, GameObject> _petals   = new Dictionary<int, GameObject>();
        private static readonly Dictionary<int, GameObject> _overlays = new Dictionary<int, GameObject>();

        public override void ApplyToPlayer(Player player)
        {
            if (player == null) return;
            int id = player.transform.GetInstanceID();
            if (_petals.ContainsKey(id)) return;
            try
            {
                _petals[id]   = FertilizerVisuals.SpawnPetals(player.transform);
                _overlays[id] = FertilizerVisuals.CreateScreenOverlay(new Color(0.55f, 0.85f, 0.20f, 0.09f));
            }
            catch (System.Exception ex) { MelonLoader.MelonLogger.Error($"[Wildflower] Apply failed: {ex.Message}"); }
        }

        public override void ClearFromPlayer(Player player)
        {
            if (player == null) return;
            int id = player.transform.GetInstanceID();
            if (_petals.TryGetValue(id,   out var p)) { if (p != null) UnityEngine.Object.Destroy(p); _petals.Remove(id); }
            if (_overlays.TryGetValue(id, out var o)) { if (o != null) UnityEngine.Object.Destroy(o); _overlays.Remove(id); }
        }

        public override void ApplyToNPC(NPC npc)
        {
            if (npc == null) return;
            int id = npc.transform.GetInstanceID();
            if (_petals.ContainsKey(id)) return;
            try { _petals[id] = FertilizerVisuals.SpawnPetals(npc.transform); }
            catch (System.Exception ex) { MelonLoader.MelonLogger.Error($"[Wildflower] NPC apply failed: {ex.Message}"); }
        }

        public override void ClearFromNPC(NPC npc)
        {
            if (npc == null) return;
            int id = npc.transform.GetInstanceID();
            if (_petals.TryGetValue(id, out var p)) { if (p != null) UnityEngine.Object.Destroy(p); _petals.Remove(id); }
        }
    }

    // Burning Passion — spicyeffect + voluptuouseffect (Hot Sauce + Lipstick)
    // Glamorous fire: persistent pink/magenta body flame + pink screen tint
    public class BurningPassionEffect : Effect
    {
        private static readonly Dictionary<int, GameObject> _fire     = new Dictionary<int, GameObject>();
        private static readonly Dictionary<int, GameObject> _overlays = new Dictionary<int, GameObject>();

        public override void ApplyToPlayer(Player player)
        {
            if (player == null) return;
            int id = player.transform.GetInstanceID();
            if (_fire.ContainsKey(id)) return;
            try
            {
                _fire[id]     = CrossEffectVisuals.SpawnPinkFire(player.transform);
                _overlays[id] = FertilizerVisuals.CreateScreenOverlay(new Color(1f, 0.15f, 0.45f, 0.09f));
            }
            catch (System.Exception ex) { MelonLoader.MelonLogger.Error($"[BurningPassion] Apply failed: {ex.Message}"); }
        }

        public override void ClearFromPlayer(Player player)
        {
            if (player == null) return;
            int id = player.transform.GetInstanceID();
            if (_fire.TryGetValue(id,     out var f)) { if (f != null) UnityEngine.Object.Destroy(f); _fire.Remove(id); }
            if (_overlays.TryGetValue(id, out var o)) { if (o != null) UnityEngine.Object.Destroy(o); _overlays.Remove(id); }
        }

        public override void ApplyToNPC(NPC npc)
        {
            if (npc == null) return;
            int id = npc.transform.GetInstanceID();
            if (_fire.ContainsKey(id)) return;
            try { _fire[id] = CrossEffectVisuals.SpawnPinkFire(npc.transform); }
            catch (System.Exception ex) { MelonLoader.MelonLogger.Error($"[BurningPassion] NPC apply failed: {ex.Message}"); }
        }

        public override void ClearFromNPC(NPC npc)
        {
            if (npc == null) return;
            int id = npc.transform.GetInstanceID();
            if (_fire.TryGetValue(id, out var f)) { if (f != null) UnityEngine.Object.Destroy(f); _fire.Remove(id); }
        }
    }

    // Upheaval — earthyeffect + airhornbase (Fertilizer + Airhorn)
    // Earth-shaking: dust flash on apply + persistent muddy earthy tint
    public class UpheavalEffect : Effect
    {
        private static readonly Dictionary<int, GameObject> _overlays = new Dictionary<int, GameObject>();

        public override void ApplyToPlayer(Player player)
        {
            if (player == null) return;
            int id = player.transform.GetInstanceID();
            if (_overlays.ContainsKey(id)) return;
            try
            {
                MelonLoader.MelonCoroutines.Start(CrossEffectVisuals.DustFlash());
                _overlays[id] = FertilizerVisuals.CreateScreenOverlay(new Color(0.40f, 0.25f, 0.07f, 0.12f));
            }
            catch (System.Exception ex) { MelonLoader.MelonLogger.Error($"[Upheaval] Apply failed: {ex.Message}"); }
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
}

namespace DomsExpandedIngredientsAndEffects.Effects.Descriptors
{
    public class WildflowerEffectDef : CustomEffect
    {
        public override string ID              => "wildflower";
        public override string Name            => "Wildflower";
        public override Color  Color           => new Color(0.50f, 0.90f, 0.25f);
        public override float  Addictiveness   => 0.38f;
        public override int    ValueChange     => 18;
        public override float  ValueMultiplier => 1.16f;

        public override Effect CreateInstance() => UnityEngine.ScriptableObject.CreateInstance<WildflowerEffect>();
    }

    public class BurningPassionEffectDef : CustomEffect
    {
        public override string ID              => "burningpassion";
        public override string Name            => "Burning Passion";
        public override Color  Color           => new Color(1.00f, 0.15f, 0.50f);
        public override float  Addictiveness   => 0.52f;
        public override int    ValueChange     => 28;
        public override float  ValueMultiplier => 1.24f;

        public override Effect CreateInstance() => UnityEngine.ScriptableObject.CreateInstance<BurningPassionEffect>();
    }

    public class UpheavalEffectDef : CustomEffect
    {
        public override string ID              => "upheaval";
        public override string Name            => "Upheaval";
        public override Color  Color           => new Color(0.55f, 0.35f, 0.10f);
        public override float  Addictiveness   => 0.42f;
        public override int    ValueChange     => 20;
        public override float  ValueMultiplier => 1.18f;

        public override Effect CreateInstance() => UnityEngine.ScriptableObject.CreateInstance<UpheavalEffect>();
    }
}
