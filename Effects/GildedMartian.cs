using UnityEngine;
using System.Collections;
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
    public class GildedMartianEffect : Effect
    {
        private const string STACK_KEY = "gildedmartian";
        private static readonly Color GoldTone   = new Color(1.0f, 0.84f, 0.2f);
        private static readonly Color OrangeTone = new Color(0.95f, 0.65f, 0.1f);

        private static bool _playerCycling;

        public override void ApplyToPlayer(Player player)
        {
            if (player == null || !player.IsOwner) return;
            var fx = player.Avatar?.Effects;
            if (fx == null) return;

            PlayerMovement.StaticMoveSpeedMultiplier = 1.2f;
            PlayerMovement.JumpMultiplier            = 1.2f;
            fx.OverrideEyeColor(GoldTone, 0.5f);
            fx.SetGlowingOn(new Color(0.5f, 0.4f, 0.0f));

            _playerCycling = true;
            MelonLoader.MelonCoroutines.Start(CycleSkin(fx, true));
        }

        public override void ClearFromPlayer(Player player)
        {
            if (player == null || !player.IsOwner) return;
            var fx = player.Avatar?.Effects;
            if (fx == null) return;

            _playerCycling = false;
            PlayerMovement.StaticMoveSpeedMultiplier = 1.0f;
            PlayerMovement.JumpMultiplier            = 1.0f;
            fx.ResetEyeColor();
            fx.SetGlowingOff();
            fx.SkinColorSmoother.RemoveOverride(STACK_KEY);
        }

        public override void ApplyToNPC(NPC npc)
        {
            if (npc == null) return;
            var fx = npc.Avatar?.Effects;
            if (fx == null) return;

            fx.OverrideEyeColor(GoldTone, 0.5f);
            fx.SetGlowingOn(new Color(0.5f, 0.4f, 0.0f));
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

        private static IEnumerator CycleSkin(AvatarEffects fx, bool isPlayer)
        {
            if (fx == null) yield break;
            bool showingGold = true;
            fx.SkinColorSmoother.AddOverride(GoldTone, 9, STACK_KEY);

            while (isPlayer ? _playerCycling : fx != null)
            {
                yield return new WaitForSeconds(2f);
                if (fx == null) yield break;
                showingGold = !showingGold;
                fx.SkinColorSmoother.AddOverride(showingGold ? GoldTone : OrangeTone, 9, STACK_KEY);
            }
        }
    }
}

namespace DomsExpandedIngredientsAndEffects.Effects.Descriptors
{
    public class GildedMartianEffectDef : CustomEffect
    {
        public override string ID              => "gildedmartianeffect";
        public override string Name            => "Gilded Martian";
        public override Color  Color           => new Color(1.0f, 0.75f, 0.15f);
        public override float  Addictiveness   => 0.70f;
        public override int    ValueChange     => 80;
        public override float  ValueMultiplier => 1.6f;

        public override Effect CreateInstance() =>UnityEngine.ScriptableObject.CreateInstance<GildedMartianEffect>();
    }
}