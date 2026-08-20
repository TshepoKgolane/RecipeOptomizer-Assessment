using RecipeOptomizer_Assessment.Business.Models;
using RecipeOptomizer_Assessment.Business.Models.Requests;
using RecipeOptomizer_Assessment.Business.Models.Responses;
using RecipeOptomizer_Assessment.Helper;
using System.Numerics;
using System.Xml.Linq;

namespace RecipeOptomizer_Assessment.Business.Services
{
    public class MealPrepService : IMealPrepService
    {
        public ProcessMealsResponseDto MealProcess(GetIngredientsRequestDto request)
        {
            try
            {
                ValidateRequest(request);

                var IngredientsDict = request.Ingredients
                    .Where(i => i.Quantity > 0)
                    .GroupBy(i => i.Name.Trim(), StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(g => g.Key, g => g.Sum(i => i.Quantity), StringComparer.OrdinalIgnoreCase);

                if (IngredientsDict.Count == 0)
                    return Failure("Please provide at least one ingredient with a quantity greater than zero.");
                

                var Recipes = request.UsePreExistingRecipes
                    ? RecipeBook.GetRecipeData()
                    : BuildCustomRecipes(request.Recipes!);

                if (!Recipes.Any())
                    return Failure("At least one recipe is required.");
                

                var Optimizer = new MealPlanProcessor(Recipes);
                var response = Optimizer.ProcessOptimalMeal(IngredientsDict.Select(x => new Ingredient
                {
                    Name = x.Key,
                    Quantity = x.Value
                }));

                return new ProcessMealsResponseDto
                {
                    IsSuccess = true,
                    Message = "Success",
                    Value = response
                };
            }
            //swallowing exceptions in order not to expose stacktrace to frontend
            catch (ArgumentException ex)
            {
                return Failure(ex.Message);
            }
            catch (OverflowException)
            {
                return Failure("One or more quantities are too large to process.");
            }
        }

        private static List<Recipe> BuildCustomRecipes(List<RecipeRequestDto> requests)
        {
            if(requests is null)
            {
                throw new ArgumentException($"Please provide a valid list of recipes");
            }
            var recipes = new List<Recipe>();
            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var request in requests)
            {
                var name = request.Name.Trim();
                if (!names.Add(name))
                {
                    throw new ArgumentException($"Recipe '{name}' is duplicated.");
                }

                var ingredients = request.Ingredients
                    .GroupBy(i => i.Name.Trim(), StringComparer.OrdinalIgnoreCase)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Sum(i => i.Quantity),
                        StringComparer.OrdinalIgnoreCase);

                recipes.Add(new Recipe
                {
                    Name = name,
                    Feeds = request.Feeds,
                    RequiredIngredients = ingredients
                });
            }

            return recipes;
        }

        private void ValidateRequest(GetIngredientsRequestDto request)
        {
            if(request is null)
                throw new ArgumentNullException(nameof(request));

            if (request.Ingredients is null || request.Ingredients.Count == 0)
                throw new ArgumentException("Please ensure you provide atleast one ingredient");

            foreach (var ingredient in request.Ingredients)
            {
                if (ingredient is null || string.IsNullOrWhiteSpace(ingredient.Name) ||
                    ingredient.Name.Equals("string", StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException("Please ensure you provide valid ingredient names");
                }

                if (ingredient.Quantity < 0)
                {
                    throw new ArgumentException("Please ensure your ingredient quantity is more than 0");
                }
            }

            if (request.UsePreExistingRecipes)
                return; // No need to go further, recipy will be insttiated from recipebook
            
            if (request.Recipes is null || request.Recipes.Count == 0)
            {
                throw new ArgumentException("Please provide at least one recipe or set UsePreExistingRecipes to false to use existing recipes");
            }

            foreach (var recipe in request.Recipes)
            {
                if (recipe is null || string.IsNullOrWhiteSpace(recipe.Name) ||
                    recipe.Name.Equals("string", StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException("All provided recipes need a valid name.");
                }

                if (recipe.Feeds <= 0)
                {
                    throw new ArgumentException($"Recipe '{recipe.Name}' must feed at least one person.");
                }

                if (recipe.Ingredients is null || recipe.Ingredients.Count == 0)
                {
                    throw new ArgumentException($"Recipe '{recipe.Name}' must contain at least one ingredient.");
                }

                foreach (var ingredient in recipe.Ingredients)
                {
                    if (ingredient is null || string.IsNullOrWhiteSpace(ingredient.Name) ||
                        ingredient.Name.Equals("string", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new ArgumentException($"Recipe '{recipe.Name}' contains an invalid ingredaient name.");
                    }

                    if (ingredient.Quantity <= 0)
                    {
                        throw new ArgumentException($"Recipe '{recipe.Name}' ingredients must have quantities more than zero.");
                    }
                }
            }
        }

        private static ProcessMealsResponseDto Failure(string message) 
        { // since this gets called alot, creating a function
            return new ProcessMealsResponseDto
            {
                IsSuccess = false,
                Message = message,
                Value = null
            };
        }
    }
}