using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Cme.Recipes.Models
{
    public class Image
    {
        public Guid imageId { get; set; }
        public required string fileName { get; set; }
        public required string filepath { get; set; }

        [Required]
        [ForeignKey(nameof(RecipeId))]

        public Guid RecipeId { get; set; }
    }
}
