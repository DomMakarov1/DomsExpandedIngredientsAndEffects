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
    public class MagicBaseEffect : Effect
    {
        public override void ApplyToPlayer(Player player) { }
        public override void ClearFromPlayer(Player player) { }
        public override void ApplyToNPC(NPC npc) { }
        public override void ClearFromNPC(NPC npc) { }
    }
    
    public class MagicJuiceMapEffect : Effect
    {
        public override void ApplyToPlayer(Player player) { }
        public override void ClearFromPlayer(Player player) { }
        public override void ApplyToNPC(NPC npc) { }
        public override void ClearFromNPC(NPC npc) { }
    }
}

namespace DomsExpandedIngredientsAndEffects.Effects.Descriptors
{
    public class MagicBaseEffectDef : CustomEffect
    {
        public override string ID              => "magicbase";
        public override string Name            => "Magic";
        public override Color  Color           => new Color(0.6f, 0.0f, 1.0f);
        public override float  Addictiveness   => 0.0f;
        public override int    ValueChange     => 0;
        public override float  ValueMultiplier => 1.0f;

        public override Effect CreateInstance() => UnityEngine.ScriptableObject.CreateInstance<MagicBaseEffect>();
    }

    public class MagicJuiceMapEffectDef : CustomEffect
    {
        public override string ID    => "magicjuicemap";
        public override string Name  => "Magic Juice";
        public override Color  Color => new Color(0.6f, 0.0f, 1.0f);

        public override Effect CreateInstance() => UnityEngine.ScriptableObject.CreateInstance<MagicJuiceMapEffect>();
    }
}