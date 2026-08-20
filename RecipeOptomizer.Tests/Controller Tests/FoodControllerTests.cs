using Microsoft.AspNetCore.Mvc;
using Moq;
using RecipeOptomizer_Assessment.Business.Models;
using RecipeOptomizer_Assessment.Business.Models.Requests;
using RecipeOptomizer_Assessment.Business.Models.Responses;
using RecipeOptomizer_Assessment.Business.Services;
using RecipeOptomizer_Assessment.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace RecipeOptomizer.Tests.Controller_Tests
{
    public class FoodControllerTests
    {
        private readonly Mock<IMealPrepService> _mealPrepServiceMock;
        private readonly FoodController _controller;
        public FoodControllerTests()
        {
            _mealPrepServiceMock = new Mock<IMealPrepService>();

            _controller = new FoodController(
                _mealPrepServiceMock.Object);
        }
        
        [Fact]
        public async Task GenerateOptomisedMealPrep_NullRequest_ReturnsBadRequest()
        {
            // Arrange
            GetIngredientsRequestDto request = null!;

            // Act
            var result = await _controller.GenerateOptomisedMealPrep(request);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);

            Assert.Equal(400, badRequest.StatusCode);

            // should neeve be called
            _mealPrepServiceMock.Verify(
                x => x.MealProcess(It.IsAny<GetIngredientsRequestDto>()),
                Times.Never);
        }

        [Fact]
        public async Task GenerateOptomisedMealPrep_NullIngredients_ReturnsBadRequest()
        {
            // Arrange
            var request = new GetIngredientsRequestDto
            {
                Ingredients = null!
            };

            // Act
            var result = await _controller.GenerateOptomisedMealPrep(request);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);

            Assert.Equal(400, badRequest.StatusCode);

            _mealPrepServiceMock.Verify(
                x => x.MealProcess(It.IsAny<GetIngredientsRequestDto>()),
                Times.Never);
        }

        [Fact]
        public async Task GenerateOptomisedMealPrep_EmptyIngredients_ReturnsBadRequest()
        {
            // Arrange
            var request = new GetIngredientsRequestDto
            {
                Ingredients = new List<Ingredient>()
            };

            // Act
            var result = await _controller.GenerateOptomisedMealPrep(request);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);

            Assert.Equal(400, badRequest.StatusCode);

            _mealPrepServiceMock.Verify(
                x => x.MealProcess(It.IsAny<GetIngredientsRequestDto>()),
                Times.Never);
        }
         
        [Fact]
        public async Task GenerateOptomisedMealPrep_ValidRequest_ReturnsOk()
        {
            // Arrange
            var request = CreateValidRequest();

            var serviceResponse = new ProcessMealsResponseDto
            {
                IsSuccess = true,
                Message = "Success",
                Value = null
            };

            _mealPrepServiceMock
                .Setup(x => x.MealProcess(request))
                .Returns(serviceResponse);

            // Act
            var result = await _controller.GenerateOptomisedMealPrep(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            Assert.Equal(200, okResult.StatusCode);
            Assert.Same(serviceResponse, okResult.Value);

            _mealPrepServiceMock.Verify(
                x => x.MealProcess(request),
                Times.Once);
        }
         
        [Fact]
        public async Task GenerateOptomisedMealPrep_ServiceFailure_ReturnsBadRequest()
        {
            // Arrange
            var request = CreateValidRequest();

            var serviceResponse = new ProcessMealsResponseDto
            {
                IsSuccess = false,
                Message = "No meals could be generated.",
                Value = null
            };

            _mealPrepServiceMock
                .Setup(x => x.MealProcess(request))
                .Returns(serviceResponse);

            // Act
            var result = await _controller.GenerateOptomisedMealPrep(request);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);

            Assert.Equal(400, badRequest.StatusCode);
            Assert.Same(serviceResponse, badRequest.Value);

            _mealPrepServiceMock.Verify(
                x => x.MealProcess(request),
                Times.Once);
        }
         
        [Fact]
        public async Task GenerateOptomisedMealPrep_ServiceFailure_ReturnsServiceMessage()
        {
            // Arrange
            var request = CreateValidRequest();

            var serviceResponse = new ProcessMealsResponseDto
            {
                IsSuccess = false,
                Message = "Please provide at least one ingredient with a quantity greater than zero.",
                Value = null
            };

            _mealPrepServiceMock
                .Setup(x => x.MealProcess(request))
                .Returns(serviceResponse);

            // Act
            var result = await _controller.GenerateOptomisedMealPrep(request);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);

            var response = Assert.IsType<ProcessMealsResponseDto>(
                badRequest.Value);

            Assert.False(response.IsSuccess);
            Assert.Equal(
                "Please provide at least one ingredient with a quantity greater than zero.",
                response.Message);
            Assert.Null(response.Value);
        }
         
        [Fact]
        public async Task GenerateOptomisedMealPrep_PassesRequestToService()
        {
            // Arrange
            var request = CreateValidRequest();

            var serviceResponse = new ProcessMealsResponseDto
            {
                IsSuccess = true,
                Message = "Success"
            };

            _mealPrepServiceMock
                .Setup(x => x.MealProcess(It.IsAny<GetIngredientsRequestDto>()))
                .Returns(serviceResponse);

            // Act
            await _controller.GenerateOptomisedMealPrep(request);

            // Assert
            _mealPrepServiceMock.Verify(
                x => x.MealProcess(
                    It.Is<GetIngredientsRequestDto>(r =>
                        r == request)),
                Times.Once);
        }

        [Fact]
        public async Task GenerateOptomisedMealPrep_ValidRequest_CallsServiceOnlyOnce()
        {
            // Arrange
            var request = CreateValidRequest();

            _mealPrepServiceMock
                .Setup(x => x.MealProcess(It.IsAny<GetIngredientsRequestDto>()))
                .Returns(new ProcessMealsResponseDto
                {
                    IsSuccess = true,
                    Message = "Success"
                });

            // Act
            await _controller.GenerateOptomisedMealPrep(request);

            // Assert
            _mealPrepServiceMock.Verify(
                x => x.MealProcess(It.IsAny<GetIngredientsRequestDto>()),
                Times.Once);
        }

       

        private static GetIngredientsRequestDto CreateValidRequest()
        {
            return new GetIngredientsRequestDto
            {
                Ingredients = new List<Ingredient>
                {
                    new Ingredient
                    {
                        Name = "Chicken",
                        Quantity = 10
                    },
                    new Ingredient
                    {
                        Name = "Rice",
                        Quantity = 10
                    }
                },

                Recipes = new List<RecipeRequestDto>(),

                UsePreExistingRecipes = true
            };
        }
    }
}
