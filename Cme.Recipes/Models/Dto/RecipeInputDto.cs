using RecipeApp.Domain.Entities;

namespace Cme.Recipes.Models.Dto
{
    public class RecipeInputDto
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public int Calories { get; set; }
        public required string PrepTime { get; set; }
        public required string Image { get; set; }
        public required string Category { get; set; }
        public required string Procedures { get; set; }
    }
}
