using Microsoft.AspNetCore.Mvc;
using System;
using Cme.Recipes.Services;
using Cme.Recipes.Models;
using Cme.Recipes.Models.Dto;
using static Azure.Core.HttpHeader;

namespace Cme.Recipes.Controllers
{
    [ApiController]
    [Route("api/v1/recipes")]
    public class RecipeAPIController : ControllerBase
    {
        private readonly IRecipeService _recipeService;

        public RecipeAPIController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
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
        public ActionResult<Recipe> CreateRecipe([FromBody] RecipeDto recipeDto)
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
                    ex.Message);
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

        // update an existing recipe
        [HttpPut("{id}")]
        public ActionResult<Recipe> UpdateRecipe(Guid id, RecipeDto recipeDto)
        {
            try
            {
                var recipenToUpdate = _recipeService.GetRecipe(id);

                if (recipenToUpdate == null)
                    return NotFound($"Recipe with Id = {id} not found");

                var updatedRecipe = _recipeService.UpdateRecipe(id, recipeDto);

                if (updatedRecipe == null)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        "Error updating data");

                return Ok(updatedRecipe);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error updating data");
            }
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> SearchRecipesByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("search parameter is required.");
            }

            var recipes = await _recipeService.SearchRecipesByNameAsync(name);

            if (recipes == null || recipes.Count == 0)
            {
                return NotFound($"No recipes found with name '{name}'.");
            }

            return Ok(recipes);
        }

        [HttpGet("category")]
        public async Task<IActionResult> GetRecipesByCategory(string category)
        {
            if (string.IsNullOrEmpty(category))
            {
                return BadRequest("Category parameter is required.");
            }

            var recipes = await _recipeService.GetRecipesByCategoryAsync(category);

            if (recipes == null || recipes.Count == 0)
            {
                return NotFound($"No recipes found in category '{category}'.");
            }

            return Ok(recipes);
        }


    }

}


