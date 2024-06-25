using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Cme.Recipes.Models.Dto
{
    public class IngredientOutputDto
    {
        public Guid IngredientId { get; set; }

        public required string IngredientName { get; set; }

        public required string Amount { get; set; }

        public required string Type { get; set; }

        public Guid RecipeId { get; set; }
    }
}
