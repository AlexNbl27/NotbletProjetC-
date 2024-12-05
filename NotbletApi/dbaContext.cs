using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notblet.Models;

public class dbaContext : DbContext
{
    public dbaContext(DbContextOptions<dbaContext> options) : base(options) { }

    public DbSet<ProductModel> products { get; set; }
    public DbSet<CategoryModel> categories { get; set; }
    public DbSet<UserModel> users { get; set; }
    public DbSet<OrderModel> orders { get; set; }
    public DbSet<ClientModel> clients { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Application des configurations explicites des entités
        modelBuilder.ApplyConfiguration(new ProductEntityConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
        modelBuilder.ApplyConfiguration(new OrderEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ClientEntityConfiguration());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source = database.db");
            SQLitePCL.Batteries.Init();
            SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());
        }
    }
}

// Configuration explicite pour ProductModel
public class ProductEntityConfiguration : IEntityTypeConfiguration<ProductModel>
{
    public void Configure(EntityTypeBuilder<ProductModel> builder)
    {
        builder.HasIndex(e => e.id).IsUnique();
        builder.HasOne(e => e.category)
            .WithMany()
            .HasForeignKey(e => e.category_id)
            .OnDelete(DeleteBehavior.SetNull); // Configurer les relations ici
    }
}

// Configuration explicite pour CategoryModel
public class CategoryEntityConfiguration : IEntityTypeConfiguration<CategoryModel>
{
    public void Configure(EntityTypeBuilder<CategoryModel> builder)
    {
        builder.HasIndex(e => e.id).IsUnique();
        builder.HasIndex(e => e.name).IsUnique(); // Exemple de configuration unique pour name
    }
}

// Configuration explicite pour UserModel
public class UserEntityConfiguration : IEntityTypeConfiguration<UserModel>
{
    public void Configure(EntityTypeBuilder<UserModel> builder)
    {
        builder.HasIndex(e => e.id).IsUnique();
        builder.HasIndex(e => e.username).IsUnique(); // Configuration unique pour username
    }
}

// Configuration explicite pour OrderModel
public class OrderEntityConfiguration : IEntityTypeConfiguration<OrderModel>
{
    public void Configure(EntityTypeBuilder<OrderModel> builder)
    {
        builder.HasIndex(e => e.id).IsUnique();
        builder.HasOne(o => o.client)
            .WithMany()
            .HasForeignKey(o => o.client_id)
            .OnDelete(DeleteBehavior.Cascade); // Configurer la relation client - Order

        builder.HasOne(o => o.product)
            .WithMany()
            .HasForeignKey(o => o.product_id)
            .OnDelete(DeleteBehavior.Cascade); // Configurer la relation produit - Order
    }
}

// Configuration explicite pour ClientModel
public class ClientEntityConfiguration : IEntityTypeConfiguration<ClientModel>
{
    public void Configure(EntityTypeBuilder<ClientModel> builder)
    {
        builder.HasIndex(e => e.id).IsUnique();
        builder.HasIndex(e => e.name).IsUnique(); // Exemple de configuration unique pour name
    }
}

