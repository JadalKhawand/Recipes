
using Cme.Recipes.Data;
using Cme.Recipes.Models;
using Microsoft.EntityFrameworkCore;

namespace Cme.Recipes.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly AppDbContext _context;

        public FileUploadService(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<Image> UploadImageAsync(Guid recipeId, IFormFile file)
        {
            var uploadsDirectory = Path.Combine(_environment.ContentRootPath, "images");
            if (!Directory.Exists(uploadsDirectory))
                Directory.CreateDirectory(uploadsDirectory);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsDirectory, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            var image = new Image
            {
                imageId = Guid.NewGuid(),
                fileName  = file.FileName,
                filepath = filePath,
                RecipeId = recipeId
            };

            _context.Images.Add(image);
            await _context.SaveChangesAsync();

            return image;
        }



    }
}
