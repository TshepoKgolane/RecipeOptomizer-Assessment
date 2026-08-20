using RecipeOptomizer_Assessment.Business.Models;
using RecipeOptomizer_Assessment.Business.Models.Requests;
using RecipeOptomizer_Assessment.Business.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecipeOptomizer.Tests.Business_logic_tests
{
    public class MealPrepServiceTests
    {
        private readonly MealPrepService _sut;
        public MealPrepServiceTests()
        {
            _sut = new MealPrepService();
        }
        
        [Fact]
        public void MealProcess_NullRequest_ReturnsFailureObject()
        {
            // Arrange
            GetIngredientsRequestDto request = null!;

            // Act
            var exception = _sut.MealProcess(request);

            // Assert
            Assert.False(exception.IsSuccess); 
            Assert.NotEmpty(exception.Message);
        }

        [Fact]
        public void MealProcess_NullIngredients_ReturnsFailure()
        {
            // Arrange
            var request = new GetIngredientsRequestDto
            {
                Ingredients = null,
                UsePreExistingRecipes = false
            };

            // Act
            var result = _sut.MealProcess(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Message);
        }

        [Fact]
        public void MealProcess_EmptyIngredients_ReturnsFailure()
        {
            // Arrange
            var request = new GetIngredientsRequestDto
            {
                Ingredients = new List<Ingredient>
                {
                    
                },
                UsePreExistingRecipes = false
            };

            // Act
            var result = _sut.MealProcess(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Message);
        }
        [Fact]
        public void MealProcess_InvalidIngredientName_ReturnsFailure()
        {
            // Arrange
            var request = CreateValidRequest();

            request.Ingredients = new List<Ingredient>
            {
                new Ingredient
                {
                    Name = "",
                    Quantity = 200
                }
            };

            // Act
            var result = _sut.MealProcess(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(
                "Please ensure you provide valid ingredient names",
                result.Message);
        }

        [Fact]
        public void MealProcess_StringIngredientName_ReturnsFailure()
        {
            // Arrange
            var request = CreateValidRequest();

            request.Ingredients = new List<Ingredient>
            {
                new Ingredient
                {
                    Name = "string",
                    Quantity = 2
                }
            };

            // Act
            var result = _sut.MealProcess(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(
                "Please ensure you provide valid ingredient names",
                result.Message);
        }

        [Fact]
        public void MealProcess_NegativeIngredientQuantity_ReturnsFailure()
        {
            // Arrange
            var request = CreateValidRequest();

            request.Ingredients = new List<Ingredient>
            {
                new Ingredient
                {
                    Name = "Chakalaka",
                    Quantity = -1
                }
            };

            // Act
            var result = _sut.MealProcess(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(
                "Please ensure your ingredient quantity is more than 0",
                result.Message);
        }

        [Fact]
        public void MealProcess_OnlyZeroQuantityIngredients_ReturnsFailure()
        {
            // Arrange
            var request = CreateValidRequest();

            request.Ingredients = new List<Ingredient>
            {
                new Ingredient
                {
                    Name = "Chakalaka",
                    Quantity = 0
                },
                new Ingredient
                {
                    Name = "Rice",
                    Quantity = 0
                }
            };

            // Act
            var result = _sut.MealProcess(request);

            // Assert
            Assert.False(result.IsSuccess);

            Assert.Equal(
                "Please provide at least one ingredient with a quantity greater than zero.",
                result.Message);

            Assert.Null(result.Value);
        }

        [Fact]
        public void MealProcess_MixOfZeroAndPositiveQuantities_IgnoresZeroQuantity()
        {
            // Arrange
            var request = CreateValidRequest();

            request.Ingredients = new List<Ingredient>
            {
                new Ingredient
                {
                    Name = "Chakalaka",
                    Quantity = 0
                },
                new Ingredient
                {
                    Name = "Rice",
                    Quantity = 5
                }
            };

            // Act
            var result = _sut.MealProcess(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Success", result.Message);
            Assert.NotNull(result.Value);
        }


        [Fact]
        public void MealProcess_CustomRecipes_NullRecipes_ReturnsFailure()
        {
            // Arrange
            var request = new GetIngredientsRequestDto
            {
                Ingredients = new List<Ingredient>
                {
                    new Ingredient
                    {
                        Name = "Chakalaka",
                        Quantity = 5
                    }
                },
                UsePreExistingRecipes = false,
                Recipes = null!
            };

            // Act
            var result = _sut.MealProcess(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(
                "Please provide at least one recipe or set UsePreExistingRecipes to false to use existing recipes",
                result.Message);
        }

        [Fact]
        public void MealProcess_CustomRecipes_EmptyRecipes_ReturnsFailure()
        {
            // Arrange
            var request = new GetIngredientsRequestDto
            {
                Ingredients = new List<Ingredient>
                {
                    new Ingredient
                    {
                        Name = "Chakalaka",
                        Quantity = 5
                    }
                },
                UsePreExistingRecipes = false,
                Recipes = new List<RecipeRequestDto>()
            };

            // Act
            var result = _sut.MealProcess(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(
                "Please provide at least one recipe or set UsePreExistingRecipes to false to use existing recipes",
                result.Message);
        }

        [Fact]
        public void MealProcess_CustomRecipe_InvalidName_ReturnsFailure()
        {
            // Arrange
            var request = CreateValidRequest();

            request.Recipes = new List<RecipeRequestDto>
            {
                new RecipeRequestDto
                {
                    Name = "",
                    Feeds = 2,
                    Ingredients = new List<Ingredient>
                    {
                        new Ingredient
                        {
                            Name = "Chakalaka",
                            Quantity = 1
                        }
                    }
                }
            };

            // Act
            var result = _sut.MealProcess(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(
                "All provided recipes need a valid name.",
                result.Message);
        }

        [Fact]
        public void MealProcess_CustomRecipe_StringName_ReturnsFailure()
        {
            // Arrange
            var request = CreateValidRequest();

            request.Recipes = new List<RecipeRequestDto>
            {
                new RecipeRequestDto
                {
                    Name = "string",
                    Feeds = 2,
                    Ingredients = new List<Ingredient>
                    {
                        new Ingredient
                        {
                            Name = "Chakalaka",
                            Quantity = 1
                        }
                    }
                }
            };

            // Act
            var result = _sut.MealProcess(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(
                "All provided recipes need a valid name.",
                result.Message);
        }

        [Fact]
        public void MealProcess_CustomRecipe_ZeroFeeds_ReturnsFailure()
        {
            // Arrange
            var request = CreateValidRequest();

            request.Recipes = new List<RecipeRequestDto>
            {
                new RecipeRequestDto
                {
                    Name = "Chakalaka Rice",
                    Feeds = 0,
                    Ingredients = new List<Ingredient>
                    {
                        new Ingredient
                        {
                            Name = "Chakalaka",
                            Quantity = 1
                        }
                    }
                }
            };

            // Act
            var result = _sut.MealProcess(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(
                "Recipe 'Chakalaka Rice' must feed at least one person.",
                result.Message);
        }

        [Fact]
        public void MealProcess_CustomRecipe_NegativeFeeds_ReturnsFailure()
        {
            // Arrange
            var request = CreateValidRequest();

            request.Recipes = new List<RecipeRequestDto>
            {
                new RecipeRequestDto
                {
                    Name = "Chakalaka Rice",
                    Feeds = -1,
                    Ingredients = new List<Ingredient>
                    {
                        new Ingredient
                        {
                            Name = "Chakalaka",
                            Quantity = 1
                        }
                    }
                }
            };

            // Act
            var result = _sut.MealProcess(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(
                "Recipe 'Chakalaka Rice' must feed at least one person.",
                result.Message);
        }

        [Fact]
        public void MealProcess_CustomRecipe_NoIngredients_ReturnsFailure()
        {
            // Arrange
            var request = CreateValidRequest();

            request.Recipes = new List<RecipeRequestDto>
            {
                new RecipeRequestDto
                {
                    Name = "Chakalaka Rice",
                    Feeds = 2,
                    Ingredients = new List<Ingredient>()
                }
            };

            // Act
            var result = _sut.MealProcess(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(
                "Recipe 'Chakalaka Rice' must contain at least one ingredient.",
                result.Message);
        }

        [Fact]
        public void MealProcess_CustomRecipe_NullIngredients_ReturnsFailure()
        {
            // Arrange
            var request = CreateValidRequest();

            request.Recipes = new List<RecipeRequestDto>
            {
                new RecipeRequestDto
                {
                    Name = "Chakalaka Rice",
                    Feeds = 2,
                    Ingredients = null!
                }
            };

            // Act
            var result = _sut.MealProcess(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(
                "Recipe 'Chakalaka Rice' must contain at least one ingredient.",
                result.Message);
        }


        [Fact]
        public void MealProcess_CustomRecipe_StringIngredientName_ReturnsFailure()
        {
            // Arrange
            var request = CreateValidRequest();

            request.Recipes = new List<RecipeRequestDto>
            {
                new RecipeRequestDto
                {
                    Name = "Chakalaka Rice",
                    Feeds = 2,
                    Ingredients = new List<Ingredient>
                    {
                        new Ingredient
                        {
                            Name = "string",
                            Quantity = 1
                        }
                    }
                }
            };

            // Act
            var result = _sut.MealProcess(request);

            // Assert
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public void MealProcess_CustomRecipe_ZeroIngredientQuantity_ReturnsFailure()
        {
            // Arrange
            var request = CreateValidRequest();

            request.Recipes = new List<RecipeRequestDto>
            {
                new RecipeRequestDto
                {
                    Name = "Chakalaka Rice",
                    Feeds = 2,
                    Ingredients = new List<Ingredient>
                    {
                        new Ingredient
                        {
                            Name = "Chakalaka",
                            Quantity = 0
                        }
                    }
                }
            };

            // Act
            var result = _sut.MealProcess(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(
                "Recipe 'Chakalaka Rice' ingredients must have quantities more than zero.",
                result.Message);
        }


        [Fact]
        public void MealProcess_DuplicateCustomRecipeNames_ReturnsFailure()
        {
            // Arrange
            var request = CreateValidRequest();

            request.Recipes = new List<RecipeRequestDto>
            {
                new RecipeRequestDto
                {
                    Name = "Chakalaka Rice",
                    Feeds = 2,
                    Ingredients = new List<Ingredient>
                    {
                        new Ingredient
                        {
                            Name = "Chakalaka",
                            Quantity = 1
                        },
                        new Ingredient
                        {
                            Name = "Rice",
                            Quantity = 1
                        }
                    }
                },
                new RecipeRequestDto
                {
                    Name = "Chakalaka Rice",
                    Feeds = 2,
                    Ingredients = new List<Ingredient>
                    {
                        new Ingredient
                        {
                            Name = "Chakalaka",
                            Quantity = 1
                        }
                    }
                }
            };

            // Act
            var result = _sut.MealProcess(request);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(
                "Recipe 'Chakalaka Rice' is duplicated.",
                result.Message);
        }

       

        [Fact]
        public void MealProcess_ValidCustomRecipe_ReturnsSuccess()
        {
            // Arrange
            var request = new GetIngredientsRequestDto
            {
                UsePreExistingRecipes = false,

                Ingredients = new List<Ingredient>
                {
                    new Ingredient
                    {
                        Name = "Chakalaka",
                        Quantity = 10
                    },
                    new Ingredient
                    {
                        Name = "Rice",
                        Quantity = 10
                    }
                },

                Recipes = new List<RecipeRequestDto>
                {
                    new RecipeRequestDto
                    {
                        Name = "Chakalaka Rice",
                        Feeds = 2,
                        Ingredients = new List<Ingredient>
                        {
                            new Ingredient
                            {
                                Name = "Chakalaka",
                                Quantity = 2
                            },
                            new Ingredient
                            {
                                Name = "Rice",
                                Quantity = 2
                            }
                        }
                    }
                }
            };

            // Act
            var result = _sut.MealProcess(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Success", result.Message);
            Assert.NotNull(result.Value);
        }

        [Fact]
        public void MealProcess_MultipleValidCustomRecipes_ReturnsSuccess()
        {
            // Arrange
            var request = new GetIngredientsRequestDto
            {
                UsePreExistingRecipes = false,

                Ingredients = new List<Ingredient>
                {
                    new Ingredient
                    {
                        Name = "Chakalaka",
                        Quantity = 20
                    },
                    new Ingredient
                    {
                        Name = "Rice",
                        Quantity = 20
                    },
                    new Ingredient
                    {
                        Name = "Vegetables",
                        Quantity = 20
                    }
                },

                Recipes = new List<RecipeRequestDto>
                {
                    new RecipeRequestDto
                    {
                        Name = "Chakalaka Rice",
                        Feeds = 2,
                        Ingredients = new List<Ingredient>
                        {
                            new Ingredient
                            {
                                Name = "Chakalaka",
                                Quantity = 2
                            },
                            new Ingredient
                            {
                                Name = "Rice",
                                Quantity = 2
                            }
                        }
                    },

                    new RecipeRequestDto
                    {
                        Name = "Chakalaka Vegetables",
                        Feeds = 2,
                        Ingredients = new List<Ingredient>
                        {
                            new Ingredient
                            {
                                Name = "Chakalaka",
                                Quantity = 2
                            },
                            new Ingredient
                            {
                                Name = "Vegetables",
                                Quantity = 2
                            }
                        }
                    }
                }
            };

            // Act
            var result = _sut.MealProcess(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Success", result.Message);
            Assert.NotNull(result.Value);
        }

        [Fact]
        public void MealProcess_CustomRecipe_DuplicateIngredients_AreAggregated()
        {
            // Arrange
            var request = new GetIngredientsRequestDto
            {
                UsePreExistingRecipes = false,

                Ingredients = new List<Ingredient>
                {
                    new Ingredient
                    {
                        Name = "Chakalaka",
                        Quantity = 10
                    },
                    new Ingredient
                    {
                        Name = "Rice",
                        Quantity = 10
                    }
                },

                Recipes = new List<RecipeRequestDto>
                {
                    new RecipeRequestDto
                    {
                        Name = "Chakalaka Rice",
                        Feeds = 2,
                        Ingredients = new List<Ingredient>
                        {
                            new Ingredient
                            {
                                Name = "Chakalaka",
                                Quantity = 1
                            },
                            new Ingredient
                            {
                                Name = " Chakalaka ",
                                Quantity = 1
                            },
                            new Ingredient
                            {
                                Name = "Rice",
                                Quantity = 2
                            }
                        }
                    }
                }
            };

            // Act
            var result = _sut.MealProcess(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
        }
        [Fact]
        public void MealProcess_UsePreExistingRecipes_DoesNotRequireCustomRecipes()
        {
            // Arrange
            var request = new GetIngredientsRequestDto
            {
                UsePreExistingRecipes = true,

                Ingredients = new List<Ingredient>
                {
                    new Ingredient
                    {
                        Name = "Chakalaka",
                        Quantity = 10
                    },
                    new Ingredient
                    {
                        Name = "Rice",
                        Quantity = 10
                    }
                },

                Recipes = null
            };

            // Act
            var result = _sut.MealProcess(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Success", result.Message);
            Assert.NotNull(result.Value);
        }

        private static GetIngredientsRequestDto CreateValidRequest()
        {
            return new GetIngredientsRequestDto
            {
                UsePreExistingRecipes = false,

                Ingredients = new List<Ingredient>
                {
                    new Ingredient
                    {
                        Name = "Chakalaka",
                        Quantity = 10
                    },
                    new Ingredient
                    {
                        Name = "Rice",
                        Quantity = 10
                    }
                },

                Recipes = new List<RecipeRequestDto>
                {
                    new RecipeRequestDto
                    {
                        Name = "Chakalaka Rice",
                        Feeds = 2,
                        Ingredients = new List<Ingredient>
                        {
                            new Ingredient
                            {
                                Name = "Chakalaka",
                                Quantity = 2
                            },
                            new Ingredient
                            {
                                Name = "Rice",
                                Quantity = 2
                            }
                        }
                    }
                }
            };
        }
    }
}
