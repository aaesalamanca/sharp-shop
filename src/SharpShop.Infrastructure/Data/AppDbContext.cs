using Microsoft.EntityFrameworkCore;
using SharpShop.Domain.Entities;

namespace SharpShop.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public AppDbContext() { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSeeding(
            (context, isUpdate) =>
            {
                if (!context.Set<Book>().Any())
                {
                    var books = new List<Book>
                    {
                        new("Clean Code", "Robert C. Martin", 45.00m),
                        new("The Pragmatic Programmer", "David Thomas", 42.50m),
                        new("Design Patterns", "Gang of Four", 38.00m),
                        new("Refactoring", "Martin Fowler", 35.00m),
                        new("Domain-Driven Design", "Eric Evans", 48.00m),
                    };

                    foreach (var book in books)
                    {
                        context.Set<Book>().Add(book);
                    }

                    context.SaveChanges();
                }
            }
        );
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired();
            entity.Property(e => e.Author).IsRequired();
            entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Status).HasConversion<string>();

            entity
                .HasMany(e => e.Items)
                .WithOne()
                .HasForeignKey("OrderId")
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => new { e.OrderId, e.BookId });
            entity.Property(e => e.Quantity).IsRequired();

            entity
                .HasOne(e => e.Book)
                .WithMany()
                .HasForeignKey(e => e.BookId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    public static async Task SeedAsync(AppDbContext context)
    {
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }
}
