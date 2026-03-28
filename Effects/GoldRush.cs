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
    public class GoldRushEffect : VoluptuousEffect
    {
        private const string GOLD_KEY = "goldrush_skin";
        private static readonly Color GoldSkin = new Color(1.0f, 0.84f, 0.2f);
        protected override string LipTextureName => "GoldLips.png";
        protected override Color  LipColor       => Color.white;
        protected override Color  EyeGlamColor   => new Color(1.0f, 0.75f, 0.0f);
        protected override Color  GlowColor      => new Color(0.4f, 0.3f, 0.0f);

        protected override void ApplyVisuals(Avatar avatar, AvatarEffects fx, bool isPlayer)
        {
            base.ApplyVisuals(avatar, fx, isPlayer);
            fx?.SkinColorSmoother.AddOverride(GoldSkin, 8, GOLD_KEY);
        }

        public override void ClearFromPlayer(Player player)
        {
            base.ClearFromPlayer(player);
            player?.Avatar?.Effects?.SkinColorSmoother.RemoveOverride(GOLD_KEY);
        }

        public override void ClearFromNPC(NPC npc)
        {
            base.ClearFromNPC(npc);
            npc?.Avatar?.Effects?.SkinColorSmoother.RemoveOverride(GOLD_KEY);
        }
    }
}

namespace DomsExpandedIngredientsAndEffects.Effects.Descriptors
{
    public class GoldRushEffectDef : CustomEffect
    {
        public override string ID              => "goldrusheffect";
        public override string Name            => "Gold Rush";
        public override Color  Color           => new Color(1.0f, 0.84f, 0.2f);
        public override float  Addictiveness   => 0.45f;
        public override int    ValueChange     => 18;
        public override float  ValueMultiplier => 1.4f;

        public override Effect CreateInstance() => UnityEngine.ScriptableObject.CreateInstance<GoldRushEffect>();
    }
}