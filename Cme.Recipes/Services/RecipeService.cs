using AutoMapper;
using Cme.Recipes.Models;
using Cme.Recipes.Models.Dto;
using Cme.Recipes.Data;
using Microsoft.EntityFrameworkCore;
using RecipeApp.Domain.Entities;
using static Azure.Core.HttpHeader;
using System.Net.Mail;
using Microsoft.AspNetCore.JsonPatch;

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


        public List<AllRecipesOutputDto> GetAllRecipesPaginated(int page = 1, int pageSize = 3)
        {
            int skipAmount = (page - 1) * pageSize;

            List<Recipe> recipes = null;

            if (page == 1)
            {
                recipes = _context.Recipes
                .Include(r => r.Ingredients)
                .Include(r => r.Image)
                .Skip(skipAmount)
                .Take(pageSize - 1)
                .ToList();
            }
            else
            {
                recipes = _context.Recipes
                 .Include(r => r.Ingredients)
                 .Include(r => r.Image)
                 .Skip(skipAmount)
                 .Take(pageSize)
                 .ToList();
            }


            List<AllRecipesOutputDto> outputRecipes = _mapper.Map<List<AllRecipesOutputDto>>(recipes);

            return outputRecipes;
        }
        public int CountRecipes()
        {
            return _context.Recipes.Count();
        }

        public RecipeOutputDto GetRecipe(Guid id)
        {
            var recipe = _context.Recipes.Include(r => r.Ingredients).Include(r => r.Image).FirstOrDefault(u => u.RecipeId == id);
            if (recipe == null)
                throw new Exception("Recipe isn't found");
            var outputRecipe = _mapper.Map<RecipeOutputDto>(recipe);
            return outputRecipe;
        }

        public async Task<List<Ingredient>> CreateIngredient(Guid id, List<IngredientInputDto> ingredientInputDto)
        {
            var ingredients = _mapper.Map<List<Ingredient>>(ingredientInputDto);

            foreach (var ingredient in ingredients)
            {
                ingredient.IngredientId = Guid.NewGuid();
                ingredient.RecipeId = id;
                _context.Ingredients.Add(ingredient);
            }
            await _context.SaveChangesAsync();
            return ingredients;
        }


        public async Task<Recipe> CreateRecipe(RecipeInputDto recipeDto)
        {
            var recipe = _mapper.Map<Recipe>(recipeDto);

            recipe.RecipeId = Guid.NewGuid();

            foreach (var ingredient in recipe.Ingredients)
            {
                ingredient.IngredientId = Guid.NewGuid();
                ingredient.RecipeId = recipe.RecipeId;
            }

            _context.Recipes.Add(recipe);

            await _context.SaveChangesAsync();

            return recipe;
        }

        public async Task<bool> DeleteRecipe(Guid id)
        {
            
                var recipe = _context.Recipes.FirstOrDefault(u => u.RecipeId == id);
                var ingredients = _context.Ingredients.Where(u => u.RecipeId == id).ToList();
                if (recipe == null)
                    return false;
                _context.Ingredients.RemoveRange(ingredients);
                _context.Recipes.Remove(recipe);
                await _context.SaveChangesAsync();
                return true;
            
        }
        public List<IngredientOutputDto> GetIngredients(Guid RecipeId)
        {
            List<Ingredient> ingredients = _context.Ingredients.Where(r => r.RecipeId == RecipeId).ToList();
            List<IngredientOutputDto> outputIngredients = _mapper.Map<List<IngredientOutputDto>>(ingredients);
            return outputIngredients;
        }

        public IngredientOutputDto GetIngredient(Guid ingredientId)
        {
            if (ingredientId == Guid.Empty)
            {
                throw new Exception("ingredient id is required");
            }
            Ingredient ingredient = _context.Ingredients.FirstOrDefault(i => i.IngredientId == ingredientId);
            if (ingredient == null)
                throw new Exception("Ingredient isn't found");
            var outputIngredient = _mapper.Map<IngredientOutputDto>(ingredient);
            return outputIngredient;
        }

        public async Task<Recipe> UpdateRecipe(Guid id, RecipeInputDto recipeDto)
        {

            var existingRecipe = _context.Recipes.FirstOrDefault(c => c.RecipeId == id);
            if (existingRecipe == null)
                throw new Exception("recipe not found");

            var updatedRecipe = _mapper.Map(recipeDto, existingRecipe);
            _context.Recipes.Update(updatedRecipe);
            await _context.SaveChangesAsync();
            return updatedRecipe;

        }

        public async Task<Ingredient> UpdateIngredient(Guid ingredientId, IngredientInputDto ingredientInputDto)
        {
            var existingIngredient = _context.Ingredients.FirstOrDefault(i => i.IngredientId == ingredientId);
            if (existingIngredient == null)
                throw new Exception("recipe not found");

            var updatedIngredient = _mapper.Map(ingredientInputDto, existingIngredient);
            _context.Ingredients.Update(updatedIngredient);

            await _context.SaveChangesAsync();


            return updatedIngredient;
        }

        public async Task<bool> DeleteIngredient(Guid id)
        {
            try
            {
                var ingredient = _context.Ingredients.FirstOrDefault(i => i.IngredientId == id);
                if (ingredient == null)
                    return false;
                _context.Ingredients.Remove(ingredient);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<AllRecipesOutputDto>> SearchRecipesByName(string partialName, int page = 1, int pageSize = 3)
        {
            int skipAmount = (page - 1) * pageSize;

            List<Recipe> recipes = null;
            if (page == 1)
            {
                recipes = await _context.Recipes
                .Where(r => EF.Functions.Like(r.Name, $"%{partialName}%")).Include(r => r.Image).Skip(skipAmount)
                .Take(pageSize - 1)
                .ToListAsync();
            }
            else
            {
                recipes = await _context.Recipes
                .Where(r => EF.Functions.Like(r.Name, $"%{partialName}%")).Include(r => r.Image).Skip(skipAmount)
                .Take(pageSize)
                .ToListAsync();
            }

            List<AllRecipesOutputDto> outputRecipes = _mapper.Map<List<AllRecipesOutputDto>>(recipes);
            return outputRecipes;

        }

        public async Task<List<AllRecipesOutputDto>> GetRecipesByCategory(string category, int page = 1, int pageSize = 3)
        {
            int skipAmount = (page - 1) * pageSize;
            List<Recipe> recipes = null;
            if (page == 1)
            {
                recipes = await _context.Recipes
                   .Where(r => r.Category.ToLower() == category.ToLower()).Include(r => r.Image).Skip(skipAmount)
                   .Take(pageSize - 1)
                   .ToListAsync();
            }
            else
            {
                recipes = await _context.Recipes
               .Where(r => r.Category.ToLower() == category.ToLower()).Include(r => r.Image).Skip(skipAmount)
               .Take(pageSize)
               .ToListAsync();
            }

            List<AllRecipesOutputDto> outputRecipes = _mapper.Map<List<AllRecipesOutputDto>>(recipes);
            return outputRecipes;
        }
        public async Task<List<AllRecipesOutputDto>> SearchRecipesByNameAndCategory(string partialName, string category, int page = 1, int pageSize = 3)
        {
            int skipAmount = (page - 1) * pageSize;
            List<Recipe> recipes = null;

            if (page == 1)
            {
                recipes = await _context.Recipes
               .Where(r => r.Category.ToLower() == category.ToLower() && EF.Functions.Like(r.Name, $"%{partialName}%")).Include(r => r.Image).Skip(skipAmount)
                .Take(pageSize - 1)
               .ToListAsync();
            }
            else
            {
                recipes = await _context.Recipes
               .Where(r => r.Category.ToLower() == category.ToLower() && EF.Functions.Like(r.Name, $"%{partialName}%")).Include(r => r.Image).Skip(skipAmount)
                .Take(pageSize)
               .ToListAsync();
            }
            List<AllRecipesOutputDto> outputRecipes = _mapper.Map<List<AllRecipesOutputDto>>(recipes);
            return outputRecipes;
        }
        public async Task<bool> UpdateRecipe(Guid id, JsonPatchDocument<RecipeInputDto> patchDto)
        {
            try
            {
                var recipe = _context.Recipes.FirstOrDefault(r => r.RecipeId == id);
                if (recipe == null)
                    return false;

                var recipeDto = _mapper.Map<RecipeInputDto>(recipe);
                patchDto.ApplyTo(recipeDto);
                _mapper.Map(recipeDto, recipe);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
