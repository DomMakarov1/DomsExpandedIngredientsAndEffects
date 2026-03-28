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
    public class SuperChargeEffect : Effect
    {
        private static Coroutine _timer;
        private static bool      _timerRunning;

        public override void ApplyToPlayer(Player player)
        {
            if (player == null || !player.IsOwner) return;
            PlayerMovement.StaticMoveSpeedMultiplier = 2f;
            PlayerMovement.JumpMultiplier            = 3f;
            PlayerMovement.GravityMultiplier         = 0.9f;
            player.Avatar?.Effects?.SetGlowingOn(new Color(0.5f, 0.0f, 1.0f));

            if (_timer != null) MelonLoader.MelonCoroutines.Stop(_timer);
            _timer = (Coroutine)MelonLoader.MelonCoroutines.Start(AutoClear(player));
        }

        public override void ClearFromPlayer(Player player)
        {
            if (player == null || !player.IsOwner) return;
            if (_timerRunning) return;
            if (_timer != null) { MelonLoader.MelonCoroutines.Stop(_timer); _timer = null; }
            PlayerMovement.StaticMoveSpeedMultiplier = 1.0f;
            PlayerMovement.JumpMultiplier            = 1.0f;
            PlayerMovement.GravityMultiplier         = 1.0f;
            player.Avatar?.Effects?.SetGlowingOff();
        }

        public override void ApplyToNPC(NPC npc)
        {
            if (npc?.Movement?.SpeedController != null)
                npc.Movement.SpeedController.SpeedMultiplier = 2.5f;
        }

        public override void ClearFromNPC(NPC npc)
        {
            if (npc?.Movement?.SpeedController != null)
                npc.Movement.SpeedController.SpeedMultiplier = 1.0f;
        }

        private static IEnumerator AutoClear(Player player)
        {
            _timerRunning = true;
            yield return new WaitForSeconds(600f);
            _timerRunning = false;
            _timer = null;
            if (player == null || !player.IsOwner) yield break;
            PlayerMovement.StaticMoveSpeedMultiplier = 1.0f;
            PlayerMovement.JumpMultiplier            = 1.0f;
            PlayerMovement.GravityMultiplier         = 1.0f;
            player.Avatar?.Effects?.SetGlowingOff();
        }
    }
}

namespace DomsExpandedIngredientsAndEffects.Effects.Descriptors
{
    public class SuperChargeEffectDef : CustomEffect
    {
        public override string ID              => "supercharge";
        public override string Name            => "Super Charge";
        public override Color  Color           => new Color(0.5f, 0.0f, 1.0f);
        public override float  Addictiveness   => 0.20f;
        public override int    ValueChange     => 0;
        public override float  ValueMultiplier => 1.0f;

        public override Effect CreateInstance() => UnityEngine.ScriptableObject.CreateInstance<SuperChargeEffect>();
    }
}