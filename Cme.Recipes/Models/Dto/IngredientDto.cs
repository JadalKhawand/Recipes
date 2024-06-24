using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Cme.Recipes.Models.Dto
{
    public class IngredientDto
    {
        public required string IngredientName { get; set; }

        public required string Amount { get; set; }

        public required string Type { get; set; }

        [Required]
        public Guid RecipeId { get; set; }

        [ForeignKey(nameof(RecipeId))]
        public virtual required Recipe Recipe { get; set; }
    }
}
