namespace RecipeOptomizer_Assessment.Business.Models.Requests
{
    public class RecipeRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public int Feeds { get; set; } = 0;
        public List<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
    }
}
