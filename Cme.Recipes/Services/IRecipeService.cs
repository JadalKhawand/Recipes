using Cme.Recipes.Models;
using Cme.Recipes.Models.Dto;

namespace Cme.Recipes.Services
{
    public interface IRecipeService
    {
        List<Recipe> GetAllRecipes();
        Recipe GetRecipe(Guid id);
        Recipe CreateRecipe(RecipeDto recipeDto);
        bool DeleteRecipe(Guid id);
        Recipe UpdateRecipe(Guid id, RecipeDto recipeDto);
        Task<List<Recipe>> SearchRecipesByNameAsync(string partialName);
        Task<List<Recipe>> GetRecipesByCategoryAsync(string category);

    }
}
