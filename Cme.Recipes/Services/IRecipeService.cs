using Cme.Recipes.Models;
using Cme.Recipes.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using RecipeApp.Domain.Entities;

namespace Cme.Recipes.Services
{
    public interface IRecipeService
    {
        RecipeOutputDto GetRecipe(Guid id);
        Task<Recipe> CreateRecipe(RecipeInputDto recipeDto);
        Task<List<Ingredient>> CreateIngredient(Guid id, List<IngredientInputDto> ingredientInputDto);
        Task<bool> DeleteRecipe(Guid id);
        Task<Recipe> UpdateRecipe(Guid id, RecipeInputDto recipeDto);
        Task<List<AllRecipesOutputDto>> SearchRecipesByName(string partialName, int page = 1, int pageSize = 3);
        Task<List<AllRecipesOutputDto>> GetRecipesByCategory(string category, int page = 1, int pageSize = 3);
        Task<List<AllRecipesOutputDto>> SearchRecipesByNameAndCategory(string partialName, string category, int page = 1, int pageSize = 3);
        List<IngredientOutputDto> GetIngredients(Guid RecipeId);
        Task<bool> UpdateRecipe(Guid id, JsonPatchDocument<RecipeInputDto> patchDto);
        Task<Ingredient> UpdateIngredient(Guid ingredientId, IngredientInputDto ingredientInputDto);
        Task<bool> DeleteIngredient(Guid id);
        IngredientOutputDto GetIngredient(Guid ingredientId);
        List<AllRecipesOutputDto> GetAllRecipesPaginated(int page = 1, int pageSize = 3);
        int CountRecipes();

    }
}
