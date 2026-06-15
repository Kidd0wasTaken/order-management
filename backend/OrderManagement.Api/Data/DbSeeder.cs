using Microsoft.EntityFrameworkCore;
using OrderManagement.Api.Constants;
using OrderManagement.Api.Models;

namespace OrderManagement.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken cancellationToken = default)
    {
        await SeedCompanyAsync(db, cancellationToken);
        await SeedOrdersAsync(db, cancellationToken);
    }

    private static async Task SeedCompanyAsync(AppDbContext db, CancellationToken cancellationToken)
    {
        if (await db.CompanyProfiles.AnyAsync(cancellationToken))
        {
            return;
        }

        db.CompanyProfiles.Add(new CompanyProfile
        {
            Cui = "12345678",
            Denumire = "SC CloudConta Demo SRL",
            Adresa = "Str. Exemplu nr. 10, București, Sector 1",
            Telefon = "0211234567",
            Email = "contact@cloudconta-demo.ro",
            NumeDeclar = "Popescu",
            PrenumeDeclar = "Ion",
            FunctieDeclar = "Administrator"
        });

        await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedOrdersAsync(AppDbContext db, CancellationToken cancellationToken)
    {
        if (await db.Orders.AnyAsync(cancellationToken))
        {
            return;
        }

        var now = DateTime.UtcNow;
        var currentMonthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

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
                CreatedAt = now.AddDays(-2)
            },
            new Order
            {
                CustomerName = "Maria Ionescu",
                Product = "Monitor LG 27\"",
                Quantity = 2,
                Price = 1299.50m,
                Status = OrderStatus.Processing,
                Notes = null,
                CreatedAt = now.AddDays(-1)
            },
            new Order
            {
                CustomerName = "SC Tech Solutions SRL",
                Product = "Licență Microsoft 365",
                Quantity = 10,
                Price = 89.99m,
                Status = OrderStatus.Completed,
                Notes = "Facturare pe firmă",
                CreatedAt = currentMonthStart.AddDays(2)
            },
            new Order
            {
                CustomerName = "Andrei Vasilescu",
                Product = "Servicii consultanță",
                Quantity = 1,
                Price = 2500m,
                Status = OrderStatus.Completed,
                Notes = null,
                CreatedAt = currentMonthStart.AddDays(10)
            },
            new Order
            {
                CustomerName = "SC Alfa Beta SRL",
                Product = "Abonament cloud",
                Quantity = 3,
                Price = 199.99m,
                Status = OrderStatus.Completed,
                Notes = "Luna curentă",
                CreatedAt = currentMonthStart.AddDays(15)
            }
        };

        db.Orders.AddRange(seedOrders);
        await db.SaveChangesAsync(cancellationToken);
    }
}
