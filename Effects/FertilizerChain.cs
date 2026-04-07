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
    // ── Shared visual helpers ─────────────────────────────────────────────────
    internal static class FertilizerVisuals
    {
        private static Material _mat;

        internal static Material GetMaterial()
        {
            if (_mat != null) return _mat;
            var shader = Shader.Find("Universal Render Pipeline/Particles/Unlit")
                      ?? Shader.Find("Universal Render Pipeline/Particles/Lit")
                      ?? Shader.Find("Sprites/Default")
                      ?? Shader.Find("Universal Render Pipeline/Lit")
                      ?? Shader.Find("Standard");
            if (shader != null) _mat = new Material(shader);
            return _mat;
        }

        internal static void SetMaterial(GameObject go)
        {
            var r = go.GetComponent<ParticleSystemRenderer>();
            if (r == null) return;
            var mat = GetMaterial();
            if (mat != null) r.material = mat;
        }

        // Full-screen colour overlay via a ScreenSpaceOverlay Canvas
        internal static GameObject CreateScreenOverlay(Color color)
        {
            var canvasGO = new GameObject("FertEffect_Canvas");
            UnityEngine.Object.DontDestroyOnLoad(canvasGO);
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 50;
            canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();

            var imgGO = new GameObject("FertEffect_Img");
            imgGO.transform.SetParent(canvasGO.transform, false);
            var img = imgGO.AddComponent<UnityEngine.UI.Image>();
            img.color         = color;
            img.raycastTarget = false;
            var rect = imgGO.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return canvasGO;
        }

        // Compost unique: fade to black, hold, fade back — overwhelming sensory moment
        internal static IEnumerator BlackoutFlash()
        {
            var go  = CreateScreenOverlay(new Color(0f, 0f, 0f, 0f));
            var img = go.GetComponentInChildren<UnityEngine.UI.Image>();

            for (float t = 0f; t < 0.4f; t += Time.deltaTime)
            {
                if (img != null) img.color = new Color(0f, 0f, 0f, Mathf.Clamp01(t / 0.4f));
                yield return null;
            }
            if (img != null) img.color = Color.black;

            yield return new WaitForSeconds(0.8f);

            for (float t = 0f; t < 0.6f; t += Time.deltaTime)
            {
                if (img != null) img.color = new Color(0f, 0f, 0f, Mathf.Clamp01(1f - t / 0.6f));
                yield return null;
            }

            if (go != null) UnityEngine.Object.Destroy(go);
        }

        // Bloom: pink/white flower petals floating upward from character — persistent
        internal static GameObject SpawnPetals(Transform root)
        {
            var go = new GameObject("Bloom_Petals");
            go.transform.SetParent(root);
            go.transform.localPosition = new Vector3(0f, 0.3f, 0f);
            go.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f); // cone opens upward

            var ps   = go.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.duration        = 999f;
            main.loop            = true;
            main.startLifetime   = 2.5f;
            main.startSpeed      = 0.4f;
            main.startSize       = 0.05f;
            main.startColor      = new Color(1f, 0.7f, 0.85f);
            main.gravityModifier = -0.06f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.rateOverTime = 18f;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle     = 55f;
            shape.radius    = 0.4f;

            var col = ps.colorOverLifetime;
            col.enabled = true;
            var g = new Gradient();
            g.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(new Color(1f,   0.60f, 0.85f), 0f),
                    new GradientColorKey(new Color(1f,   0.95f, 0.70f), 0.5f),
                    new GradientColorKey(new Color(1f,   1f,   1f),    1f)
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(0.9f, 0f),
                    new GradientAlphaKey(0.6f, 0.6f),
                    new GradientAlphaKey(0f,   1f)
                }
            );
            col.color = new ParticleSystem.MinMaxGradient(g);

            SetMaterial(go);
            ps.Play();
            return go;
        }
    }

    // ── Effect runtime classes ────────────────────────────────────────────────

    // Depth 2 — Earthy + calming — stats only
    public class RootBoundEffect : Effect
    {
        public override void ApplyToNPC(NPC npc) { }
        public override void ClearFromNPC(NPC npc) { }
        public override void ApplyToPlayer(Player player) { }
        public override void ClearFromPlayer(Player player) { }
    }

    // Depth 3 — Root Bound + toxic — green screen tint (player) + NPC movement slow
    public class OvergrownEffect : Effect
    {
        private static readonly Dictionary<int, GameObject> _overlays  = new Dictionary<int, GameObject>();
        private static readonly Dictionary<int, float>      _npcSpeeds = new Dictionary<int, float>();

        public override void ApplyToPlayer(Player player)
        {
            if (player == null) return;
            int id = player.transform.GetInstanceID();
            if (_overlays.ContainsKey(id)) return;
            try   { _overlays[id] = FertilizerVisuals.CreateScreenOverlay(new Color(0f, 0.35f, 0f, 0.15f)); }
            catch (System.Exception ex) { MelonLoader.MelonLogger.Error($"[Overgrown] Overlay failed: {ex.Message}"); }
        }

        public override void ClearFromPlayer(Player player)
        {
            if (player == null) return;
            int id = player.transform.GetInstanceID();
            if (_overlays.TryGetValue(id, out var go))
            {
                if (go != null) UnityEngine.Object.Destroy(go);
                _overlays.Remove(id);
            }
        }

        public override void ApplyToNPC(NPC npc)
        {
            if (npc == null) return;
            int id = npc.transform.GetInstanceID();
            try
            {
                var nav = npc.GetComponent<UnityEngine.AI.NavMeshAgent>();
                if (nav != null) { _npcSpeeds[id] = nav.speed; nav.speed *= 0.5f; }
            }
            catch (System.Exception ex) { MelonLoader.MelonLogger.Error($"[Overgrown] NPC slow failed: {ex.Message}"); }
        }

        public override void ClearFromNPC(NPC npc)
        {
            if (npc == null) return;
            int id = npc.transform.GetInstanceID();
            try
            {
                var nav = npc.GetComponent<UnityEngine.AI.NavMeshAgent>();
                if (nav != null && _npcSpeeds.TryGetValue(id, out float s))
                {
                    nav.speed = s;
                    _npcSpeeds.Remove(id);
                }
            }
            catch (System.Exception ex) { MelonLoader.MelonLogger.Error($"[Overgrown] NPC restore failed: {ex.Message}"); }
        }
    }

    // Depth 4 — Overgrown + energizing — flower petals + warm golden tint (player + NPC)
    public class BloomEffect : Effect
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
                _overlays[id] = FertilizerVisuals.CreateScreenOverlay(new Color(1f, 0.85f, 0f, 0.08f));
            }
            catch (System.Exception ex) { MelonLoader.MelonLogger.Error($"[Bloom] Apply failed: {ex.Message}"); }
        }

        public override void ClearFromPlayer(Player player)
        {
            if (player == null) return;
            int id = player.transform.GetInstanceID();
            if (_petals.TryGetValue(id,   out var petals))  { if (petals   != null) UnityEngine.Object.Destroy(petals);  _petals.Remove(id); }
            if (_overlays.TryGetValue(id, out var overlay)) { if (overlay  != null) UnityEngine.Object.Destroy(overlay); _overlays.Remove(id); }
        }

        public override void ApplyToNPC(NPC npc)
        {
            if (npc == null) return;
            int id = npc.transform.GetInstanceID();
            if (_petals.ContainsKey(id)) return;
            try   { _petals[id] = FertilizerVisuals.SpawnPetals(npc.transform); }
            catch (System.Exception ex) { MelonLoader.MelonLogger.Error($"[Bloom] NPC petals failed: {ex.Message}"); }
        }

        public override void ClearFromNPC(NPC npc)
        {
            if (npc == null) return;
            int id = npc.transform.GetInstanceID();
            if (_petals.TryGetValue(id, out var petals))
            {
                if (petals != null) UnityEngine.Object.Destroy(petals);
                _petals.Remove(id);
            }
        }
    }

    // Depth 5 — Bloom + foggy
    // Player: brief blackout flash on apply + persistent dark tint
    // NPC: near-stop confused shuffle for ~4s
    public class CompostEffect : Effect
    {
        private static readonly Dictionary<int, GameObject> _overlays = new Dictionary<int, GameObject>();

        public override void ApplyToPlayer(Player player)
        {
            if (player == null) return;
            int id = player.transform.GetInstanceID();
            if (_overlays.ContainsKey(id)) return;
            try
            {
                MelonLoader.MelonCoroutines.Start(FertilizerVisuals.BlackoutFlash());
                _overlays[id] = FertilizerVisuals.CreateScreenOverlay(new Color(0f, 0f, 0f, 0.18f));
            }
            catch (System.Exception ex) { MelonLoader.MelonLogger.Error($"[Compost] Apply failed: {ex.Message}"); }
        }

        public override void ClearFromPlayer(Player player)
        {
            if (player == null) return;
            int id = player.transform.GetInstanceID();
            if (_overlays.TryGetValue(id, out var go))
            {
                if (go != null) UnityEngine.Object.Destroy(go);
                _overlays.Remove(id);
            }
        }

        public override void ApplyToNPC(NPC npc)
        {
            if (npc == null) return;
            try
            {
                var nav = npc.GetComponent<UnityEngine.AI.NavMeshAgent>();
                if (nav != null) MelonLoader.MelonCoroutines.Start(ConfusedShuffle(nav));
            }
            catch (System.Exception ex) { MelonLoader.MelonLogger.Error($"[Compost] NPC confused failed: {ex.Message}"); }
        }

        public override void ClearFromNPC(NPC npc) { }

        // NPC slows to a near-stop for 4 seconds then recovers
        private static IEnumerator ConfusedShuffle(UnityEngine.AI.NavMeshAgent nav)
        {
            if (nav == null) yield break;
            float originalSpeed = nav.speed;
            nav.speed = 0.25f;
            yield return new WaitForSeconds(4f);
            if (nav != null) nav.speed = originalSpeed;
        }
    }
}

namespace DomsExpandedIngredientsAndEffects.Effects.Descriptors
{
    public class RootBoundEffectDef : CustomEffect
    {
        public override string ID              => "rootbound";
        public override string Name            => "Root Bound";
        public override Color  Color           => new Color(0.30f, 0.60f, 0.20f);
        public override float  Addictiveness   => 0.23f;
        public override int    ValueChange     => 13;
        public override float  ValueMultiplier => 1.08f;

        public override Effect CreateInstance() => UnityEngine.ScriptableObject.CreateInstance<RootBoundEffect>();
    }

    public class OvergrownEffectDef : CustomEffect
    {
        public override string ID              => "overgrown";
        public override string Name            => "Overgrown";
        public override Color  Color           => new Color(0.10f, 0.80f, 0.30f);
        public override float  Addictiveness   => 0.4f;
        public override int    ValueChange     => 20;
        public override float  ValueMultiplier => 1.14f;

        public override Effect CreateInstance() => UnityEngine.ScriptableObject.CreateInstance<OvergrownEffect>();
    }

    public class BloomEffectDef : CustomEffect
    {
        public override string ID              => "bloom";
        public override string Name            => "Bloom";
        public override Color  Color           => new Color(1.00f, 0.85f, 0.10f);
        public override float  Addictiveness   => 0.68f;
        public override int    ValueChange     => 30;
        public override float  ValueMultiplier => 1.25f;

        public override Effect CreateInstance() => UnityEngine.ScriptableObject.CreateInstance<BloomEffect>();
    }

    public class CompostEffectDef : CustomEffect
    {
        public override string ID              => "compost";
        public override string Name            => "Compost";
        public override Color  Color           => new Color(0.60f, 0.25f, 0.55f);
        public override float  Addictiveness   => 1.0f;
        public override int    ValueChange     => 38;
        public override float  ValueMultiplier => 1.38f;

        public override Effect CreateInstance() => UnityEngine.ScriptableObject.CreateInstance<CompostEffect>();
    }
}
