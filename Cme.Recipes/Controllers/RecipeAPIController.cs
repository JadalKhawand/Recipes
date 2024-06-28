using Microsoft.AspNetCore.Mvc;
using System;
using Cme.Recipes.Services;
using Cme.Recipes.Models;
using Cme.Recipes.Models.Dto;
using static Azure.Core.HttpHeader;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using RecipeApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cme.Recipes.Controllers
{
    [ApiController]
    [Route("api/v1/recipes")]
    public class RecipeAPIController : ControllerBase
    {
        private readonly IRecipeService _recipeService;
        private readonly IFileUploadService _uploadService;
        public RecipeAPIController(IRecipeService recipeService, IFileUploadService uploadService)
        {
            _recipeService = recipeService;
            _uploadService = uploadService;
        }

        [HttpGet()]
        public async Task<IActionResult> GetAllRecipes(string name = "", string category = "", int pageNumber = 1, int pageSize = 3)
        {
            if (pageNumber <= 0)
            {
                return BadRequest("pageNumber must be greater than zero.");
            }

            if (pageSize <= 0 || pageSize > 50)
            {
                return BadRequest("pageSize must be between 1 and 50.");
            }
            if(pageSize <= 0 && pageNumber <= 0)
            {
                return BadRequest("pageSize and pageNumber must be greater than zero.");
            }
            List<AllRecipesOutputDto> recipes = null;

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(category))
            {
                recipes = await _recipeService.SearchRecipesByNameAndCategory(name, category, pageNumber, pageSize);
            }
            else if (!string.IsNullOrEmpty(name))
            {
                recipes = await _recipeService.SearchRecipesByName(name, pageNumber, pageSize);
            }
            else if (!string.IsNullOrEmpty(category))
            {
                recipes = await _recipeService.GetRecipesByCategory(category, pageNumber, pageSize);
            }
            else
            {
                recipes = _recipeService.GetAllRecipesPaginated(pageNumber, pageSize);
            }

            if (recipes == null || recipes.Count == 0)
            {
                if (!string.IsNullOrEmpty(name) && string.IsNullOrEmpty(category))
                {
                    return NotFound($"No recipes found with name '{name}'.");
                }
                else if (string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(category))
                {
                    return NotFound($"No recipes found in category '{category}'.");
                }
                else
                {
                    return NotFound("No recipes found.");
                }
            }
            var totalCount = _recipeService.CountRecipes();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var response = new
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                PageSize = pageSize,
                Page = pageNumber,
                Recipes = recipes
            };

            return Ok(response);
        }

        [HttpGet("{recipeId:Guid}/ingredients")]
        public IActionResult GetAllIngredients(Guid recipeId)
        {

            try
            {
                var ingredients = _recipeService.GetIngredients(recipeId);

                if (ingredients == null)
                    return NotFound();

                return Ok(ingredients);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ex.Message);
            }
        }

        [HttpGet("{id:Guid}")]
        public IActionResult GetRecipe(Guid id)
        {
            try
            {
                var recipe = _recipeService.GetRecipe(id);

                if (recipe == null)
                    return NotFound();

                return Ok(recipe);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Recipe>> CreateRecipe([FromBody] RecipeInputDto recipeDto)
        {

            if (recipeDto == null)
                return BadRequest();

            var createdRecipe = await _recipeService.CreateRecipe(recipeDto);

            if (createdRecipe == null)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error creating new recipe");

            return CreatedAtAction(nameof(CreateRecipe), createdRecipe);

        }

        [HttpPost("{recipeId}/ingredients")]
        public async Task<ActionResult<Recipe>> CreateIngredient([FromRoute] Guid recipeId, [FromBody] List<IngredientInputDto> ingredientDto)
        {
            try
            {
                if (ingredientDto == null)
                    return BadRequest();

                var createdIngredient = await _recipeService.CreateIngredient(recipeId, ingredientDto);

                if (createdIngredient == null)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        "Error creating ingredient");

                return CreatedAtAction(nameof(CreateIngredient), createdIngredient);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ex.Message);
            }
        }
        [HttpPut("{recipeId}/ingredients/{ingredientId}")]
        public async Task<ActionResult<Ingredient>> UpdateIngredient([FromRoute] Guid recipeId, [FromRoute] Guid ingredientId, [FromBody] IngredientInputDto ingredientDto)
        {
            try
            {
                if (ingredientDto == null)
                {
                    return BadRequest();
                }

                var existingIngredient = _recipeService.GetIngredient(ingredientId);

                if (existingIngredient == null)
                {
                    return NotFound($"Ingredient with ID = {ingredientId} was not found");
                }

                var updatedIngredient = await _recipeService.UpdateIngredient(ingredientId, ingredientDto);

                if (updatedIngredient == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error updating ingredient");
                }

                return updatedIngredient;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [HttpDelete("{recipeId}/ingredients/{ingredientId}")]
        public async Task<ActionResult<Ingredient>> DeleteIngredient([FromRoute] Guid recipeId, [FromRoute] Guid ingredientId)
        {
            try
            {
                var ingredientToDelete = _recipeService.GetIngredient(ingredientId);

                if (ingredientToDelete == null)
                {
                    return NotFound($"Recipe with Id = {ingredientId} not found");
                }

                var deleteResult = await _recipeService.DeleteIngredient(ingredientId);

                if (!deleteResult)
                {
                    return BadRequest("Error deleting recipe");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{id:Guid}")]
        public async Task<ActionResult<Recipe>> DeleteRecipe(Guid id)
        {
            try
            {
                var recipeToDelete = _recipeService.GetRecipe(id);

                if (recipeToDelete == null)
                {
                    return NotFound($"Recipe with Id = {id} not found");
                }

                var deleteResult = await _recipeService.DeleteRecipe(id);

                if (!deleteResult)
                {
                    return BadRequest("Error deleting recipe");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [HttpPatch("{id:Guid}")]
        public async Task<IActionResult> UpdateRecipe(Guid id, JsonPatchDocument<RecipeInputDto> recipeDto)
        {
            try
            {
                var recipeToUpdate = _recipeService.GetRecipe(id);

                if (recipeToUpdate == null)
                    return NotFound($"recipe with Id = {id} not found");

                if (recipeDto == null)
                {
                    return BadRequest();
                }

                var recipe = await _recipeService.UpdateRecipe(id, recipeDto);
                if (recipe == false)
                {
                    return BadRequest();
                }


                var updatedRecipe = _recipeService.GetRecipe(id);

                return Ok(updatedRecipe);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{recipeId}/image")]
        public async Task<IActionResult> UploadImage(Guid recipeId, [FromForm] ImageUploadDto imageUploadDto)
        {
            try
            {
                if (imageUploadDto.Image == null)
                    return BadRequest("No image uploaded.");

                var image = await _uploadService.UploadImageAsync(recipeId, imageUploadDto.Image);

                if (image == null)
                    return StatusCode(500, "An error occurred while uploading the image.");

                return Ok(image);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.InnerException);
            }
        }

        [HttpDelete("{recipeId}/image")]
        public async Task<IActionResult> DeleteImage(Guid recipeId)
        {
            try
            {
                var recipeToDelete = _recipeService.GetRecipe(recipeId);
                if(recipeToDelete == null)
                {
                    return NotFound("recipe not found");
                }
                var deletedImageResult = await _uploadService.DeleteImage(recipeId);
                if (!deletedImageResult)
                {
                    return BadRequest("Error deleting image");
                }
                return NoContent();
                   
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{recipeId}/image")]
        public async Task<IActionResult> UpdateImage(Guid recipeId, [FromForm] ImageUploadDto imageUploadDto)
        {
            try
            {
                var recipe = _recipeService.GetRecipe(recipeId);
                if(recipe == null)
                {
                    return NotFound("Recipe not found");
                }
                if (imageUploadDto.Image == null)
                    return BadRequest("No image uploaded.");

                var image = await _uploadService.UpdateImage(recipeId, imageUploadDto.Image);

                if (image == null)
                    return StatusCode(500, "An error occurred while uploading the image.");

                return Ok(image);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.InnerException);
            }
        }

    }
}


