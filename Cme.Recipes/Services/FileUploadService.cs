
using AutoMapper;
using Cme.Recipes.Data;
using Cme.Recipes.Models;
using Cme.Recipes.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace Cme.Recipes.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public FileUploadService(AppDbContext context, IWebHostEnvironment environment, IMapper mapper)
        {
            _context = context;
            _environment = environment;
            _mapper = mapper;

        }
        public async Task<ImageOutputDto> UploadImageAsync(Guid recipeId, IFormFile file)
        {
            var uploadsDirectory = Path.Combine(_environment.ContentRootPath, "images");
            if (!Directory.Exists(uploadsDirectory))
                Directory.CreateDirectory(uploadsDirectory);

            var existingImage = await _context.Images.FirstOrDefaultAsync(i => i.RecipeId == recipeId);

            if (existingImage != null)
            {
                if (File.Exists(existingImage.filepath))
                {
                    File.Delete(existingImage.filepath);
                }

                existingImage.fileName = file.FileName;
                existingImage.filepath = Path.Combine(uploadsDirectory, Guid.NewGuid().ToString() + Path.GetExtension(file.FileName));
                var imageOutput = _mapper.Map<ImageOutputDto>(existingImage);
                await _context.SaveChangesAsync();
                return imageOutput;
            }
            else
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploadsDirectory, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                var newImage = new Image
                {
                    imageId = Guid.NewGuid(),
                    fileName = file.FileName,
                    filepath = filePath,
                    RecipeId = recipeId
                };

                _context.Images.Add(newImage);
                await _context.SaveChangesAsync();
                var imageOutput = _mapper.Map<ImageOutputDto>(newImage);
                return imageOutput;

            }
        }
    }
}
