using System.Collections.Generic;
using DomsExpandedIngredientsAndEffects.Framework;

#if MONO
using ScheduleOne.Effects;
#else
using Il2CppScheduleOne.Effects;
#endif

namespace DomsExpandedIngredientsAndEffects.Patches
{
    public class RegisteredCombination 
    {
        public string       ExistingEffectID   { get; }
        public string       IngredientEffectID { get; }
        public CustomEffect OutputEffect       { get; }
        public Effect       BuiltOutputEffect  { get; }

        public RegisteredCombination(EffectCombination combo)
        {
            ExistingEffectID   = combo.ExistingEffectID;
            IngredientEffectID = combo.IngredientEffectID;
            OutputEffect       = combo.OutputEffect;
            BuiltOutputEffect  = combo.OutputEffect.Build();
        }
    }

    public static class CustomMixRegistry
    {
        private static List<RegisteredCombination> _combinations;
        public static List<RegisteredCombination> Combinations =>
            _combinations ?? (_combinations = new List<RegisteredCombination>());

        private static List<Effect> _outputEffects;
        public static List<Effect> OutputEffects =>
            _outputEffects ?? (_outputEffects = new List<Effect>());

        private static List<MultiEffectCombination> _multiCombinations;
        public static List<MultiEffectCombination> MultiCombinations =>
            _multiCombinations ?? (_multiCombinations = new List<MultiEffectCombination>());

        public static void Register(EffectCombination combo)
        {
            var registered = new RegisteredCombination(combo);
            Combinations.Add(registered);
            OutputEffects.Add(registered.BuiltOutputEffect);
        }

        public static void Register(MultiEffectCombination combo)
        {
            MultiCombinations.Add(combo);
            OutputEffects.Add(combo.BuiltOutputEffect);
        }
    }

    public class MultiEffectCombination
    {
        public string[]     RequiredEffectIDs  { get; }
        public string       IngredientEffectID { get; }
        public CustomEffect OutputEffect       { get; }
        public Effect       BuiltOutputEffect  { get; }

        public MultiEffectCombination(string[] requiredEffectIDs, string ingredientEffectID, CustomEffect outputEffect)
        {
            RequiredEffectIDs  = requiredEffectIDs;
            IngredientEffectID = ingredientEffectID;
            OutputEffect       = outputEffect;
            BuiltOutputEffect  = outputEffect.Build();
        }
    }
}