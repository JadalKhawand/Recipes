using Cme.Recipes.Models;
using Microsoft.EntityFrameworkCore;
using RecipeApp.Domain.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Cme.Recipes.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
    }
}












