using Cme.Recipes.Models;
using Cme.Recipes.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using RecipeApp.Domain.Entities;

namespace Cme.Recipes.Services
{
    public interface IRecipeService
    {
        List<RecipeOutputDto> GetAllRecipes();
        RecipeOutputDto GetRecipe(Guid id);
        Recipe CreateRecipe(RecipeInputDto recipeDto);
        List<Ingredient> CreateIngredient(Guid id, List<IngredientInputDto> ingredientInputDto);
        bool DeleteRecipe(Guid id);
        Recipe UpdateRecipe(Guid id, RecipeInputDto recipeDto);
        Task<List<RecipeOutputDto>> SearchRecipesByName(string partialName);
        Task<List<RecipeOutputDto>> GetRecipesByCategory(string category);
        Task<List<RecipeOutputDto>> SearchRecipesByNameAndCategory(string partialName, string category);
        List<IngredientOutputDto> GetIngredients(Guid RecipeId);
        bool UpdateRecipe(Guid id, JsonPatchDocument<RecipeInputDto> patchDto);
        Ingredient UpdateIngredient(Guid ingredientId, IngredientInputDto ingredientInputDto);
        bool DeleteIngredient(Guid id);
        IngredientOutputDto GetIngredient(Guid ingredientId);
        List<RecipeOutputDto> GetAllRecipesPaginated(int page = 1, int pageSize = 3);
        int CountRecipes();

    }
}
