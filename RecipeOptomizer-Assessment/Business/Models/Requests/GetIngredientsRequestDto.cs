namespace RecipeOptomizer_Assessment.Business.Models.Requests
{
    public class GetIngredientsRequestDto
    {
        public List<Ingredient> Ingredients { get; set; }
        public List<RecipeRequestDto>? Recipes { get; set; }
        public bool UsePreExistingRecipes { get; set; } = true;
    }
}
