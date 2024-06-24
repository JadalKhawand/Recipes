using AutoMapper;
using Cme.Recipes.Models;
using Cme.Recipes.Models.Dto;
using Cme.Recipes.Data;
using Microsoft.EntityFrameworkCore;
using RecipeApp.Domain.Entities;
using static Azure.Core.HttpHeader;

namespace Cme.Recipes.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public RecipeService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public List<Recipe> GetAllRecipes()
        {
            List<Recipe> recipes = _context.Recipes
                .Include(r => r.Ingredients)
                .ToList();
            return recipes;

        }

        public Recipe GetRecipe(Guid id)
        {
            var recipe = _context.Recipes.FirstOrDefault(u => u.RecipeId == id);
            if (recipe == null)
                throw new Exception("Recipe isn't found");

            return recipe;
        }
        /*
         public Ingredient CreateIngredient(Guid id, IngredientDto ingredientDto)
         {
             var ingredient = _mapper.Map<Ingredient>(ingredientDto);
             ingredient.IngredientId = Guid.NewGuid();
             ingredient.RecipeId = id;
             return ingredient;
         }
        */
        public Recipe CreateRecipe(RecipeDto recipeDto)
        {
            var recipe = _mapper.Map<Recipe>(recipeDto);

            recipe.RecipeId = Guid.NewGuid();

            foreach (var ingredient in recipe.Ingredients)
            {
                ingredient.IngredientId = Guid.NewGuid();
                ingredient.RecipeId = recipe.RecipeId;
            }

            _context.Recipes.Add(recipe);

            _context.SaveChangesAsync();

            return recipe;
        }

        public bool DeleteRecipe(Guid id)
        {
            try
            {
                var recipe = _context.Recipes.FirstOrDefault(u => u.RecipeId == id);
                if (recipe == null)
                    return false;

                _context.Recipes.Remove(recipe);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Recipe UpdateRecipe(Guid id, RecipeDto recipeDto)
        {

            var existingRecipe = _context.Recipes.FirstOrDefault(c => c.RecipeId == id);
            if (existingRecipe == null)
                return null;

            var updatedRecipe = _mapper.Map(recipeDto, existingRecipe);
            _context.Recipes.Update(updatedRecipe);
            _context.SaveChanges();
            return updatedRecipe;

        }
        public async Task<List<Recipe>> SearchRecipesByNameAsync(string partialName)
        {
            return await _context.Recipes
                .Where(r => EF.Functions.Like(r.Name, $"%{partialName}%"))
                .ToListAsync();
        }

        public async Task<List<Recipe>> GetRecipesByCategoryAsync(string category)
        {
            return await _context.Recipes
                .Where(r => r.Category.ToLower() == category.ToLower())
                .ToListAsync();
        }

    }
}
