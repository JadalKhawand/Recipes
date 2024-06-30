using AutoFixture.AutoNSubstitute;
using AutoFixture;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cme.Recipes.Models;
using Cme.Recipes.Services;
using Cme.Recipes.Controllers;
using Cme.Recipes.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System.Web.Http.Results;
using RecipeApp.Domain.Entities;
using Microsoft.AspNetCore.JsonPatch;

namespace CmeRecipe.Test.Controllers
{
    public class RecipeControllerTest
    {
        [Fact]
         public async Task RecipeApiController_GetAllRecipes_ReturnsOK()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());

            var fakeRecipes = fixture.CreateMany<AllRecipesOutputDto>(3).ToList();


            var recipeService = Substitute.For<IRecipeService>();
            var FileUploadService = Substitute.For<IFileUploadService>();
            recipeService.GetAllRecipesPaginated(1,3).Returns(fakeRecipes);

            var controller = new RecipeAPIController(recipeService,FileUploadService);

            // Act
            var result = await controller.GetAllRecipes("","",1,3);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var okObjectResult = (OkObjectResult)result;
        }

        [Fact]
        public void RecipeApiController_GetAllRecipes_ReturnsNotFound()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());

            var fakeRecipes = fixture.CreateMany<AllRecipesOutputDto>(3).ToList();


            var recipeService = Substitute.For<IRecipeService>();
            var FileUploadService = Substitute.For<IFileUploadService>();
            recipeService.GetAllRecipesPaginated(2, 3).Returns(fakeRecipes);

            var controller = new RecipeAPIController(recipeService, FileUploadService);

            // Act
            var result = controller.GetAllRecipes();

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        

        [Fact]
        public async Task RecipeApiController_GetAllRecipes_ReturnsBadRequestForNegativePageSize()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());

            var recipeService = Substitute.For<IRecipeService>();
            var fileUploadService = Substitute.For<IFileUploadService>();

            var controller = new RecipeAPIController(recipeService, fileUploadService);

            // Act
            var result = await controller.GetAllRecipes(pageNumber: 1, pageSize: -1); 

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task RecipeApiController_GetAllRecipes_ReturnsBadRequestForNegativePageNumber()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());

            var recipeService = Substitute.For<IRecipeService>();
            var fileUploadService = Substitute.For<IFileUploadService>();

            var controller = new RecipeAPIController(recipeService, fileUploadService);

            // Act
            var result = await controller.GetAllRecipes(pageNumber: -1, pageSize: 10); 

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
        [Fact]
        public void RecipeApiController_GetAllIngredients_ReturnsOK()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());

            var recipeId = Guid.NewGuid();
            var fakeIngredients = fixture.CreateMany<IngredientOutputDto>(3).ToList();

            var recipeService = Substitute.For<IRecipeService>();
            var fileUploadService = Substitute.For<IFileUploadService>();

            recipeService.GetIngredients(recipeId).Returns(fakeIngredients);

            var controller = new RecipeAPIController(recipeService, fileUploadService);

            // Act
            var result = controller.GetAllIngredients(recipeId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var ingredients = Assert.IsAssignableFrom<List<IngredientOutputDto>>(okResult.Value);
            Assert.Equal(3, ingredients.Count);
        }

        [Fact]
        public void RecipeApiController_GetAllIngredients_ReturnsNotFound()
        {
            // Arrange
            var recipeId = Guid.NewGuid();

            var recipeService = Substitute.For<IRecipeService>();
            var fileUploadService = Substitute.For<IFileUploadService>();

            recipeService.GetIngredients(recipeId).Returns((List<IngredientOutputDto>)null);

            var controller = new RecipeAPIController(recipeService, fileUploadService);

            // Act
            var result = controller.GetAllIngredients(recipeId);

            // Assert
            Assert.IsType<Microsoft.AspNetCore.Mvc.NotFoundResult>(result);
        }

        [Fact]
        public void RecipeApiController_GetAllIngredients_ReturnsServerError()
        {
            // Arrange
            var recipeId = Guid.NewGuid();

            var recipeService = Substitute.For<IRecipeService>();
            var fileUploadService = Substitute.For<IFileUploadService>();

            recipeService.GetIngredients(recipeId).Returns(x => { throw new Exception("Test exception"); });

            var controller = new RecipeAPIController(recipeService, fileUploadService);

            // Act
            var result = controller.GetAllIngredients(recipeId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
            Assert.Equal("Test exception", statusCodeResult.Value);
        }

        [Fact]
        public void RecipeApiController_GetRecipe_ReturnsOK()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());

            var recipeId = Guid.NewGuid();
            var fakeRecipe = fixture.Create<RecipeOutputDto>();

            var recipeService = Substitute.For<IRecipeService>();
            var fileUploadService = Substitute.For<IFileUploadService>();

            recipeService.GetRecipe(recipeId).Returns(fakeRecipe);

            var controller = new RecipeAPIController(recipeService, fileUploadService);

            // Act
            var result = controller.GetRecipe(recipeId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var recipe = Assert.IsAssignableFrom<RecipeOutputDto>(okResult.Value);
            Assert.Equal(fakeRecipe.RecipeId, recipe.RecipeId);
        }

        [Fact]
        public void RecipeApiController_GetRecipe_ReturnsNotFound()
        {
            // Arrange
            var recipeId = Guid.NewGuid();

            var recipeService = Substitute.For<IRecipeService>();
            var fileUploadService = Substitute.For<IFileUploadService>();

            recipeService.GetRecipe(recipeId).Returns((RecipeOutputDto)null);

            var controller = new RecipeAPIController(recipeService, fileUploadService);

            // Act
            var result = controller.GetRecipe(recipeId);

            // Assert
            Assert.IsType<Microsoft.AspNetCore.Mvc.NotFoundResult>(result);
        }

        [Fact]
        public void RecipeApiController_GetRecipe_ReturnsServerError()
        {
            // Arrange
            var recipeId = Guid.NewGuid();

            var recipeService = Substitute.For<IRecipeService>();
            var fileUploadService = Substitute.For<IFileUploadService>();

            recipeService.GetRecipe(recipeId).Returns(x => { throw new Exception("Test exception"); });

            var controller = new RecipeAPIController(recipeService, fileUploadService);

            // Act
            var result = controller.GetRecipe(recipeId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
            Assert.Equal("Test exception", statusCodeResult.Value);
        }

        [Fact]
        public async void RecipeApiController_CreateRecipe_ReturnsActionResult()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());

            var recipeDto = fixture.Create<RecipeInputDto>();
            var createdRecipe = fixture.Create<Recipe>();
            var recipeService = Substitute.For<IRecipeService>();
            var FileUploadService = Substitute.For<IFileUploadService>();


            recipeService.CreateRecipe(recipeDto).Returns(createdRecipe);

            var controller = new RecipeAPIController(recipeService, FileUploadService);


            // Act
            var result = await controller.CreateRecipe(recipeDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(RecipeAPIController.CreateRecipe), createdAtActionResult.ActionName);
            Assert.Equal(StatusCodes.Status201Created, createdAtActionResult.StatusCode);
            var createdRecipeResult = Assert.IsType<Recipe>(createdAtActionResult.Value);

        }
        [Fact]
        public async Task RecipeApiController_CreateRecipe_ReturnsBadRequest()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());

            var recipeService = Substitute.For<IRecipeService>();
            var FileUploadService = Substitute.For<IFileUploadService>();


            var controller = new RecipeAPIController(recipeService, FileUploadService);

            // Act
            var result = await controller.CreateRecipe(null);

            // Assert
            Assert.IsType<Microsoft.AspNetCore.Mvc.BadRequestResult>(result.Result);
        }

        [Fact]
        public async Task RecipeApiController_CreateIngredient_ReturnsCreated()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());

            var recipeId = Guid.NewGuid();
            var fakeIngredientsDto = fixture.CreateMany<IngredientInputDto>(3).ToList();
            var fakeIngredients = fixture.CreateMany<Ingredient>(3).ToList();

            var recipeService = Substitute.For<IRecipeService>();
            var fileUploadService = Substitute.For<IFileUploadService>();

            recipeService.CreateIngredient(recipeId, fakeIngredientsDto).Returns(fakeIngredients);

            var controller = new RecipeAPIController(recipeService, fileUploadService);

            // Act
            var result = await controller.CreateIngredient(recipeId, fakeIngredientsDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var ingredients = Assert.IsAssignableFrom<List<Ingredient>>(createdResult.Value);
            Assert.Equal(3, ingredients.Count);
        }

        [Fact]
        public async Task RecipeApiController_CreateIngredient_ReturnsBadRequest()
        {
            // Arrange
            var recipeId = Guid.NewGuid();

            var recipeService = Substitute.For<IRecipeService>();
            var fileUploadService = Substitute.For<IFileUploadService>();

            var controller = new RecipeAPIController(recipeService, fileUploadService);

            // Act
            var result = await controller.CreateIngredient(recipeId, null);

            // Assert
            Assert.IsType<Microsoft.AspNetCore.Mvc.BadRequestResult>(result.Result);
        }

        [Fact]
        public async Task RecipeApiController_CreateIngredient_ReturnsServerError()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());

            var recipeId = Guid.NewGuid();
            var fakeIngredientsDto = fixture.CreateMany<IngredientInputDto>(3).ToList();

            var recipeService = Substitute.For<IRecipeService>();
            var fileUploadService = Substitute.For<IFileUploadService>();

            recipeService.CreateIngredient(recipeId, fakeIngredientsDto).Returns((List<Ingredient>)null);

            var controller = new RecipeAPIController(recipeService, fileUploadService);

            // Act
            var result = await controller.CreateIngredient(recipeId, fakeIngredientsDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
            Assert.Equal("Error creating ingredient", statusCodeResult.Value);
        }

        [Fact]
        public async Task RecipeApiController_UpdateIngredient_ReturnsUpdatedIngredient()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());

            var recipeId = Guid.NewGuid();
            var ingredientId = Guid.NewGuid();
            var fakeIngredientDto = fixture.Create<IngredientInputDto>();
            var fakeIngredient = fixture.Create<IngredientOutputDto>();
            var ingredientToUpdate = fixture.Create<Ingredient>();
            ingredientToUpdate.IngredientId = fakeIngredient.IngredientId;

            var recipeService = Substitute.For<IRecipeService>();
            var fileUploadService = Substitute.For<IFileUploadService>();

            recipeService.GetIngredient(ingredientId).Returns(fakeIngredient);
            recipeService.UpdateIngredient(ingredientId, fakeIngredientDto).Returns(ingredientToUpdate);

            var controller = new RecipeAPIController(recipeService, fileUploadService);

            // Act
            var result = await controller.UpdateIngredient(recipeId, ingredientId, fakeIngredientDto);

            // Assert
            var okResult = Assert.IsType<ActionResult<Ingredient>>(result);
            var ingredient = Assert.IsAssignableFrom<Ingredient>(okResult.Value);
            Assert.Equal(fakeIngredient.IngredientId, ingredient.IngredientId);
        }

        [Fact]
        public async Task RecipeApiController_UpdateIngredient_ReturnsBadRequest()
        {
            // Arrange
            var recipeId = Guid.NewGuid();
            var ingredientId = Guid.NewGuid();

            var recipeService = Substitute.For<IRecipeService>();
            var fileUploadService = Substitute.For<IFileUploadService>();

            var controller = new RecipeAPIController(recipeService, fileUploadService);

            // Act
            var result = await controller.UpdateIngredient(recipeId, ingredientId, null);

            // Assert
            Assert.IsType<Microsoft.AspNetCore.Mvc.BadRequestResult>(result.Result);
        }

        [Fact]
        public async Task RecipeApiController_UpdateIngredient_ReturnsNotFound()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());

            var recipeId = Guid.NewGuid();
            var ingredientId = Guid.NewGuid();
            var fakeIngredientDto = fixture.Create<IngredientInputDto>();

            var recipeService = Substitute.For<IRecipeService>();
            var fileUploadService = Substitute.For<IFileUploadService>();

            recipeService.GetIngredient(ingredientId).Returns((IngredientOutputDto)null);

            var controller = new RecipeAPIController(recipeService, fileUploadService);

            // Act
            var result = await controller.UpdateIngredient(recipeId, ingredientId, fakeIngredientDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal($"Ingredient with ID = {ingredientId} was not found", notFoundResult.Value);
        }


        [Fact]
        public async void RecipeApiController_DeleteRecipe_ReturnsNoContent()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());

            var id = Guid.NewGuid();
            var recipeToDelete = fixture.Create<RecipeOutputDto>();
            var recipeService = Substitute.For<IRecipeService>();
            var FileUploadService = Substitute.For<IFileUploadService>();

            recipeService.GetRecipe(id).Returns(recipeToDelete);
            recipeService.DeleteRecipe(id).Returns(true);

            var controller = new RecipeAPIController(recipeService, FileUploadService);

            // Act
            var result = await controller.DeleteRecipe(id);

            // Assert
            Assert.IsType<NoContentResult>(result.Result);
        }

        [Fact]
        public async void RecipeApiController_DeleteRecipe_NonExistingRecipe_ReturnsNotFound()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());

            var id = Guid.NewGuid();
            var recipeService = Substitute.For<IRecipeService>();
            var FileUploadService = Substitute.For<IFileUploadService>();

            recipeService.GetRecipe(id).Returns((RecipeOutputDto)null);

            var controller = new RecipeAPIController(recipeService, FileUploadService);


            // Act
            var result = await controller.DeleteRecipe(id);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async void RecipeApiController_DeleteRecipe_FailureToDelete_ReturnsBadRequest()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());

            var id = Guid.NewGuid();
            var recipeToDelete = fixture.Create<RecipeOutputDto>();
            var recipeService = Substitute.For<IRecipeService>();
            var FileUploadService = Substitute.For<IFileUploadService>();

            recipeService.GetRecipe(id).Returns(recipeToDelete);
            recipeService.DeleteRecipe(id).Returns(false);

            var controller = new RecipeAPIController(recipeService, FileUploadService);

            // Act
            var result = await controller.DeleteRecipe(id);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }
        [Fact]
        public async Task RecipeApiController_DeleteIngredient_ReturnsNoContent()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());

            var ingredientId = Guid.NewGuid();
            var recipeId = Guid.NewGuid();
            var ingredientToDelete = fixture.Create<IngredientOutputDto>();

            var recipeService = Substitute.For<IRecipeService>();
            var fileUploadService = Substitute.For<IFileUploadService>();

            recipeService.GetIngredient(ingredientId).Returns(ingredientToDelete);
            recipeService.DeleteIngredient(ingredientId).Returns(true);
            recipeService.GetIngredients(recipeId).Returns(new List<IngredientOutputDto>());

            var controller = new RecipeAPIController(recipeService, fileUploadService);

            // Act
            var result = await controller.DeleteIngredient(recipeId, ingredientId);
            var ingredientsResult = controller.GetAllIngredients(recipeId);
            var okObjectResult = ingredientsResult as OkObjectResult;
            var ingredients = okObjectResult.Value as List<IngredientOutputDto>;

            // Assert
            Assert.IsType<NoContentResult>(result.Result);
            Assert.Empty(ingredients); 
        }

        [Fact]
        public async Task RecipeApiController_UpdateRecipe_ReturnsOkResult()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());

            var id = Guid.NewGuid();
            var patchDocument = new JsonPatchDocument<RecipeInputDto>();

            var existingRecipe = fixture.Create<RecipeOutputDto>();
            var updatedRecipe = fixture.Create<RecipeOutputDto>();

            var recipeService = Substitute.For<IRecipeService>();
            var fileUploadService = Substitute.For<IFileUploadService>();

            recipeService.GetRecipe(id).Returns(existingRecipe);
            recipeService.UpdateRecipe(id, patchDocument).Returns(true);
            recipeService.GetRecipe(id).Returns(updatedRecipe);

            var controller = new RecipeAPIController(recipeService, fileUploadService);

            // Act
            var result = await controller.UpdateRecipe(id, patchDocument);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedRecipe = Assert.IsAssignableFrom<RecipeOutputDto>(okResult.Value);
            Assert.Equal(updatedRecipe.RecipeId, returnedRecipe.RecipeId);
        }

        [Fact]
        public async Task RecipeApiController_UpdateRecipe_ReturnsNotFound()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());

            var id = Guid.NewGuid();
            var patchDocument = new JsonPatchDocument<RecipeInputDto>();

            var recipeService = Substitute.For<IRecipeService>();
            var fileUploadService = Substitute.For<IFileUploadService>();

            recipeService.GetRecipe(id).Returns((RecipeOutputDto)null); 

            var controller = new RecipeAPIController(recipeService, fileUploadService);

            // Act
            var result = await controller.UpdateRecipe(id, patchDocument);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }


    }
}
