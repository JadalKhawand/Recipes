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

        public List<RecipeOutputDto> GetAllRecipes()
        {
           // List<Recipe> recipes = _context.Recipes.FromSqlRaw("EXECUTE GetAllRecipes").ToList();
           List<Recipe> recipes = _context.Recipes.Include(r=>r.Ingredients).Include(r=>r.Image).ToList();
            List<RecipeOutputDto> outputRecipes = _mapper.Map<List<RecipeOutputDto>>(recipes);

            return outputRecipes;

        }

        public RecipeOutputDto GetRecipe(Guid id)
        {
            var recipe = _context.Recipes.Include(r => r.Ingredients).Include(r => r.Image).FirstOrDefault(u => u.RecipeId == id);
            if (recipe == null)
                throw new Exception("Recipe isn't found");
            var outputRecipe = _mapper.Map<RecipeOutputDto>(recipe);
            return outputRecipe;
        }

        public List<Ingredient> CreateIngredient(Guid id, List<IngredientInputDto> ingredientInputDto)
        {
            var ingredients = _mapper.Map<List<Ingredient>>(ingredientInputDto);

            foreach (var ingredient in ingredients)
            {
                ingredient.IngredientId = Guid.NewGuid();
                ingredient.RecipeId = id;
                _context.Ingredients.Add(ingredient);
            }
            _context.SaveChanges();
            return ingredients;
        }


        public Recipe CreateRecipe(RecipeInputDto recipeDto)
        {
            var recipe = _mapper.Map<Recipe>(recipeDto);

            recipe.RecipeId = Guid.NewGuid();

            foreach (var ingredient in recipe.Ingredients)
            {
                ingredient.IngredientId = Guid.NewGuid();
                ingredient.RecipeId = recipe.RecipeId;
            }

            _context.Recipes.Add(recipe);

            _context.SaveChanges();

            return recipe;
        }

        public bool DeleteRecipe(Guid id)
        {
            try
            {
                var recipe = _context.Recipes.FirstOrDefault(u => u.RecipeId == id);
                var ingredients = _context.Ingredients.Where(u => u.RecipeId == id).ToList();
                if (recipe == null)
                    return false;
                _context.Ingredients.RemoveRange(ingredients);
                _context.Recipes.Remove(recipe);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public List<IngredientOutputDto> GetIngredients(Guid RecipeId)
        {
            List<Ingredient> ingredients = _context.Ingredients.Where(r=> r.RecipeId == RecipeId).ToList();
            List<IngredientOutputDto> outputIngredients = _mapper.Map<List<IngredientOutputDto>>(ingredients);
            return outputIngredients;
        }

        public IngredientOutputDto GetIngredient(Guid ingredientId)
        {
            Ingredient ingredient = _context.Ingredients.FirstOrDefault(i => i.IngredientId == ingredientId);
            if (ingredient == null)
                throw new Exception("Ingredient isn't found");
            var outputIngredient = _mapper.Map<IngredientOutputDto>(ingredient);
            return outputIngredient;
        }
        
        public Recipe UpdateRecipe(Guid id, RecipeInputDto recipeDto)
        {

            var existingRecipe = _context.Recipes.FirstOrDefault(c => c.RecipeId == id);
            if (existingRecipe == null)
                return null;

            var updatedRecipe = _mapper.Map(recipeDto, existingRecipe);
            _context.Recipes.Update(updatedRecipe);
            _context.SaveChanges();
            return updatedRecipe;

        }
        
        public Ingredient UpdateIngredient(Guid ingredientId, IngredientInputDto ingredientInputDto)
        {
            var existingIngredient = _context.Ingredients.FirstOrDefault(i=>i.IngredientId == ingredientId);
            if (existingIngredient == null)
                return null;

            var updatedIngredient = _mapper.Map(ingredientInputDto, existingIngredient);
            _context.Ingredients.Update(updatedIngredient);

            _context.SaveChanges();

            return updatedIngredient;
        }

        public bool DeleteIngredient(Guid id)
        {
            try
            {
                var ingredient = _context.Ingredients.FirstOrDefault(i => i.IngredientId == id);
                if (ingredient == null)
                    return false;
                _context.Ingredients.Remove(ingredient);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<RecipeOutputDto>> SearchRecipesByName(string partialName)
        {
            var recipes = await _context.Recipes
                .Where(r => EF.Functions.Like(r.Name, $"%{partialName}%"))
                .ToListAsync();
            List<RecipeOutputDto> outputRecipes = _mapper.Map<List<RecipeOutputDto>>(recipes);
            return outputRecipes;
        }

        public async Task<List<RecipeOutputDto>> GetRecipesByCategory(string category)
        {
            var recipes = await _context.Recipes
                .Where(r => r.Category.ToLower() == category.ToLower())
                .ToListAsync();
            List<RecipeOutputDto> outputRecipes = _mapper.Map<List<RecipeOutputDto>>(recipes);
            return outputRecipes;
        }
        public async Task<List<RecipeOutputDto>> SearchRecipesByNameAndCategory(string partialName, string category)
        {
            var recipes = await _context.Recipes
               .Where(r => EF.Functions.Like(r.Name, $"%{partialName}%")&& r.Category.ToLower() == category.ToLower())
               .ToListAsync();
            List<RecipeOutputDto> outputRecipes = _mapper.Map<List<RecipeOutputDto>>(recipes);
            return outputRecipes;
        }
        public bool UpdateRecipe(Guid id, JsonPatchDocument<RecipeInputDto> patchDto)
        {
            try
            {
                var recipe = _context.Recipes.FirstOrDefault(r => r.RecipeId == id);
                if (recipe == null)
                    return false;

                var recipeDto = _mapper.Map<RecipeInputDto>(recipe);
                patchDto.ApplyTo(recipeDto);
                _mapper.Map(recipeDto, recipe);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        

    }
}
