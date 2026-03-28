using UnityEngine;
using System.Collections;
using DomsExpandedIngredientsAndEffects.Framework;

#if MONO
using ScheduleOne.Effects;
using ScheduleOne.NPCs;
using ScheduleOne.PlayerScripts;
using ScheduleOne.AvatarFramework;
using ScheduleOne.DevUtilities;
#else
using Il2CppScheduleOne.Effects;
using Il2CppScheduleOne.NPCs;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.AvatarFramework;
using Il2CppScheduleOne.DevUtilities;

#endif

namespace DomsExpandedIngredientsAndEffects.Effects
{
    public class SuperMartianEffect : Effect
    {
        private const string STACK_KEY = "supermartian";
        private static readonly Color AlienGreen   = new Color(0.4f,  0.55f, 0.3f);
        private static readonly Color MartianOrange = new Color(0.85f, 0.72f, 0.55f);
        private static Coroutine _playerCoroutine;
        private static bool      _playerCycling;

        public override void ApplyToPlayer(Player player)
        {
            if (player == null || !player.IsOwner) return;

            PlayerMovement.StaticMoveSpeedMultiplier = 1.2f;
            PlayerMovement.JumpMultiplier            = 1.2f;

            _playerCycling = true;
            _playerCoroutine = MelonLoader.MelonCoroutines.Start(
                CycleSkinColor(player.Avatar?.Effects, true)) as Coroutine;
        }

        public override void ClearFromPlayer(Player player)
        {
            if (player == null || !player.IsOwner) return;

            PlayerMovement.StaticMoveSpeedMultiplier = 1.0f;
            PlayerMovement.JumpMultiplier            = 1.0f;

            _playerCycling = false;
            player.Avatar?.Effects?.SkinColorSmoother.RemoveOverride(STACK_KEY);
        }

        public override void ApplyToNPC(NPC npc)
        {
            if (npc == null) return;
            MelonLoader.MelonCoroutines.Start(CycleSkinColor(npc.Avatar?.Effects, false));
        }

        public override void ClearFromNPC(NPC npc)
        {
            if (npc == null) return;
            npc.Avatar?.Effects?.SkinColorSmoother.RemoveOverride(STACK_KEY);
        }

        private static IEnumerator CycleSkinColor(AvatarEffects avatarEffects, bool isPlayer)
        {
            if (avatarEffects == null) yield break;

            bool showingGreen = true;
            avatarEffects.SkinColorSmoother.AddOverride(AlienGreen, 8, STACK_KEY);

            while (isPlayer ? _playerCycling : avatarEffects != null)
            {
                yield return new WaitForSeconds(2f);

                if (avatarEffects == null) yield break;

                showingGreen = !showingGreen;
                Color target = showingGreen ? AlienGreen : MartianOrange;
                avatarEffects.SkinColorSmoother.AddOverride(target, 8, STACK_KEY);
            }
        }
    }
}

namespace DomsExpandedIngredientsAndEffects.Effects.Descriptors
{
    public class SuperMartianEffectDef : CustomEffect
    {
        public override string ID              => "supermartianeffect";
        public override string Name            => "Super Martian";
        public override Color  Color           => new Color(0.6f, 0.6f, 0.4f);
        public override float  Addictiveness   => 1.0f;
        public override int    ValueChange     => 100;
        public override float  ValueMultiplier => 1.5f;

        public override Effect CreateInstance() => UnityEngine.ScriptableObject.CreateInstance<SuperMartianEffect>();
    }
}