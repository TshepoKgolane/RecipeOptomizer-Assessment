namespace RecipeOptomizer_Assessment.Business.Models
{
    public class Recipe
    {
        public string Name { get; set; } = string.Empty;
        public Dictionary<string, int> RequiredIngredients { get; set; } = new Dictionary<string, int>();
        public int Feeds { get; set; } =  0;
    }
}
