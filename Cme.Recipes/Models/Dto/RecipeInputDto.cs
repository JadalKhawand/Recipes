﻿using RecipeApp.Domain.Entities;

namespace Cme.Recipes.Models.Dto
{
    public class RecipeInputDto
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public int Calories { get; set; }
        public required string PrepTime { get; set; }
        public required string Category { get; set; }
        public List<IngredientInputDto> Ingredients { get; set; } = new List<IngredientInputDto>();
        public required string Procedures { get; set; }
    }
}
