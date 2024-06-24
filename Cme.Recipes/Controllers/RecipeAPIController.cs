using Cme.Recipes.Models;
using Microservices.Services.CouponAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cme.Recipes.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipeAPIController
    {
        private readonly AppDbContext _context;

        public RecipeAPIController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetRecipes()
        {
            var recipes = await _context.Recipes
                .FromSqlRaw("EXECUTE GetRecipesWithIngredients")
                .ToListAsync();

            return recipes;
        }
    }
}


