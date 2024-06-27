using Cme.Recipes.Models;
using Cme.Recipes.Models.Dto;

namespace Cme.Recipes.Services
{
    public interface IFileUploadService
    {
        Task<ImageOutputDto> UploadImageAsync(Guid recipeId, IFormFile file);
    }
}
