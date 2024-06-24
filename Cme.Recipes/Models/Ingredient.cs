using Cme.Recipes.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeApp.Domain.Entities
{
    public class Ingredient
    {
        [Key]
        public Guid IngredientId { get; set; }

        [Required]
        public required string IngredientName { get; set; }

        [Required]
        public required string Amount { get; set; }

        [Required]
        public required string Type { get; set; }

        [Required]
        public Guid RecipeId { get; set; }

        [ForeignKey(nameof(RecipeId))]
        public virtual required Recipe Recipe { get; set; }
    }
}
