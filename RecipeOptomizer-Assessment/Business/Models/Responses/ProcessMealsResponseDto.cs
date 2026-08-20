namespace RecipeOptomizer_Assessment.Business.Models.Responses
{
    public class ProcessMealsResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public MealPrepResponse Value { get; set; }
    }
}
