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
                    .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.Ingredients));

                config.CreateMap<Recipe, RecipeOutputDto>()
                    .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.Ingredients));

                config.CreateMap<Recipe, RecipeInputDto>();

                config.CreateMap<RecipeInputDto, Recipe>();

                config.CreateMap<IngredientInputDto, Ingredient>();

                config.CreateMap<Ingredient, IngredientInputDto>();

                config.CreateMap<IngredientOutputDto, Ingredient>();

                config.CreateMap<Ingredient, IngredientOutputDto>();
            });

            return mappingConfig;
        }
    }
}
