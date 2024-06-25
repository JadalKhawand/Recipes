using AutoMapper;
using Cme.Recipes.Models;
using Cme.Recipes.Models.Dto;
using Cme.Recipes.Data;
using Microsoft.EntityFrameworkCore;
using RecipeApp.Domain.Entities;
using static Azure.Core.HttpHeader;
using System.Net.Mail;

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
            List<Recipe> recipes = _context.Recipes
                .Include(r => r.Ingredients)
                .ToList();
            List<RecipeOutputDto> outputRecipes = _mapper.Map<List<RecipeOutputDto>>(recipes);

            return outputRecipes;

        }

        public RecipeOutputDto GetRecipe(Guid id)
        {
            var recipe = _context.Recipes.Include(r => r.Ingredients).FirstOrDefault(u => u.RecipeId == id);
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
            _context.SaveChangesAsync();
            return ingredients;
        }


        public Recipe CreateRecipe(RecipeInputDto recipeInputDto)
        {
            var recipe = _mapper.Map<Recipe>(recipeInputDto);

            recipe.RecipeId = Guid.NewGuid();

            _context.Recipes.Add(recipe);

            _context.SaveChangesAsync();

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
        /*
        public List<Ingredient> UpdateIngredient(Guid id, List<IngredientInputDto> ingredientInputDto)
        {
            var ingredients = GetIngredients(id);

            

            _context.SaveChangesAsync();

            return ingredients;
        }
         
        public async Task<List<Ingredient>> UpdateIngredient(RecipeOutputDto recipe, List<IngredientInputDto> ingredientInputDto)
        {
            var existingIngredients = recipe.Ingredients;

            // Update existing ingredients based on ingredientInputDto
            foreach (var existingIngredient in existingIngredients)
            {
                // Find matching ingredientInputDto
                var dto = ingredientInputDto.FirstOrDefault(i => i.IngredientName == existingIngredient.IngredientId);

                if (dto != null)
                {
                    // Update existing ingredient properties
                    existingIngredient.Name = dto.Name;
                    existingIngredient.Quantity = dto.Quantity;
                    existingIngredient.Unit = dto.Unit;
                    // Add any other properties you need to update
                }
                else
                {
                    // Ingredient was not found in ingredientInputDto, consider deleting or ignoring it
                    // For example:
                    // _context.Ingredients.Remove(existingIngredient);
                }
            }

            // Add new ingredients if any
            foreach (var dto in ingredientInputDto)
            {
                if (dto.IngredientId == Guid.Empty)
                {
                    // This is a new ingredient, create a new Ingredient entity
                    var newIngredient = new Ingredient
                    {
                        IngredientId = Guid.NewGuid(),
                        RecipeId = id,
                        Name = dto.Name,
                        Quantity = dto.Quantity,
                        Unit = dto.Unit
                        // Add any other properties as needed
                    };

                    _context.Ingredients.Add(newIngredient);
                }
            }

            // Save changes asynchronously
            await _context.SaveChangesAsync();

            // Return updated ingredients
            return existingIngredients;
        }*/


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

    }
}
