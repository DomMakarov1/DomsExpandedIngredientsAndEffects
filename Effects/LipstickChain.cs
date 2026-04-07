using UnityEngine;
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
    // Depth — voluptuouseffect + foggy
    // Glittery pink/gold sparkles swirl around the character + soft pink screen tint
    public class FemmeFataleEffect : Effect
    {
        private static readonly Dictionary<int, GameObject> _sparkles = new Dictionary<int, GameObject>();
        private static readonly Dictionary<int, GameObject> _overlays = new Dictionary<int, GameObject>();

        public override void ApplyToPlayer(Player player)
        {
            if (player == null) return;
            int id = player.transform.GetInstanceID();
            if (_sparkles.ContainsKey(id)) return;
            try
            {
                _sparkles[id] = SpawnSparkles(player.transform);
                _overlays[id] = FertilizerVisuals.CreateScreenOverlay(new Color(1f, 0.55f, 0.75f, 0.10f));
            }
            catch (System.Exception ex) { MelonLoader.MelonLogger.Error($"[FemmeFatale] Apply failed: {ex.Message}"); }
        }

        public override void ClearFromPlayer(Player player)
        {
            if (player == null) return;
            int id = player.transform.GetInstanceID();
            if (_sparkles.TryGetValue(id, out var s)) { if (s != null) UnityEngine.Object.Destroy(s); _sparkles.Remove(id); }
            if (_overlays.TryGetValue(id, out var o)) { if (o != null) UnityEngine.Object.Destroy(o); _overlays.Remove(id); }
        }

        public override void ApplyToNPC(NPC npc)
        {
            if (npc == null) return;
            int id = npc.transform.GetInstanceID();
            if (_sparkles.ContainsKey(id)) return;
            try { _sparkles[id] = SpawnSparkles(npc.transform); }
            catch (System.Exception ex) { MelonLoader.MelonLogger.Error($"[FemmeFatale] NPC apply failed: {ex.Message}"); }
        }

        public override void ClearFromNPC(NPC npc)
        {
            if (npc == null) return;
            int id = npc.transform.GetInstanceID();
            if (_sparkles.TryGetValue(id, out var s)) { if (s != null) UnityEngine.Object.Destroy(s); _sparkles.Remove(id); }
        }

        static GameObject SpawnSparkles(Transform root)
        {
            var go = new GameObject("FemmeFatale_Sparkles");
            go.transform.SetParent(root);
            go.transform.localPosition = new Vector3(0f, 1.0f, 0f);
            go.transform.localRotation = Quaternion.identity;

            var ps   = go.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.duration        = 999f;
            main.loop            = true;
            main.startLifetime   = new ParticleSystem.MinMaxCurve(0.5f, 1.0f);
            main.startSpeed      = new ParticleSystem.MinMaxCurve(0.5f, 1.8f);
            main.startSize       = 0.025f;
            main.startColor      = new Color(1f, 0.70f, 0.90f);
            main.gravityModifier = -0.04f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.rateOverTime = 35f;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius    = 0.45f;

            var col = ps.colorOverLifetime;
            col.enabled = true;
            var g = new Gradient();
            g.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(new Color(1f,   0.60f, 0.90f), 0f),
                    new GradientColorKey(new Color(1f,   0.95f, 0.50f), 0.5f),
                    new GradientColorKey(new Color(1f,   1f,   1f),    1f)
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(1f,   0f),
                    new GradientAlphaKey(0.6f, 0.5f),
                    new GradientAlphaKey(0f,   1f)
                }
            );
            col.color = new ParticleSystem.MinMaxGradient(g);

            FertilizerVisuals.SetMaterial(go);
            ps.Play();
            return go;
        }
    }

    // Depth — divaeffect + toxic
    // Deep purple-green screen tint — glamour meets venom
    public class VenomKissEffect : Effect
    {
        private static readonly Dictionary<int, GameObject> _overlays = new Dictionary<int, GameObject>();

        public override void ApplyToPlayer(Player player)
        {
            if (player == null) return;
            int id = player.transform.GetInstanceID();
            if (_overlays.ContainsKey(id)) return;
            try { _overlays[id] = FertilizerVisuals.CreateScreenOverlay(new Color(0.35f, 0f, 0.45f, 0.18f)); }
            catch (System.Exception ex) { MelonLoader.MelonLogger.Error($"[VenomKiss] Apply failed: {ex.Message}"); }
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
    public class FemmeFataleEffectDef : CustomEffect
    {
        public override string ID              => "femmefatale";
        public override string Name            => "Femme Fatale";
        public override Color  Color           => new Color(1.00f, 0.40f, 0.70f);
        public override float  Addictiveness   => 0.30f;
        public override int    ValueChange     => 20;
        public override float  ValueMultiplier => 1.19f;

        public override Effect CreateInstance() => UnityEngine.ScriptableObject.CreateInstance<FemmeFataleEffect>();
    }

    public class VenomKissEffectDef : CustomEffect
    {
        public override string ID              => "venomkiss";
        public override string Name            => "Venom Kiss";
        public override Color  Color           => new Color(0.40f, 0.00f, 0.55f);
        public override float  Addictiveness   => 0.50f;
        public override int    ValueChange     => 30;
        public override float  ValueMultiplier => 1.26f;

        public override Effect CreateInstance() => UnityEngine.ScriptableObject.CreateInstance<VenomKissEffect>();
    }
}
