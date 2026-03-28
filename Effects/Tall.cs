using UnityEngine;
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
    public class TallEffect : Effect
    {
        private const string STACK_KEY = "tall";

        public override void ApplyToPlayer(Player player)
        {
            if (player == null || !player.IsOwner) return;
            player.Avatar.Effects.SetGiraffeActive(true);
            player.transform.localScale = Vector3.one * 1.5f;
        }

        public override void ClearFromPlayer(Player player)
        {
            if (player == null || !player.IsOwner) return;
            player.Avatar.Effects.SetGiraffeActive(false);
            player.transform.localScale = Vector3.one;
        }

        public override void ApplyToNPC(NPC npc)
        {
            if (npc == null) return;
            npc.Avatar.Effects.SetGiraffeActive(true);
            npc.transform.localScale = Vector3.one * 1.5f;
        }

        public override void ClearFromNPC(NPC npc)
        {
            if (npc == null) return;
            npc.Avatar.Effects.SetGiraffeActive(false);
            npc.transform.localScale = Vector3.one;
        }
    }
}

namespace DomsExpandedIngredientsAndEffects.Effects.Descriptors
{
    public class TallEffectDef : CustomEffect
    {
        public override string ID              => "talleffect";
        public override string Name            => "Tall";
        public override Color  Color           => new Color(0.4f, 0.8f, 0.4f);
        public override float  Addictiveness   => 0.15f;
        public override int    ValueChange     => 12;
        public override float  ValueMultiplier => 1.1f;

        public override Effect CreateInstance() => UnityEngine.ScriptableObject.CreateInstance<TallEffect>();
    }
}