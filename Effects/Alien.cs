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
    public class AlienEffect : Effect
    {
        private const string STACK_KEY  = "alien";
        private static readonly Color AlienGreen = new Color(0.4f, 0.55f, 0.3f);

        public override void ApplyToPlayer(Player player)
        {
            if (player == null || !player.IsOwner) return;
            player.Avatar?.Effects?.SkinColorSmoother.AddOverride(AlienGreen, 7, STACK_KEY);
        }

        public override void ClearFromPlayer(Player player)
        {
            if (player == null || !player.IsOwner) return;
            player.Avatar?.Effects?.SkinColorSmoother.RemoveOverride(STACK_KEY);
        }

        public override void ApplyToNPC(NPC npc)
        {
            if (npc == null) return;
            npc.Avatar?.Effects?.SkinColorSmoother.AddOverride(AlienGreen, 7, STACK_KEY);
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
    public class AlienEffectDef : CustomEffect
    {
        public override string ID             => "alieneffect";
        public override string Name           => "Alien";
        public override Color  Color          => new Color(0.4f, 0.55f, 0.3f);
        public override float  Addictiveness  => 0.50f;
        public override int    ValueChange    => 30;
        public override float  ValueMultiplier => 1.25f;

        public override Effect CreateInstance() => UnityEngine.ScriptableObject.CreateInstance<AlienEffect>();
    }
}