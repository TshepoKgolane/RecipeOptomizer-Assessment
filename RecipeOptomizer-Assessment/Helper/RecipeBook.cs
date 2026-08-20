using RecipeOptomizer_Assessment.Business.Models;

namespace RecipeOptomizer_Assessment.Helper
{
    public static class RecipeBook
    {
        public static List<Recipe> GetRecipeData()
        {
            return new List<Recipe>
            {
                new Recipe
                {
                    Name = "Burger",
                    Feeds = 1,
                    RequiredIngredients = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
                    {
                        ["Meat"] = 1,
                        ["Lettuce"] = 1,
                        ["Tomato"] = 1,
                        ["Cheese"] = 1,
                        ["Dough"] = 1
                    }
                },
                new Recipe
                {
                    Name = "Pie",
                    Feeds = 1,
                    RequiredIngredients = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
                    {
                        ["Dough"] = 2,
                        ["Meat"] = 2
                    }
                },
                new Recipe
                {
                    Name = "Sandwich",
                    Feeds = 1,
                    RequiredIngredients = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
                    {
                        ["Dough"] = 1,
                        ["Cucumber"] = 1
                    }
                },
                new Recipe
                {
                    Name = "Pasta",
                    Feeds = 2,
                    RequiredIngredients = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
                    {
                        ["Dough"] = 2,
                        ["Tomato"] = 1,
                        ["Cheese"] = 2,
                        ["Meat"] = 1
                    }
                },
                new Recipe
                {
                    Name = "Salad",
                    Feeds = 3,
                    RequiredIngredients = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
                    {
                        ["Lettuce"] = 2,
                        ["Tomato"] = 2,
                        ["Cucumber"] = 1,
                        ["Cheese"] = 2,
                        ["Olives"] = 1
                    }
                },
                new Recipe
                {
                    Name = "Pizza",
                    Feeds = 4,
                    RequiredIngredients = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
                    {
                        ["Dough"] = 3,
                        ["Tomato"] = 2,
                        ["Cheese"] = 3,
                        ["Olives"] = 1
                    }
                }
            };
        }
    }
}
