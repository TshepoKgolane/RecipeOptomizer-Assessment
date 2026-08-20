namespace RecipeOptomizer_Assessment.Business.Models.Responses
{
    public class MealPrepResponse
    {
        public int totalPeopleFed { get; set; }
        public List<MealPlan> MealPlan { get; set; } = new List<MealPlan>();
        public Dictionary<string, int> ingredientsUsed { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> ingredientsRemaining { get; set; } = new Dictionary<string, int>();
    }
}
