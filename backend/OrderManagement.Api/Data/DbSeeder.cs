using Microsoft.EntityFrameworkCore;
using OrderManagement.Api.Constants;
using OrderManagement.Api.Models;

namespace OrderManagement.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken cancellationToken = default)
    {
        if (await db.Orders.AnyAsync(cancellationToken))
        {
            return;
        }

        var seedOrders = new[]
        {
            new Order
            {
                CustomerName = "Ion Popescu",
                Product = "Laptop Dell XPS 15",
                Quantity = 1,
                Price = 7499.99m,
                Status = OrderStatus.Pending,
                Notes = "Livrare la sediu",
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new Order
            {
                CustomerName = "Maria Ionescu",
                Product = "Monitor LG 27\"",
                Quantity = 2,
                Price = 1299.50m,
                Status = OrderStatus.Processing,
                Notes = null,
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new Order
            {
                CustomerName = "SC Tech Solutions SRL",
                Product = "Licență Microsoft 365",
                Quantity = 10,
                Price = 89.99m,
                Status = OrderStatus.Completed,
                Notes = "Facturare pe firmă",
                CreatedAt = DateTime.UtcNow.AddHours(-6)
            }
        };

        db.Orders.AddRange(seedOrders);
        await db.SaveChangesAsync(cancellationToken);
    }
}
