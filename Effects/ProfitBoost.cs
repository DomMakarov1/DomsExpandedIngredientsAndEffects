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
    public class ProfitBoostEffect : Effect
    {
        public static bool IsActive { get; private set; }

        private static Coroutine _timer;
        private static bool      _timerRunning;

        public override void ApplyToPlayer(Player player)
        {
            if (player == null || !player.IsOwner) return;
            IsActive = true;
            if (_timer != null) MelonLoader.MelonCoroutines.Stop(_timer);
            _timer = (Coroutine)MelonLoader.MelonCoroutines.Start(AutoClear());
        }

        public override void ClearFromPlayer(Player player)
        {
            if (player == null || !player.IsOwner) return;
            if (_timerRunning) return;
            if (_timer != null) { MelonLoader.MelonCoroutines.Stop(_timer); _timer = null; }
            IsActive = false;
        }

        public override void ApplyToNPC(NPC npc) { }
        public override void ClearFromNPC(NPC npc) { }

        private static IEnumerator AutoClear()
        {
            _timerRunning = true;
            yield return new WaitForSeconds(600f);
            _timerRunning = false;
            _timer = null;
            IsActive = false;
        }
    }
}

namespace DomsExpandedIngredientsAndEffects.Effects.Descriptors
{
    public class ProfitBoostEffectDef : CustomEffect
    {
        public override string ID              => "profitboost";
        public override string Name            => "Profit Boost";
        public override Color  Color           => new Color(0.2f, 0.9f, 0.2f);
        public override float  Addictiveness   => 0.15f;
        public override int    ValueChange     => 0;
        public override float  ValueMultiplier => 1.0f;

        public override Effect CreateInstance() => UnityEngine.ScriptableObject.CreateInstance<ProfitBoostEffect>();
    }
}