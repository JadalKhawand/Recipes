using RecipeApp.Domain.Entities;

namespace Cme.Recipes.Models.Dto
{
    public class AllRecipesOutputDto
    {
        public Guid RecipeId { get; set; }

        public required string Name { get; set; }

        public required string Description { get; set; }


        public required string PrepTime { get; set; }

        public required ImageOutputDto Image { get; set; }

        public required string Category { get; set; }

    }
}
