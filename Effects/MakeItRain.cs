using UnityEngine;
using System.Collections;
using DomsExpandedIngredientsAndEffects.Framework;

#if MONO
using ScheduleOne.Effects;
using ScheduleOne.NPCs;
using ScheduleOne.PlayerScripts;
using ScheduleOne.Weather;
using ScheduleOne.DevUtilities;
#else
using Il2CppScheduleOne.Effects;
using Il2CppScheduleOne.NPCs;
using Il2CppScheduleOne.PlayerScripts;
using Il2CppScheduleOne.Weather;

using Il2CppScheduleOne.DevUtilities;
#endif

namespace DomsExpandedIngredientsAndEffects.Effects
{
    public class MakeItRainEffect : Effect
    {
        private static string _originalWeatherID;
        private static bool   _rainActive;

        public override void ApplyToPlayer(Player player)
        {
            if (player == null || !player.IsOwner) return;
            if (_rainActive) return;
            _rainActive = true;
            MelonLoader.MelonCoroutines.Start(DoRain());
        }

        public override void ClearFromPlayer(Player player)
        {
            // Rain restores itself after duration
        }

        public override void ApplyToNPC(NPC npc) { }
        public override void ClearFromNPC(NPC npc) { }

        private static IEnumerator DoRain()
        {
            var em = NetworkSingleton<EnvironmentManager>.Instance;
            if (em == null) { _rainActive = false; yield break; }

            WeatherConditions rainConditions = new WeatherConditions
            {
                Sunny  = 0f,
                Cloudy = 1f,
                Rainy  = 1f,
                Stormy = 0.5f,
                Snowy  = 0f,
                Foggy  = 0f,
                Windy  = 0.8f,
                Hail   = 0f,
                Sleet  = 0f
            };

            EnvironmentHandler.RaiseWeatherChange(rainConditions);
            MelonLoader.MelonLogger.Msg("MAKEITRAIN IS CURRENTLY BROKEN, WILL BE FIXED IN FUTURE");

            _rainActive = false;
        }

        private static string FindHeavyRainSequenceID(EnvironmentManager em)
        {
            var field = typeof(EnvironmentManager).GetField("_weatherSequences",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var sequences = field?.GetValue(em) as System.Collections.Generic.List<WeatherSequence>;
            if (sequences == null) return null;

            foreach (var seq in sequences)
                if (seq.Id.ToLower().Contains("heavy") && seq.Id.ToLower().Contains("rain"))
                    return seq.Id;

            return null;
        }

        private static string FindRainSequenceID(EnvironmentManager em)
        {
            var field = typeof(EnvironmentManager).GetField("_weatherSequences",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var sequences = field?.GetValue(em) as System.Collections.Generic.List<WeatherSequence>;
            if (sequences == null) return null;

            foreach (var seq in sequences)
                if (seq.Id.ToLower().Contains("rain"))
                    return seq.Id;

            return null;
        }
    }
}

namespace DomsExpandedIngredientsAndEffects.Effects.Descriptors
{
    public class MakeItRainEffectDef : CustomEffect
    {
        public override string ID              => "makeitrain";
        public override string Name            => "Make It Rain";
        public override Color  Color           => new Color(0.3f, 0.5f, 1.0f);
        public override float  Addictiveness   => 0.10f;
        public override int    ValueChange     => 0;
        public override float  ValueMultiplier => 1.0f;

        public override Effect CreateInstance() =>UnityEngine.ScriptableObject.CreateInstance<MakeItRainEffect>();
    }
}