using UnityEngine;
using DomsExpandedIngredientsAndEffects.Framework;
using DomsExpandedIngredientsAndEffects.Effects;

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
    public class VoidEffect : Effect
    {
        private const string STACK_KEY = "void";

        public override void ApplyToPlayer(Player player)
        {
            if (player == null || !player.IsOwner) return;

            PlayerMovement.StaticMoveSpeedMultiplier = 0.85f;

            AvatarEffects avatarEffects = player.Avatar?.Effects;
            if (avatarEffects != null)
            {
                avatarEffects.SkinColorSmoother.AddOverride(Color.black, 7, STACK_KEY);
            }
        }

        public override void ClearFromPlayer(Player player)
        {
            if (player == null || !player.IsOwner) return;

            PlayerMovement.StaticMoveSpeedMultiplier = 1.0f;

            AvatarEffects avatarEffects = player.Avatar?.Effects;
            if (avatarEffects != null)
            {
                avatarEffects.SkinColorSmoother.RemoveOverride(STACK_KEY);
            }
        }

        public override void ApplyToNPC(NPC npc)
        {
            if (npc == null) return;

            if (npc.Movement?.SpeedController != null)
                npc.Movement.SpeedController.SpeedMultiplier = 0.85f;

            AvatarEffects avatarEffects = npc.Avatar?.Effects;
            if (avatarEffects != null)
                avatarEffects.SkinColorSmoother.AddOverride(Color.black, 7, STACK_KEY);
        }

        public override void ClearFromNPC(NPC npc)
        {
            if (npc == null) return;

            if (npc.Movement?.SpeedController != null)
                npc.Movement.SpeedController.SpeedMultiplier = 1.0f;

            AvatarEffects avatarEffects = npc.Avatar?.Effects;
            if (avatarEffects != null)
                avatarEffects.SkinColorSmoother.RemoveOverride(STACK_KEY);
        }
    }
}

namespace DomsExpandedIngredientsAndEffects.Effects.Descriptors
{
    public class VoidEffectDef : CustomEffect
    {
        public override string ID             => "voideffect";
        public override string Name           => "Void";
        public override Color  Color          => Color.black;
        public override float  Addictiveness  => 0.4f;
        public override int    ValueChange    => 25;
        public override float  ValueMultiplier => 1.3f;

        public override Effect CreateInstance() => UnityEngine.ScriptableObject.CreateInstance<VoidEffect>();
    }
}