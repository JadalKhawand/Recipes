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
        [ForeignKey(nameof(RecipeId))]

        public Guid RecipeId { get; set; }



    }
}
