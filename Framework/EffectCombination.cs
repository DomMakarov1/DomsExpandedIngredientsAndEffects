using UnityEngine;
using DomsExpandedIngredientsAndEffects.Framework;

namespace DomsExpandedIngredientsAndEffects.Framework
{
    public class EffectCombination
    {
        public string          ExistingEffectID  { get; }
        public string          IngredientEffectID { get; }
        public CustomEffect    OutputEffect      { get; }
        public float           OutputRadius      { get; }

        public EffectCombination(
            string existingEffectID,
            string ingredientEffectID,
            CustomEffect outputEffect,
            float outputRadius = 0.45f)
        {
            ExistingEffectID   = existingEffectID;
            IngredientEffectID = ingredientEffectID;
            OutputEffect       = outputEffect;
            OutputRadius       = outputRadius;
        }
    }
}