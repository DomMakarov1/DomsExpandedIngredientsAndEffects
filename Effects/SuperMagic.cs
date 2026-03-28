using UnityEngine;
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
    public class SuperMagicEffect : Effect
    {
        private const string STACK_KEY = "supermagic";
        private static readonly Color MagicPurple = new Color(0.6f, 0.0f, 1.0f);
        private static readonly Color MagicGold   = new Color(1.0f, 0.84f, 0.0f);
        private static bool _playerCycling;

        public override void ApplyToPlayer(Player player)
        {
            if (player == null || !player.IsOwner) return;
            var fx = player.Avatar?.Effects;
            if (fx == null) return;

            fx.OverrideEyeColor(MagicPurple, 0.6f);
            fx.SetGlowingOn(new Color(0.3f, 0.0f, 0.5f));
            _playerCycling = true;
            MelonLoader.MelonCoroutines.Start(CycleSkin(fx, true));
        }

        public override void ClearFromPlayer(Player player)
        {
            if (player == null || !player.IsOwner) return;
            var fx = player.Avatar?.Effects;
            if (fx == null) return;

            _playerCycling = false;
            fx.ResetEyeColor();
            fx.SetGlowingOff();
            fx.SkinColorSmoother.RemoveOverride(STACK_KEY);
        }

        public override void ApplyToNPC(NPC npc)
        {
            if (npc == null) return;
            var fx = npc.Avatar?.Effects;
            if (fx == null) return;
            fx.OverrideEyeColor(MagicPurple, 0.6f);
            fx.SetGlowingOn(new Color(0.3f, 0.0f, 0.5f));
            MelonLoader.MelonCoroutines.Start(CycleSkin(fx, false));
        }

        public override void ClearFromNPC(NPC npc)
        {
            if (npc == null) return;
            var fx = npc.Avatar?.Effects;
            if (fx == null) return;
            fx.ResetEyeColor();
            fx.SetGlowingOff();
            fx.SkinColorSmoother.RemoveOverride(STACK_KEY);
        }

        private static System.Collections.IEnumerator CycleSkin(AvatarEffects fx, bool isPlayer)
        {
            if (fx == null) yield break;
            bool showingPurple = true;
            fx.SkinColorSmoother.AddOverride(MagicPurple, 9, STACK_KEY);

            while (isPlayer ? _playerCycling : fx != null)
            {
                yield return new WaitForSeconds(1.5f);
                if (fx == null) yield break;
                showingPurple = !showingPurple;
                fx.SkinColorSmoother.AddOverride(showingPurple ? MagicPurple : MagicGold, 9, STACK_KEY);
            }
        }
    }
}

namespace DomsExpandedIngredientsAndEffects.Effects.Descriptors
{
    public class SuperMagicEffectDef : CustomEffect
    {
        public override string ID              => "supermagic";
        public override string Name            => "Super Magic";
        public override Color  Color           => new Color(0.6f, 0.0f, 1.0f);
        public override float  Addictiveness   => 0.25f;
        public override int    ValueChange     => 0;
        public override float  ValueMultiplier => 1.0f;

        public override Effect CreateInstance() => UnityEngine.ScriptableObject.CreateInstance<SuperMagicEffect>();
    }
}