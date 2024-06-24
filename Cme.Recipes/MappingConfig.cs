using AutoMapper;
using Cme.Recipes.Models;
using Cme.Recipes.Models.Dto;
using static Azure.Core.HttpHeader;

namespace Microservices.Services.CouponAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<RecipeDto, Recipe>();
                config.CreateMap<Recipe, RecipeDto>();
            });
            return mappingConfig;
        }
    }
}
