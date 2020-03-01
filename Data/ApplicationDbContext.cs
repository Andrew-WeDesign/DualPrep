using System;
using System.Collections.Generic;
using System.Text;
using DualPrep.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DualPrep.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Meal> Meals { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<MealFavorite> MealFavorites { get; set; }
        public DbSet<ExerciseFavorite> ExerciseFavorites { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Meal>().ToTable("Meal");
            modelBuilder.Entity<ApplicationUser>().ToTable("ApplicationUser");
            modelBuilder.Entity<Exercise>().ToTable("Exercise");
            modelBuilder.Entity<MealFavorite>().ToTable("MealFavorite");
            modelBuilder.Entity<ExerciseFavorite>().ToTable("ExerciseFavorite");
            base.OnModelCreating(modelBuilder);
        }

    }
}
