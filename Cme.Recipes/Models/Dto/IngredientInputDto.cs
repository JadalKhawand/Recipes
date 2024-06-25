using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Cme.Recipes.Models.Dto
{
    public class IngredientInputDto
    {
        public required string IngredientName { get; set; }

        public required string Amount { get; set; }

        public required string Type { get; set; }

    }
}
