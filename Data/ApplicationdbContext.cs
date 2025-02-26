﻿using Microsoft.EntityFrameworkCore;
using let_em_cook.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace let_em_cook.Data
{
    public class ApplicationdbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationdbContext(DbContextOptions<ApplicationdbContext> options) : base(options)
        {
        }

        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Image> Images { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Vote>()
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<Comment>()
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<Review>()
                .Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETDATE()");
            
            
            
            modelBuilder.Entity<Recipe>()
                .HasOne(r => r.Chef)
                .WithMany(u => u.Recipes)
                .HasForeignKey(r => r.UserId);

            modelBuilder.Entity<Vote>()
                .HasOne(v => v.User)
                .WithMany(u => u.Votes)
                .HasForeignKey(v => v.UserId);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId);
            
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Recipe)
                .WithMany(r => r.Reviews)
                .HasForeignKey(r => r.RecipeId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict); 
            
            modelBuilder.Entity<Vote>()
                .HasOne(v => v.Recipe)
                .WithMany(r => r.Votes)
                .HasForeignKey(v => v.RecipeId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<ApplicationUser>().ToTable("ApplicationUser");
        }
    }
}