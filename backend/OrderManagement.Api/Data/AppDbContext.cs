using Microsoft.EntityFrameworkCore;
using OrderManagement.Api.Models;

namespace OrderManagement.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var order = modelBuilder.Entity<Order>();

        order.Property(o => o.CustomerName)
            .HasMaxLength(200)
            .IsRequired();

        order.Property(o => o.Product)
            .HasMaxLength(200)
            .IsRequired();

        order.Property(o => o.Quantity)
            .IsRequired();

        order.Property(o => o.Price)
            .HasPrecision(18, 2);

        order.Property(o => o.Status)
            .HasMaxLength(50)
            .IsRequired();

        order.Property(o => o.Notes)
            .HasMaxLength(1000);

        order.Property(o => o.CreatedAt)
            .IsRequired();

        order.HasIndex(o => o.CreatedAt);
    }
}
