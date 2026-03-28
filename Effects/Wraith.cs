using UnityEngine;
using System.Collections;
using DomsExpandedIngredientsAndEffects.Framework;

#if MONO
using ScheduleOne.Effects;
using ScheduleOne.NPCs;
using ScheduleOne.PlayerScripts;
using ScheduleOne.AvatarFramework;
#else
using Il2CppScheduleOne.Effects;
using Il2CppScheduleOne.NPCs;
using Il2CppScheduleOne.PlayerScripts;

using Il2CppScheduleOne.AvatarFramework;
#endif

namespace DomsExpandedIngredientsAndEffects.Effects
{
    public class WraithEffect : Effect
    {
        private const string STACK_KEY = "wraith";
        private static readonly Color WraithNavy  = new Color(0.1f, 0.1f, 0.35f);
        private static readonly Color WraithBlack = new Color(0.02f, 0.02f, 0.05f);

        private static bool _playerCycling;

        public override void ApplyToPlayer(Player player)
        {
            if (player == null || !player.IsOwner) return;
            PlayerMovement.StaticMoveSpeedMultiplier = 1.3f;
            PlayerMovement.GravityMultiplier         = 0.7f;
            _playerCycling = true;
            MelonLoader.MelonCoroutines.Start(CycleSkinColor(player.Avatar?.Effects, true));
        }

        public override void ClearFromPlayer(Player player)
        {
            if (player == null || !player.IsOwner) return;
            PlayerMovement.StaticMoveSpeedMultiplier = 1.0f;
            PlayerMovement.GravityMultiplier         = 1.0f;
            _playerCycling = false;
            player.Avatar?.Effects?.SkinColorSmoother.RemoveOverride(STACK_KEY);
        }

        public override void ApplyToNPC(NPC npc)
        {
            if (npc == null) return;
            if (npc.Movement?.SpeedController != null)
                npc.Movement.SpeedController.SpeedMultiplier = 1.3f;
            MelonLoader.MelonCoroutines.Start(CycleSkinColor(npc.Avatar?.Effects, false));
        }

        public override void ClearFromNPC(NPC npc)
        {
            if (npc == null) return;
            if (npc.Movement?.SpeedController != null)
                npc.Movement.SpeedController.SpeedMultiplier = 1.0f;
            npc.Avatar?.Effects?.SkinColorSmoother.RemoveOverride(STACK_KEY);
        }

        private static IEnumerator CycleSkinColor(AvatarEffects avatarEffects, bool isPlayer)
        {
            if (avatarEffects == null) yield break;

            bool showingNavy = true;
            avatarEffects.SkinColorSmoother.AddOverride(WraithNavy, 8, STACK_KEY);

            while (isPlayer ? _playerCycling : avatarEffects != null)
            {
                yield return new WaitForSeconds(1.5f);
                if (avatarEffects == null) yield break;
                showingNavy = !showingNavy;
                avatarEffects.SkinColorSmoother.AddOverride(
                    showingNavy ? WraithNavy : WraithBlack, 8, STACK_KEY);
            }
        }
    }
}

namespace DomsExpandedIngredientsAndEffects.Effects.Descriptors
{
    public class WraithEffectDef : CustomEffect
    {
        public override string ID              => "wraitheffect";
        public override string Name            => "Wraith";
        public override Color  Color           => new Color(0.1f, 0.1f, 0.3f);
        public override float  Addictiveness   => 0.45f;
        public override int    ValueChange     => 40;
        public override float  ValueMultiplier => 1.35f;

        public override Effect CreateInstance() => UnityEngine.ScriptableObject.CreateInstance<WraithEffect>();
    }
}