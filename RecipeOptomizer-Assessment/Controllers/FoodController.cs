using Microsoft.AspNetCore.Mvc;
using RecipeOptomizer_Assessment.Business.Models.Requests;
using RecipeOptomizer_Assessment.Business.Services; 

namespace RecipeOptomizer_Assessment.Controllers
{
    [ApiController]
    [Route("[controller]/")]
    public class FoodController : Controller
    {
        private readonly IMealPrepService _mealPrepService;

        public FoodController(IMealPrepService mealPrepService)
        {
            _mealPrepService = mealPrepService;
        }
        [Route("GenerateOptomisedMealPrep")]
        [HttpPost]
        public async Task<IActionResult> GenerateOptomisedMealPrep([FromBody]GetIngredientsRequestDto request)
        {
            // validation
            if (request == null || request.Ingredients == null || !request.Ingredients.Any())
                return BadRequest(new { isSuccess = false, Message = "Please provide a list of ingredients." });

           var result = _mealPrepService.MealProcess(request); // exceptions caught within the service, not need to catch here
            if(result.IsSuccess)
                return Ok(result);
            else
                return BadRequest(result);

        }
    }
}
