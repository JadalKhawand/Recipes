using Microsoft.AspNetCore.Mvc;
using System;
using Cme.Recipes.Services;
using Cme.Recipes.Models;
using Cme.Recipes.Models.Dto;
using static Azure.Core.HttpHeader;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using RecipeApp.Domain.Entities;

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

        [HttpGet]
        public IActionResult GetAllRecipes()
        {

            try
            {
                var recipes = _recipeService.GetAllRecipes();

                if (recipes == null)
                    return NotFound();

                return Ok(recipes);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ex.Message);
            }
        }
        // get all ingredients for a specific recipe
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

        // get a single recipe by id
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

        //Create a new recipe
        [HttpPost]
        public ActionResult<Recipe> CreateRecipe([FromBody] RecipeInputDto recipeDto)
        {
            try
            {
                if (recipeDto == null)
                    return BadRequest();

                var createdRecipe = _recipeService.CreateRecipe(recipeDto);

                if (createdRecipe == null)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        "Error creating new recipe");

                return CreatedAtAction(nameof(CreateRecipe), createdRecipe); // 201 Created
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ex.InnerException);
            }
        }

        [HttpPost("{recipeId}/ingredients")]
        public ActionResult<Recipe> CreateIngredient([FromRoute] Guid recipeId ,[FromBody] List<IngredientInputDto> ingredientDto)
        {
            try
            {
                if (ingredientDto == null)
                    return BadRequest();

                var createdIngredient = _recipeService.CreateIngredient(recipeId, ingredientDto);

                if (createdIngredient == null)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        "Error creating ingredient");

                return CreatedAtAction(nameof(CreateIngredient), createdIngredient); // 201 Created
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ex.Message);
            }
        }
        [HttpPut("{recipeId}/ingredients/{ingredientId}")]
        public ActionResult<Ingredient> UpdateIngredient([FromRoute] Guid recipeId, [FromRoute] Guid ingredientId ,[FromBody] IngredientInputDto ingredientDto)
        {
            try
            {
                if (ingredientDto == null)
                    return BadRequest();

                var updatedIngredient = _recipeService.UpdateIngredient(ingredientId, ingredientDto);

                if (updatedIngredient == null)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        "Error updating ingredient");

                return updatedIngredient;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ex.Message);
            }
        }

        [HttpDelete("{recipeId}/ingredients/{ingredientId}")]
        public ActionResult<Ingredient> DeleteIngredient([FromRoute] Guid recipeId, [FromRoute] Guid ingredientId)
        {
            try
            {
                var ingredientToDelete = _recipeService.GetIngredient(ingredientId);

                if (ingredientToDelete == null)
                {
                    return NotFound($"Recipe with Id = {ingredientId} not found");
                }

                var deleteResult = _recipeService.DeleteIngredient(ingredientId);

                if (!deleteResult)
                {
                    return BadRequest("Error deleting recipe");
                }

                return Ok(ingredientToDelete);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // Delete an existing recipe
        [HttpDelete("{id:Guid}")]
        public ActionResult<Recipe> DeleteRecipe(Guid id)
        {
            try
            {
                var recipeToDelete = _recipeService.GetRecipe(id);

                if (recipeToDelete == null)
                {
                    return NotFound($"Recipe with Id = {id} not found");
                }

                var deleteResult = _recipeService.DeleteRecipe(id);

                if (!deleteResult)
                {
                    return BadRequest("Error deleting recipe");
                }

                return Ok(recipeToDelete);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        
        [HttpGet("filter")]
        public async Task<IActionResult> SearchRecipesByNameAndCategory([FromQuery] string name = "", [FromQuery] string category = "")
        {
            List<RecipeOutputDto> recipes = null;

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(category))
            {
                recipes = await _recipeService.SearchRecipesByNameAndCategory(name, category);
            }
            else if (!string.IsNullOrEmpty(name))
            {
                recipes = await _recipeService.SearchRecipesByName(name);
            }
            else if (!string.IsNullOrEmpty(category))
            {
                recipes = await _recipeService.GetRecipesByCategory(category);
            }
            else
            {
                recipes = _recipeService.GetAllRecipes();
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

            return Ok(recipes);
        }


        [HttpPatch("{id:Guid}")]
        public IActionResult UpdateRecipe(Guid id, JsonPatchDocument<RecipeInputDto> recipeDto)
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

                var recipe = _recipeService.UpdateRecipe(id, recipeDto);
                if (recipe == false)
                {
                    return BadRequest();
                }


                var updatedRecipe = _recipeService.GetRecipe(id);

                return Ok(updatedRecipe);

            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost("{recipeId}/upload")]
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
            
            catch(Exception ex){
                return BadRequest(ex.InnerException);
            }
        }



    }

}


