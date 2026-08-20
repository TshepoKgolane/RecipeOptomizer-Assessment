using RecipeOptomizer_Assessment.Business.Models;
using RecipeOptomizer_Assessment.Business.Models.Responses;

namespace RecipeOptomizer_Assessment.Helper
{
    public class MealPlanProcessor
    {
        private readonly IReadOnlyList<Recipe> _recipes;
        private readonly string[] _recipeIngredientNames;
        private readonly Dictionary<string, Plan> _rememberedBestPlans = new(StringComparer.Ordinal);

        public MealPlanProcessor(List<Recipe> recipes)
        {
            _recipes = recipes ?? throw new ArgumentNullException(nameof(recipes)); // thrqwing early and catching on the calling function
            if (_recipes.Count == 0)
                throw new ArgumentException("At least one recipe is required.", nameof(recipes));

            _recipeIngredientNames = _recipes
                .SelectMany(recipe => recipe.RequiredIngredients.Keys)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        public MealPrepResponse ProcessOptimalMeal(IEnumerable<Ingredient> ingredients)
        {
            var pantry = CreatePantry(ingredients);
            var recipePantry = _recipeIngredientNames.ToDictionary(
                name => name,
                name => pantry.GetValueOrDefault(name),
                StringComparer.OrdinalIgnoreCase);

            var bestPlan = FindBestPlan(recipePantry);
            var usedIngredients = CountUsedIngredients(bestPlan);

            return new MealPrepResponse
            {
                totalPeopleFed = bestPlan.PeopleFed,
                MealPlan = _recipes
                    .Where(recipe => bestPlan.MealCounts.ContainsKey(recipe.Name))
                    .Select(recipe => new MealPlan
                    {
                        RecipeName = recipe.Name,
                        amount = bestPlan.MealCounts[recipe.Name],
                        Feeds = checked(bestPlan.MealCounts[recipe.Name] * recipe.Feeds)
                    })
                    .ToList(),
                ingredientsUsed = usedIngredients,
                ingredientsRemaining = pantry.ToDictionary(
                    ingredient => ingredient.Key,
                    ingredient => ingredient.Value - usedIngredients.GetValueOrDefault(ingredient.Key),
                    StringComparer.OrdinalIgnoreCase)
            };
        }

        private static Dictionary<string, int> CreatePantry(IEnumerable<Ingredient> ingredients)
        {
            var pantry = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var ingredient in ingredients)
            {
                if (ingredient is null || string.IsNullOrWhiteSpace(ingredient.Name))
                    throw new ArgumentException("Every ingredient must have a name.");
                if (ingredient.Quantity < 0)
                    throw new ArgumentException("Ingredient quantities cannot be negative.");

                var name = ingredient.Name.Trim();
                pantry[name] = checked(pantry.GetValueOrDefault(name) + ingredient.Quantity);
            }

            return pantry;
        }

        private Plan FindBestPlan(IReadOnlyDictionary<string, int> remainingIngredients)
        {
            var pantryKey = string.Join(',', _recipeIngredientNames.Select(name => remainingIngredients.GetValueOrDefault(name)));
            if (_rememberedBestPlans.TryGetValue(pantryKey, out var rememberedPlan))
                return rememberedPlan;

            var bestPlan = Plan.Empty;

            foreach (var recipe in _recipes.Where(recipe => CanMake(recipe, remainingIngredients)))
            {
                var smallerPantry = new Dictionary<string, int>(remainingIngredients, StringComparer.OrdinalIgnoreCase);
                foreach (var (ingredient, quantity) in recipe.RequiredIngredients)
                    smallerPantry[ingredient] -= quantity;

                var candidate = FindBestPlan(smallerPantry).Add(recipe);
                if (IsBetter(candidate, bestPlan))
                    bestPlan = candidate;
            }

            _rememberedBestPlans[pantryKey] = bestPlan;
            return bestPlan;
        }

        private static bool CanMake(Recipe recipe, IReadOnlyDictionary<string, int> pantry) =>
            recipe.RequiredIngredients.All(x => pantry.GetValueOrDefault(x.Key) >= x.Value);

        private Dictionary<string, int> CountUsedIngredients(Plan plan)
        {
            var used = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var recipe in _recipes)
            {
                var amount = plan.MealCounts.GetValueOrDefault(recipe.Name);
                foreach (var (ingredient, quantity) in recipe.RequiredIngredients)
                {
                    used[ingredient] = checked(used.GetValueOrDefault(ingredient) + quantity * amount);
                }
            }

            return used.Where(x => x.Value > 0)
                .ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);
        }

        private static bool IsBetter(Plan candidate, Plan currentBest)
        {
            if (candidate.PeopleFed != currentBest.PeopleFed)
                return candidate.PeopleFed > currentBest.PeopleFed;

            // Prefer a plan that uses fewer ingredient units when it feeds the same number of people.
            if (candidate.IngredientUnitsUsed != currentBest.IngredientUnitsUsed)
                return candidate.IngredientUnitsUsed < currentBest.IngredientUnitsUsed;

            return false;
        }

        private sealed record Plan(int PeopleFed, int IngredientUnitsUsed, Dictionary<string, int> MealCounts)
        {
            public static Plan Empty { get; } = new(0, 0, new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase));

            public Plan Add(Recipe recipe)
            {
                var mealCounts = new Dictionary<string, int>(MealCounts, StringComparer.OrdinalIgnoreCase)
                {
                    [recipe.Name] = MealCounts.GetValueOrDefault(recipe.Name) + 1
                };

                return new Plan(
                    checked(PeopleFed + recipe.Feeds),
                    checked(IngredientUnitsUsed + recipe.RequiredIngredients.Values.Sum()),
                    mealCounts);
            }
        }
    }
}
