﻿using RecipeApp.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Cme.Recipes.Models.Dto
{
    public class RecipeOutputDto
    {
        public Guid RecipeId { get; set; }

        public required string Name { get; set; }

        public required string Description { get; set; }

        public int Calories { get; set; }

        public required string PrepTime { get; set; }

        public required ImageOutputDto Image { get; set; }

        public required string Category { get; set; }

        public List<Ingredient> Ingredients { get; set; } = new List<Ingredient>();

        public required string Procedures { get; set; }
    }
}
