using Domain.Models;
using Microsoft.EntityFrameworkCore;


namespace Repository;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<InventoryItem> InventoryItems { get; set; }
    public DbSet<WishlistItem> WishlistItems { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<IngredientReaction> IngredientReactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Fluent API configs go here (composite keys, table names, etc.)
    }
}