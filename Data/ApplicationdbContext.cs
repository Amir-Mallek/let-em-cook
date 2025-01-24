using Microsoft.EntityFrameworkCore;
using let_em_cook.Models;

namespace let_em_cook.Data
{
    public class ApplicationdbContext : DbContext
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
        }
    }
}