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
            var recipes = Assert.IsAssignableFrom<List<AllRecipesOutputDto>>(okResult.Value);
            var count = recipes.Count;
            Assert.Equal(3, count);
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

        /* [Fact]
         public void RecipeApiController_UpdateRecipe_ReturnsActionResult()
         {
             // Arrange
             var fixture = new Fixture();
             fixture.Customize(new AutoNSubstituteCustomization());

             var id = Guid.NewGuid();
             var recipeDto = fixture.Create<RecipeInputDto>();
             var recipeToUpdate = fixture.Create<RecipeOutputDto>();
             var updatedRecipe = fixture.Create<Recipe>();
             var recipeService = Substitute.For<IRecipeService>();
             var FileUploadService = Substitute.For<IFileUploadService>();

             recipeService.GetRecipe(id).Returns(recipeToUpdate);
             recipeService.UpdateRecipe(id, recipeDto).Returns(updatedRecipe);

             var controller = new RecipeAPIController(recipeService, FileUploadService);


             // Act
             var result = controller.UpdateRecipe(id, recipeDto);

             // Assert
             Assert.IsType<OkObjectResult>(result.Result);
             var okObjectResult = (OkObjectResult)result.Result;
             Assert.Equal(updatedRecipe, okObjectResult.Value);
         }*/

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
    }
}
