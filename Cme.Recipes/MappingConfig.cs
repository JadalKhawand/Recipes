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
                config.CreateMap<RecipeDto, Recipe>()
                    .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.Ingredients));

                config.CreateMap<Recipe, RecipeDto>()
                    .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.Ingredients)); 

                config.CreateMap<IngredientDto, Ingredient>();

                config.CreateMap<Ingredient, IngredientDto>();
            });

            return mappingConfig;
        }
    }
}
