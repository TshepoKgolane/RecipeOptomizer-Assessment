using RecipeOptomizer_Assessment.Business.Models.Requests;
using RecipeOptomizer_Assessment.Business.Models.Responses;

namespace RecipeOptomizer_Assessment.Business.Services
{
    public interface IMealPrepService
    {
        public ProcessMealsResponseDto MealProcess(GetIngredientsRequestDto request);
    }
}
