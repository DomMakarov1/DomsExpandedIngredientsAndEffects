using UnityEngine;
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
    public class MartianEffect : Effect
    {
        private const string STACK_KEY    = "martian";
        private static readonly Color MartianOrange = new Color(0.85f, 0.72f, 0.55f);

        public override void ApplyToPlayer(Player player)
        {
            if (player == null || !player.IsOwner) return;
            player.Avatar?.Effects?.SkinColorSmoother.AddOverride(MartianOrange, 7, STACK_KEY);
        }

        public override void ClearFromPlayer(Player player)
        {
            if (player == null || !player.IsOwner) return;
            player.Avatar?.Effects?.SkinColorSmoother.RemoveOverride(STACK_KEY);
        }

        public override void ApplyToNPC(NPC npc)
        {
            if (npc == null) return;
            npc.Avatar?.Effects?.SkinColorSmoother.AddOverride(MartianOrange, 7, STACK_KEY);
        }

        public override void ClearFromNPC(NPC npc)
        {
            if (npc == null) return;
            npc.Avatar?.Effects?.SkinColorSmoother.RemoveOverride(STACK_KEY);
        }
    }
}

namespace DomsExpandedIngredientsAndEffects.Effects.Descriptors
{
    public class MartianEffectDef : CustomEffect
    {
        public override string ID             => "martianeffect";
        public override string Name           => "Martian";
        public override Color  Color          => new Color(0.85f, 0.72f, 0.55f);
        public override float  Addictiveness  => 0.80f;
        public override int    ValueChange    => 50;
        public override float  ValueMultiplier => 1.2f;

        public override Effect CreateInstance() => UnityEngine.ScriptableObject.CreateInstance<MartianEffect>();
    }
}