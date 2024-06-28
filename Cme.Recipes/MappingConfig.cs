using AutoMapper;
using Cme.Recipes.Models;
using Cme.Recipes.Models.Dto;
using RecipeApp.Domain.Entities;

namespace Microservices.Services.CouponAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<RecipeOutputDto, Recipe>()
                    .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.Ingredients))
                    .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image));

                config.CreateMap<Recipe, RecipeOutputDto>()
                    .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.Ingredients))
                    .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image));
                
                config.CreateMap<AllRecipesOutputDto, Recipe>()
                    .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image));

                config.CreateMap<Recipe, AllRecipesOutputDto>()
                    .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image));

                config.CreateMap<Recipe, RecipeInputDto>();

                config.CreateMap<RecipeInputDto, Recipe>();

                config.CreateMap<IngredientInputDto, Ingredient>();

                config.CreateMap<Ingredient, IngredientInputDto>();

                config.CreateMap<IngredientOutputDto, Ingredient>();

                config.CreateMap<Ingredient, IngredientOutputDto>();

                config.CreateMap<Image,ImageUploadDto>();
                config.CreateMap<ImageUploadDto, Image>();
                config.CreateMap<Image,ImageOutputDto>();
                config.CreateMap<ImageOutputDto, Image>();

            });

            return mappingConfig;
        }
    }
}
