using UnityEngine;
using System.Collections;
using DomsExpandedIngredientsAndEffects.Framework;

#if MONO
using ScheduleOne.Effects;
using ScheduleOne.NPCs;
using ScheduleOne.PlayerScripts;
using ScheduleOne.DevUtilities;
#else
using Il2CppScheduleOne.Effects;
using Il2CppScheduleOne.NPCs;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.DevUtilities;

#endif

namespace DomsExpandedIngredientsAndEffects.Effects
{
    public class InversionEffect : Effect
    {
        private static bool      _playerInverted;
        private static Coroutine _playerTimer;

        private static readonly System.Collections.Generic.Dictionary<int, Vector3>
            _originalNPCPositions = new System.Collections.Generic.Dictionary<int, Vector3>();

        public override void ApplyToPlayer(Player player)
        {
            if (player == null || !player.IsOwner) return;
            if (_playerInverted) return;
            _playerInverted = true;

            var playerCam = PlayerSingleton<PlayerCamera>.Instance;
            if (playerCam != null)
            {
                Camera cam = playerCam.GetComponentInChildren<Camera>();
                if (cam != null)
                {
                    Matrix4x4 proj = cam.projectionMatrix;
                    proj.m11 = -proj.m11;
                    cam.projectionMatrix = proj;
                }
            }

            if (_playerTimer != null) MelonLoader.MelonCoroutines.Stop(_playerTimer);
            _playerTimer = (Coroutine)MelonLoader.MelonCoroutines.Start(AutoClearPlayer(player));
        }

        public override void ClearFromPlayer(Player player)
        {
            if (!_playerInverted) return;
            _playerInverted = false;

            if (_playerTimer != null)
            {
                MelonLoader.MelonCoroutines.Stop(_playerTimer);
                _playerTimer = null;
            }

            var playerCam = PlayerSingleton<PlayerCamera>.Instance;
            if (playerCam != null)
            {
                Camera cam = playerCam.GetComponentInChildren<Camera>();
                cam?.ResetProjectionMatrix();
            }
        }
    private static readonly System.Collections.Generic.Dictionary<int, Vector3> _originalBodyScale = new System.Collections.Generic.Dictionary<int, Vector3>();
    public override void ApplyToNPC(NPC npc)
    {
        if (npc == null) return;
        int id = npc.GetInstanceID();

        if (npc.Avatar?.BodyContainer != null)
        {
            _originalBodyScale[id] = npc.Avatar.BodyContainer.localScale;

            var s = _originalBodyScale[id];
            npc.Avatar.BodyContainer.localScale = new Vector3(s.x, -s.y, s.z);
        }

        MelonLoader.MelonCoroutines.Start(AutoClearNPC(npc));
    }

    public override void ClearFromNPC(NPC npc)
    {
        if (npc == null) return;
        int id = npc.GetInstanceID();

        if (npc.Avatar?.BodyContainer != null)
        {
            if (_originalBodyScale.TryGetValue(id, out var orig))
            {
                npc.Avatar.BodyContainer.localScale = orig;
                _originalBodyScale.Remove(id);
            }
        }
    }

        private static IEnumerator AutoClearPlayer(Player player)
        {
            yield return new WaitForSeconds(30f);
            if (!_playerInverted) yield break;
            _playerInverted = false;
            var playerCam = PlayerSingleton<PlayerCamera>.Instance;
            if (playerCam != null)
                playerCam.GetComponentInChildren<Camera>()?.ResetProjectionMatrix();
        }

        private static IEnumerator AutoClearNPC(NPC npc)
        {
            yield return new WaitForSeconds(30f);
            if (npc == null) yield break;
            int id = npc.GetInstanceID();
            if (npc.Avatar?.BodyContainer != null && _originalBodyScale.TryGetValue(id, out var orig))
            {
                npc.Avatar.BodyContainer.localScale = orig;
                _originalBodyScale.Remove(id);
            }
        }

        private static void ClearCamera()
        {
            Camera cam = Camera.main;
            if (cam != null) cam.ResetProjectionMatrix();
        }
    }
}

namespace DomsExpandedIngredientsAndEffects.Effects.Descriptors
{
    public class InversionEffectDef : CustomEffect
    {
        public override string ID              => "inversioneffect";
        public override string Name            => "Inversion";
        public override Color  Color           => new Color(0.4f, 0.0f, 0.8f);
        public override float  Addictiveness   => 0.35f;
        public override int    ValueChange     => 28;
        public override float  ValueMultiplier => 1.25f;

        public override Effect CreateInstance() => UnityEngine.ScriptableObject.CreateInstance<InversionEffect>();
    }
}