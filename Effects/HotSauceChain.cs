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
    // ── Shared particle helpers ───────────────────────────────────────────────
    internal static class HotSauceVisuals
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
            if (shader == null)
            {
                MelonLoader.MelonLogger.Warning("[HotSauce] Could not find a particle shader.");
                return null;
            }
            _mat = new Material(shader);
            return _mat;
        }

        internal static void SetMaterial(GameObject go)
        {
            var r = go.GetComponent<ParticleSystemRenderer>();
            if (r == null) return;
            var mat = GetMaterial();
            if (mat != null) r.material = mat;
        }

        // Grey smoke cone from mouth (attached to root at mouth height, emits forward)
        // worldForward: pass Camera.main.transform.forward for player, root.forward for NPCs
        // worldPos: explicit world-space spawn point; if null, falls back to root + 1.6 up + 0.15 forward
        internal static GameObject SpawnSmoke(Transform root, float duration, Vector3 worldForward, Vector3? worldPos = null)
        {
            MelonLoader.MelonLogger.Msg($"[HotSauce] SpawnSmoke — root={root.name}, fwd={worldForward}");

            var go = new GameObject("HotSauce_Smoke");
            go.transform.SetParent(root);
            go.transform.position = worldPos ?? (root.position + Vector3.up * 1.6f + worldForward * 0.15f);
            // World-space rotation: emit along worldForward with a gentle upward tilt
            go.transform.rotation = Quaternion.LookRotation(worldForward) * Quaternion.Euler(-10f, 0f, 0f);

            var ps   = go.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.duration        = duration;
            main.loop            = false;
            main.startLifetime   = 2.0f;
            main.startSpeed      = 0.3f;
            main.startSize       = 0.08f;
            main.startColor      = new Color(0.85f, 0.85f, 0.85f, 0.8f);
            main.gravityModifier = -0.08f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            // Inherit emitter velocity so particles move with the character
            var inherit = ps.inheritVelocity;
            inherit.enabled     = true;
            inherit.mode            = ParticleSystemInheritVelocityMode.Current;
            inherit.curveMultiplier = 1.0f;

            var emission = ps.emission;
            emission.rateOverTime = 20f;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle     = 20f;
            shape.radius    = 0.02f;

            var col = ps.colorOverLifetime;
            col.enabled = true;
            var g = new Gradient();
            g.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(new Color(0.80f, 0.80f, 0.80f), 0f),
                    new GradientColorKey(new Color(0.95f, 0.95f, 0.95f), 1f)
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(0.8f, 0f),
                    new GradientAlphaKey(0f,   1f)
                }
            );
            col.color = new ParticleSystem.MinMaxGradient(g);

            SetMaterial(go);
            ps.Play();
            MelonLoader.MelonLogger.Msg($"[HotSauce] Smoke spawned, isPlaying={ps.isPlaying}, worldPos={go.transform.position}");
            return go;
        }

        // Orange/red fire jet from mouth (attached to root at mouth height, emits forward)
        // worldForward: pass Camera.main.transform.forward for player, root.forward for NPCs
        internal static GameObject SpawnFireBreath(Transform root, float duration, Vector3 worldForward, Vector3? worldPos = null)
        {
            MelonLoader.MelonLogger.Msg($"[HotSauce] SpawnFireBreath — root={root.name}, fwd={worldForward}");

            var go = new GameObject("HotSauce_FireBreath");
            go.transform.SetParent(root);
            go.transform.position = worldPos ?? (root.position + Vector3.up * 1.6f + worldForward * 0.15f);
            go.transform.rotation = Quaternion.LookRotation(worldForward) * Quaternion.Euler(-10f, 0f, 0f);

            var ps   = go.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.duration        = duration;
            main.loop            = false;
            main.startLifetime   = 0.8f;
            main.startSpeed      = 1.2f;
            main.startSize       = 0.05f;
            main.startColor      = new Color(1f, 0.55f, 0f);
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            // Inherit emitter velocity so particles move with the character
            var inherit = ps.inheritVelocity;
            inherit.enabled    = true;
            inherit.mode            = ParticleSystemInheritVelocityMode.Current;
            inherit.curveMultiplier = 1.0f;

            var emission = ps.emission;
            emission.rateOverTime = 45f;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle     = 12f;
            shape.radius    = 0.02f;

            var col = ps.colorOverLifetime;
            col.enabled = true;
            var g = new Gradient();
            g.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(new Color(1f,   1f,   0f),  0f),
                    new GradientColorKey(new Color(1f,   0.4f, 0f),  0.4f),
                    new GradientColorKey(new Color(0.8f, 0.1f, 0f),  1f)
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(1f,   0f),
                    new GradientAlphaKey(0.8f, 0.5f),
                    new GradientAlphaKey(0f,   1f)
                }
            );
            col.color = new ParticleSystem.MinMaxGradient(g);

            SetMaterial(go);
            ps.Play();
            MelonLoader.MelonLogger.Msg($"[HotSauce] FireBreath spawned, isPlaying={ps.isPlaying}, worldPos={go.transform.position}");
            return go;
        }

        // Full-body fire + smoke + glow — loops until the returned container is destroyed.
        internal static GameObject SpawnBodyFire(Transform root)
        {
            MelonLoader.MelonLogger.Msg($"[HotSauce] SpawnBodyFire — root={root.name}");

            var container = new GameObject("HotSauce_BodyEffect");
            container.transform.SetParent(root);
            container.transform.localPosition = new Vector3(0f, 0.1f, 0f);
            container.transform.localRotation = Quaternion.identity;

            // ── Fire particles ─────────────────────────────────────────────────
            var fireGO = new GameObject("HotSauce_BodyFire");
            fireGO.transform.SetParent(container.transform);
            fireGO.transform.localPosition = Vector3.zero;
            fireGO.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);

            var ps   = fireGO.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.duration        = 999f;
            main.loop            = true;
            main.startLifetime   = 1.4f;   // +0.5s
            main.startSpeed      = 0.36f;
            main.startSize       = 0.032f; // +15%
            main.startColor      = new Color(1f, 0.5f, 0f);
            main.gravityModifier = -0.15f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.rateOverTime = 330f;  // ×1.5

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle     = 35f;
            shape.radius    = 0.35f;

            var col = ps.colorOverLifetime;
            col.enabled = true;
            var fg = new Gradient();
            fg.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(new Color(1f,   1f,   0.2f), 0f),
                    new GradientColorKey(new Color(1f,   0.4f, 0f),   0.4f),
                    new GradientColorKey(new Color(0.6f, 0.1f, 0f),   1f)
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(1f,   0f),
                    new GradientAlphaKey(0.8f, 0.5f),
                    new GradientAlphaKey(0f,   1f)
                }
            );
            col.color = new ParticleSystem.MinMaxGradient(fg);
            SetMaterial(fireGO);
            ps.Play();

            // ── Smoke particles (same settings as fire, dark colour) ───────────
            var smokeGO = new GameObject("HotSauce_BodySmoke");
            smokeGO.transform.SetParent(container.transform);
            smokeGO.transform.localPosition = Vector3.zero;
            smokeGO.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);

            var sps   = smokeGO.AddComponent<ParticleSystem>();
            var smain = sps.main;
            smain.duration        = 999f;
            smain.loop            = true;
            smain.startLifetime   = 0.9f;   // matches fire
            smain.startSpeed      = 0.36f;  // matches fire
            smain.startSize       = 0.028f; // matches fire
            smain.startColor      = new Color(0.2f, 0.2f, 0.2f, 0.5f);
            smain.gravityModifier = -0.04f;
            smain.simulationSpace = ParticleSystemSimulationSpace.World;

            var semission = sps.emission;
            semission.rateOverTime = 220f;  // matches fire

            var sshape = sps.shape;
            sshape.shapeType = ParticleSystemShapeType.Cone;
            sshape.angle     = 35f;   // matches fire
            sshape.radius    = 0.35f; // matches fire

            var scol = sps.colorOverLifetime;
            scol.enabled = true;
            var sg = new Gradient();
            sg.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(new Color(0.35f, 0.30f, 0.25f), 0f),
                    new GradientColorKey(new Color(0.15f, 0.15f, 0.15f), 1f)
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(0.45f, 0f),
                    new GradientAlphaKey(0f,    1f)
                }
            );
            scol.color = new ParticleSystem.MinMaxGradient(sg);
            SetMaterial(smokeGO);
            sps.Play();

            // ── Point light (warm orange glow) ─────────────────────────────────
            var lightGO = new GameObject("HotSauce_FireLight");
            lightGO.transform.SetParent(container.transform);
            lightGO.transform.localPosition = new Vector3(0f, 1.0f, 0f);
            var lt = lightGO.AddComponent<Light>();
            lt.type      = LightType.Point;
            lt.color     = new Color(1f, 0.45f, 0.1f);
            lt.intensity = 3f;
            lt.range     = 4f;

            MelonLoader.MelonLogger.Msg($"[HotSauce] BodyFire container spawned");
            return container;
        }
    }

    // ── Effect runtime classes ────────────────────────────────────────────────

    public class FiveAlarmEffect : Effect
    {
        public override void ApplyToNPC(NPC npc) { }
        public override void ClearFromNPC(NPC npc) { }
        public override void ApplyToPlayer(Player player) { }
        public override void ClearFromPlayer(Player player) { }
    }

    public class CapsaicinRushEffect : Effect
    {
        private const float Duration = 4f;

        public override void ApplyToPlayer(Player player)
        {
            if (player == null) return;
            Vector3 fwd = Camera.main != null ? Camera.main.transform.forward : player.transform.forward;
            // Mouth position: slightly below and in front of the camera
            Vector3 pos = (Camera.main != null ? Camera.main.transform.position : player.transform.position + Vector3.up * 1.6f)
                          - Vector3.up * 0.2f + fwd * 0.25f;
            MelonLoader.MelonLogger.Msg($"[CapsaicinRush] ApplyToPlayer — fwd={fwd}");
            MelonLoader.MelonCoroutines.Start(TimedSmoke(player.transform, fwd, pos));
        }
        public override void ClearFromPlayer(Player player) { }

        public override void ApplyToNPC(NPC npc)
        {
            if (npc == null) return;
            MelonLoader.MelonLogger.Msg($"[CapsaicinRush] ApplyToNPC — transform={npc.transform?.name ?? "null"}");
            MelonLoader.MelonCoroutines.Start(TimedSmoke(npc.transform, npc.transform.forward));
        }
        public override void ClearFromNPC(NPC npc) { }

        private static IEnumerator TimedSmoke(Transform root, Vector3 worldForward, Vector3? worldPos = null)
        {
            if (root == null) { MelonLoader.MelonLogger.Warning("[CapsaicinRush] root is null, aborting"); yield break; }
            GameObject go = null;
            try   { go = HotSauceVisuals.SpawnSmoke(root, Duration, worldForward, worldPos); }
            catch (System.Exception ex) { MelonLoader.MelonLogger.Error($"[CapsaicinRush] Spawn failed: {ex.Message}\n{ex.StackTrace}"); }

            if (go != null)
            {
                yield return new WaitForSeconds(Duration + 2f);
                if (go != null) UnityEngine.Object.Destroy(go);
            }
        }
    }

    public class DragonBreathEffect : Effect
    {
        private const float Duration = 5f;

        public override void ApplyToPlayer(Player player)
        {
            if (player == null) return;
            Vector3 fwd = Camera.main != null ? Camera.main.transform.forward : player.transform.forward;
            Vector3 pos = (Camera.main != null ? Camera.main.transform.position : player.transform.position + Vector3.up * 1.6f)
                          - Vector3.up * 0.2f + fwd * 0.25f;
            MelonLoader.MelonLogger.Msg($"[DragonBreath] ApplyToPlayer — fwd={fwd}");
            MelonLoader.MelonCoroutines.Start(TimedFire(player.transform, fwd, pos));
        }
        public override void ClearFromPlayer(Player player) { }

        public override void ApplyToNPC(NPC npc)
        {
            if (npc == null) return;
            MelonLoader.MelonLogger.Msg($"[DragonBreath] ApplyToNPC — transform={npc.transform?.name ?? "null"}");
            MelonLoader.MelonCoroutines.Start(TimedFire(npc.transform, npc.transform.forward));
        }
        public override void ClearFromNPC(NPC npc) { }

        private static IEnumerator TimedFire(Transform root, Vector3 worldForward, Vector3? worldPos = null)
        {
            if (root == null) { MelonLoader.MelonLogger.Warning("[DragonBreath] root is null, aborting"); yield break; }
            GameObject go = null;
            try   { go = HotSauceVisuals.SpawnFireBreath(root, Duration, worldForward, worldPos); }
            catch (System.Exception ex) { MelonLoader.MelonLogger.Error($"[DragonBreath] Spawn failed: {ex.Message}\n{ex.StackTrace}"); }

            if (go != null)
            {
                yield return new WaitForSeconds(Duration + 1f);
                if (go != null) UnityEngine.Object.Destroy(go);
            }
        }
    }

    public class InfernoEffect : Effect
    {
        private static readonly Dictionary<int, GameObject> _active = new Dictionary<int, GameObject>();

        public override void ApplyToPlayer(Player player)
        {
            if (player == null) return;
            int id = player.transform.GetInstanceID();
            if (_active.ContainsKey(id)) return;
            MelonLoader.MelonLogger.Msg($"[Inferno] ApplyToPlayer id={id}");
            try   { _active[id] = HotSauceVisuals.SpawnBodyFire(player.transform); }
            catch (System.Exception ex) { MelonLoader.MelonLogger.Error($"[Inferno] Spawn failed: {ex.Message}\n{ex.StackTrace}"); }
        }

        public override void ClearFromPlayer(Player player)
        {
            if (player == null) return;
            int id = player.transform.GetInstanceID();
            if (_active.TryGetValue(id, out var go))
            {
                if (go != null) UnityEngine.Object.Destroy(go);
                _active.Remove(id);
            }
        }

        public override void ApplyToNPC(NPC npc)
        {
            if (npc == null) return;
            int id = npc.transform.GetInstanceID();
            if (_active.ContainsKey(id)) return;
            MelonLoader.MelonLogger.Msg($"[Inferno] ApplyToNPC id={id}");
            try   { _active[id] = HotSauceVisuals.SpawnBodyFire(npc.transform); }
            catch (System.Exception ex) { MelonLoader.MelonLogger.Error($"[Inferno] Spawn failed: {ex.Message}\n{ex.StackTrace}"); }
        }

        public override void ClearFromNPC(NPC npc)
        {
            if (npc == null) return;
            int id = npc.transform.GetInstanceID();
            if (_active.TryGetValue(id, out var go))
            {
                if (go != null) UnityEngine.Object.Destroy(go);
                _active.Remove(id);
            }
        }
    }
}

namespace DomsExpandedIngredientsAndEffects.Effects.Descriptors
{
    public class FiveAlarmEffectDef : CustomEffect
    {
        public override string ID              => "fivealarm";
        public override string Name            => "Five Alarm";
        public override Color  Color           => new Color(0.85f, 0.05f, 0.05f);
        public override float  Addictiveness   => 0.35f;
        public override int    ValueChange     => 18;
        public override float  ValueMultiplier => 1.15f;

        public override Effect CreateInstance() => UnityEngine.ScriptableObject.CreateInstance<FiveAlarmEffect>();
    }

    public class CapsaicinRushEffectDef : CustomEffect
    {
        public override string ID              => "capsaicinrush";
        public override string Name            => "Capsaicin Rush";
        public override Color  Color           => new Color(1.00f, 0.00f, 0.50f);
        public override float  Addictiveness   => 0.55f;
        public override int    ValueChange     => 27;
        public override float  ValueMultiplier => 1.20f;

        public override Effect CreateInstance() => UnityEngine.ScriptableObject.CreateInstance<CapsaicinRushEffect>();
    }

    public class DragonBreathEffectDef : CustomEffect
    {
        public override string ID              => "dragonbreath";
        public override string Name            => "Dragon Breath";
        public override Color  Color           => new Color(1.00f, 0.50f, 0.00f);
        public override float  Addictiveness   => 0.75f;
        public override int    ValueChange     => 36;
        public override float  ValueMultiplier => 1.30f;

        public override Effect CreateInstance() => UnityEngine.ScriptableObject.CreateInstance<DragonBreathEffect>();
    }

    public class InfernoEffectDef : CustomEffect
    {
        public override string ID              => "inferno";
        public override string Name            => "Inferno";
        public override Color  Color           => new Color(1.00f, 0.90f, 0.00f);
        public override float  Addictiveness   => 1.0f;
        public override int    ValueChange     => 55;
        public override float  ValueMultiplier => 1.40f;

        public override Effect CreateInstance() => UnityEngine.ScriptableObject.CreateInstance<InfernoEffect>();
    }
}
