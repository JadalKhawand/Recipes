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

namespace CmeRecipe.Test.Controllers
{
    public class RecipeControllerTest
    {
        [Fact]
        public void RecipeApiController_GetAllRecipes_ReturnsOK()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());

            var fakeRecipes = fixture.CreateMany<RecipeOutputDto>(2).ToList();


            var recipeService = Substitute.For<IRecipeService>();
            var FileUploadService = Substitute.For<IFileUploadService>();
            recipeService.GetAllRecipes().Returns(fakeRecipes);

            var controller = new RecipeAPIController(recipeService,FileUploadService);

            // Act
            var result = controller.GetAllRecipes();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var recipes = Assert.IsAssignableFrom<List<RecipeOutputDto>>(okResult.Value);
            var count = recipes.Count;
            Assert.Equal(2, count);
        }



        [Fact]
        public void RecipeApiController_CreateRecipe_ReturnsActionResult()
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
            var result = controller.CreateRecipe(recipeDto);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdAtActionResult = (CreatedAtActionResult)result.Result;
            Assert.Equal(nameof(RecipeAPIController.CreateRecipe), createdAtActionResult.ActionName);
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
        public void RecipeApiController_DeleteRecipe_ReturnsOk()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());

            var id = Guid.NewGuid();
            var recipeToDelete = fixture.Create<RecipeOutputDto>();
            var updatedrecipe = fixture.Create<Recipe>();
            var recipeService = Substitute.For<IRecipeService>();
            var FileUploadService = Substitute.For<IFileUploadService>();

            recipeService.GetRecipe(id).Returns(recipeToDelete);
            recipeService.DeleteRecipe(id).Returns(true);

            var controller = new RecipeAPIController(recipeService, FileUploadService);

            // Act
            var result = controller.DeleteRecipe(id);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            var okObjectResult = (OkObjectResult)result.Result;
            Assert.Equal(recipeToDelete, okObjectResult.Value);
        }

        [Fact]
        public void RecipeApiController_DeleteRecipe_NonExistingRecipe_ReturnsNotFound()
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
            var result = controller.DeleteRecipe(id);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public void RecipeApiController_DeleteRecipe_FailureToDelete_ReturnsBadRequest()
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
            var result = controller.DeleteRecipe(id);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }
    }
}
