using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DomsExpandedIngredientsAndEffects.Framework;

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
    public class VoluptuousEffect : Effect
    {
        protected const string STACK_KEY       = "voluptuous";
        protected virtual int    LIP_LAYER     => 2;
        protected virtual string LipTextureName => "Lips.png";
        protected virtual Color  LipColor       => Color.white;
        protected virtual Color  EyeGlamColor   => new Color(0.9f, 0.4f, 0.6f);
        protected virtual Color  GlowColor      => new Color(0.3f, 0.05f, 0.1f);

        private static readonly Dictionary<string, Texture2D> _textureCache
            = new Dictionary<string, Texture2D>();

        private static readonly Dictionary<int, List<GameObject>> _lashObjects
            = new Dictionary<int, List<GameObject>>();

        private static readonly Dictionary<int, Vector3> _originalHipScale
            = new Dictionary<int, Vector3>();

        public override void ApplyToPlayer(Player player)
        {
            if (player == null || !player.IsOwner) return;
            var avatar = player.Avatar;
            var fx     = avatar?.Effects;
            if (fx == null) return;

            ApplyVisuals(avatar, fx, isPlayer: true);
            MelonLoader.MelonCoroutines.Start(AutoClear(player, null, 900f));
        }

        public override void ClearFromPlayer(Player player)
        {
            if (player == null || !player.IsOwner) return;
            ClearVisuals(player.Avatar, player.Avatar?.Effects);
        }

        public override void ApplyToNPC(NPC npc)
        {
            if (npc == null) return;
            var avatar = npc.Avatar;
            var fx     = avatar?.Effects;
            if (fx == null) return;

            ApplyVisuals(avatar, fx, isPlayer: false);
            MelonLoader.MelonCoroutines.Start(AutoClear(null, npc, 900f));
        }

        public override void ClearFromNPC(NPC npc)
        {
            if (npc == null) return;
            ClearVisuals(npc.Avatar, npc.Avatar?.Effects);
        }

        protected virtual void ApplyVisuals(Avatar avatar, AvatarEffects fx, bool isPlayer)
        {
            fx.OverrideEyeColor(EyeGlamColor, 0.3f);
            fx.SetGlowingOn(GlowColor);

            ApplyBoobs(avatar);
            ApplyButtScale(avatar);
            ApplyLipLayer(avatar);
            ApplyEyelashes(avatar);
        }

        private static readonly Dictionary<int, float> _originalGenderShape
            = new Dictionary<int, float>();
        private static readonly Dictionary<int, float> _originalWeightShape
            = new Dictionary<int, float>();

        private void ApplyBoobs(Avatar avatar)
        {
            if (avatar?.ShapeKeyMeshes == null) return;
            int id = avatar.GetInstanceID();

            foreach (var mesh in avatar.ShapeKeyMeshes)
            {
                if (mesh == null || mesh.sharedMesh.blendShapeCount < 2) continue;
                if (!_originalGenderShape.ContainsKey(id))
                    _originalGenderShape[id] = mesh.GetBlendShapeWeight(0);
                if (!_originalWeightShape.ContainsKey(id))
                    _originalWeightShape[id] = mesh.GetBlendShapeWeight(1);

                mesh.SetBlendShapeWeight(0, Mathf.Clamp(_originalGenderShape[id] + 80f, 0f, 100f));
            }
        }

        private void ClearBoobs(Avatar avatar)
        {
            if (avatar?.ShapeKeyMeshes == null) return;
            int id = avatar.GetInstanceID();
            if (!_originalGenderShape.TryGetValue(id, out var origGender)) return;

            foreach (var mesh in avatar.ShapeKeyMeshes)
                if (mesh != null && mesh.sharedMesh.blendShapeCount >= 1)
                    mesh.SetBlendShapeWeight(0, origGender);

            _originalGenderShape.Remove(id);
        }

        protected void ClearVisuals(Avatar avatar, AvatarEffects fx)
        {
            if (fx != null)
            {
                fx.ResetEyeColor();
                fx.SetGlowingOff();
            }
            ClearBoobs(avatar);
            ClearButtScale(avatar);
            ClearLipLayer(avatar);
            ClearEyelashes(avatar);
        }

        private IEnumerator AutoClear(Player player, NPC npc, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (player != null) ClearVisuals(player.Avatar, player.Avatar?.Effects);
            if (npc    != null) ClearVisuals(npc.Avatar,    npc.Avatar?.Effects);
        }

        private static readonly Dictionary<int, Vector3> _originalHipPos = new Dictionary<int, Vector3>();

        private void ApplyButtScale(Avatar avatar)
        {
            if (avatar?.ShapeKeyMeshes == null) return;
            int id = avatar.GetInstanceID();

            foreach (var mesh in avatar.ShapeKeyMeshes)
            {
                if (mesh == null || mesh.sharedMesh.blendShapeCount < 2) continue;
                if (!_originalWeightShape.ContainsKey(id))
                    _originalWeightShape[id] = mesh.GetBlendShapeWeight(1);
                mesh.SetBlendShapeWeight(1, Mathf.Clamp(_originalWeightShape[id] + 80f, 0f, 100f));
            }

            if (avatar.HipBone != null)
            {
                if (!_originalHipPos.ContainsKey(id))
                    _originalHipPos[id] = avatar.HipBone.localPosition;
                avatar.HipBone.localPosition = _originalHipPos[id] + new Vector3(0f, 0f, -0.06f);
            }
        }

        private void ClearButtScale(Avatar avatar)
        {
            if (avatar?.ShapeKeyMeshes == null) return;
            int id = avatar.GetInstanceID();

            if (_originalWeightShape.TryGetValue(id, out var origWeight))
            {
                foreach (var mesh in avatar.ShapeKeyMeshes)
                    if (mesh != null && mesh.sharedMesh.blendShapeCount >= 2)
                        mesh.SetBlendShapeWeight(1, origWeight);
                _originalWeightShape.Remove(id);
            }

            if (avatar.HipBone != null && _originalHipPos.TryGetValue(id, out var origPos))
            {
                avatar.HipBone.localPosition = origPos;
                _originalHipPos.Remove(id);
            }
        }

        protected void ApplyLipLayer(Avatar avatar)
        {
            if (avatar?.FaceMesh == null) return;
            Texture2D tex = GetCachedTexture(LipTextureName);
            if (tex == null)
            {
                MelonLoader.MelonLogger.Warning($"[Voluptuous] {LipTextureName} not found.");
                return;
            }
            avatar.FaceMesh.material.SetTexture("_Layer_" + LIP_LAYER + "_Texture", tex);
            avatar.FaceMesh.material.SetColor("_Layer_"   + LIP_LAYER + "_Color",   LipColor);
        }

        protected void ClearLipLayer(Avatar avatar)
        {
            if (avatar?.FaceMesh == null) return;
            avatar.FaceMesh.material.SetTexture("_Layer_" + LIP_LAYER + "_Texture", null);
            avatar.FaceMesh.material.SetColor("_Layer_"   + LIP_LAYER + "_Color",   Color.clear);
        }

        private static Texture2D GetCachedTexture(string fileName)
        {
            if (_textureCache.TryGetValue(fileName, out var cached) && cached != null)
            {
                return cached;
            }

            string path = System.IO.Path.Combine(
                System.IO.Directory.GetCurrentDirectory(),
                "Mods", "DomsCustomEffects", "Icons", fileName);

            var tex = DomsCustomEffects.LoadCustomTexture(fileName);
            if (tex != null) _textureCache[fileName] = tex;
            return tex;
        }

        protected void ApplyEyelashes(Avatar avatar)
        {
            if (avatar?.Eyes == null) return;
            int id = avatar.GetInstanceID();
            if (_lashObjects.ContainsKey(id)) return;

            var lashes = new List<GameObject>();

            if (avatar.Eyes.leftEye  != null)
                lashes.AddRange(BuildLashRow(avatar.Eyes.leftEye.transform,  true));
            if (avatar.Eyes.rightEye != null)
                lashes.AddRange(BuildLashRow(avatar.Eyes.rightEye.transform, false));

            if (lashes.Count == 0)
                MelonLoader.MelonLogger.Warning("[Voluptuous] No eye transforms for lashes.");

            _lashObjects[id] = lashes;
        }

        protected void ClearEyelashes(Avatar avatar)
        {
            if (avatar == null) return;
            int id = avatar.GetInstanceID();
            if (!_lashObjects.TryGetValue(id, out var lashes)) return;
            foreach (var obj in lashes)
                if (obj != null) UnityEngine.Object.Destroy(obj);
            _lashObjects.Remove(id);
        }

        private static List<GameObject> BuildLashRow(Transform eyeTransform, bool isLeft)
        {
            var result = new List<GameObject>();
            int   count  = 9;
            float spread = 0.008f;
            float length = 0.014f;
            float sign   = isLeft ? 1f : -1f;

            var mat = new Material(Shader.Find("Unlit/Color"));
            mat.color = Color.black;

            for (int i = 0; i < count; i++)
            {
                float t      = (i / (float)(count - 1)) - 0.5f;
                float xOff   = t * spread * sign;
                float curve  = Mathf.Abs(t) * 20f;

                var lash = new GameObject($"Lash_{(isLeft ? "L" : "R")}_{i}");
                lash.transform.SetParent(eyeTransform, false);
                lash.transform.localPosition = new Vector3(xOff, 0.003f, 0.006f);
                lash.transform.localRotation = Quaternion.Euler(-curve - 10f, 0f, 0f);

                var lr = lash.AddComponent<LineRenderer>();
                lr.useWorldSpace     = false;
                lr.positionCount     = 2;
                lr.SetPosition(0, Vector3.zero);
                lr.SetPosition(1, new Vector3(0f, length, 0f));
                lr.startWidth        = 0.0012f;
                lr.endWidth          = 0.0001f;
                lr.sharedMaterial    = mat;
                lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                lr.receiveShadows    = false;
                lr.sortingOrder      = 10;
                
                result.Add(lash);
            }
            return result;
        }
    }
}

namespace DomsExpandedIngredientsAndEffects.Effects.Descriptors
{
    public class VoluptuousEffectDef : CustomEffect
    {
        public override string ID              => "voluptuouseffect";
        public override string Name            => "Voluptuous";
        public override Color  Color           => new Color(0.9f, 0.1f, 0.3f);
        public override float  Addictiveness   => 0.3f;
        public override int    ValueChange     => 15;
        public override float  ValueMultiplier => 1.2f;

        public override Effect CreateInstance() => UnityEngine.ScriptableObject.CreateInstance<VoluptuousEffect>();
    }
}