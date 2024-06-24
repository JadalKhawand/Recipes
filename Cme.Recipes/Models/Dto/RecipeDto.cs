using RecipeApp.Domain.Entities;

namespace Cme.Recipes.Models.Dto
{
    public class RecipeDto
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public int Calories { get; set; }
        public required string PrepTime { get; set; }
        public required string Image { get; set; }
        public required string Category { get; set; }
        public List<IngredientDto> Ingredients { get; set; } = new List<IngredientDto>();
        public required string Procedures { get; set; }
    }
}
