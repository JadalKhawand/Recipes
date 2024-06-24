using RecipeApp.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Cme.Recipes.Models
{
    public class Recipe
    {
        public Guid RecipeId { get; set; }

        [Required]
        public required string Name { get; set; }

        [Required]
        public required string Description { get; set; }

        public int Calories { get; set; }

        [Required]
        public required string PrepTime { get; set; }

        [Required]
        public required string Image { get; set; }

        [Required]
        public required string Category { get; set; }

        public List<Ingredient> Ingredients { get; set; } = new List<Ingredient>();

        [Required]
        public required string Procedures { get; set; }
    }
}
