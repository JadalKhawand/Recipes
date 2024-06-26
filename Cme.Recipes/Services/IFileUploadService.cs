using Cme.Recipes.Models;

namespace Cme.Recipes.Services
{
    public interface IFileUploadService
    {
        Task<Image> UploadImageAsync(Guid recipeId, IFormFile file);
    }
}
