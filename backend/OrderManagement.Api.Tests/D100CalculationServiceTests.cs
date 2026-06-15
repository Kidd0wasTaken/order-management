using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrderManagement.Api.Configuration;
using OrderManagement.Api.Constants;
using OrderManagement.Api.Data;
using OrderManagement.Api.Models;
using OrderManagement.Api.Services.D100;
using Xunit;

namespace OrderManagement.Api.Tests;

public class D100CalculationServiceTests
{
    [Theory]
    [InlineData(6, 2025, "25.07.2025")]
    [InlineData(12, 2025, "25.01.2026")]
    public void ComputeScadenta_ReturnsNextMonth25th(int luna, int an, string expected)
    {
        Assert.Equal(expected, D100CalculationService.ComputeScadenta(luna, an));
    }

    [Fact]
    public void ToLeiInteger_RoundsAwayFromZero()
    {
        Assert.Equal(100L, D100CalculationService.ToLeiInteger(99.5m));
        Assert.Equal(100L, D100CalculationService.ToLeiInteger(100.4m));
    }

    [Fact]
    public async Task PreviewAsync_SumsOnlyCompletedOrdersInMonth()
    {
        await using var db = CreateDb();
        var luna = 3;
        var an = 2025;
        var (start, _) = D100CalculationService.GetMonthRangeUtc(luna, an);

        db.Orders.AddRange(
            new Order
            {
                CustomerName = "A",
                Product = "P1",
                Quantity = 2,
                Price = 50,
                Status = OrderStatus.Completed,
                CreatedAt = start.AddDays(1)
            },
            new Order
            {
                CustomerName = "B",
                Product = "P2",
                Quantity = 1,
                Price = 100,
                Status = OrderStatus.Completed,
                CreatedAt = start.AddDays(10)
            },
            new Order
            {
                CustomerName = "C",
                Product = "P3",
                Quantity = 5,
                Price = 10,
                Status = OrderStatus.Pending,
                CreatedAt = start.AddDays(2)
            },
            new Order
            {
                CustomerName = "D",
                Product = "P4",
                Quantity = 1,
                Price = 999,
                Status = OrderStatus.Completed,
                CreatedAt = start.AddMonths(1)
            });
        await db.SaveChangesAsync();

        var service = new D100CalculationService(db, Options.Create(new D100Options()));
        var preview = await service.PreviewAsync(luna, an);

        Assert.Equal(2, preview.OrderCount);
        Assert.Equal(200m, preview.TotalSales);
        Assert.Equal(200L, preview.SumaDat);
        Assert.Equal(200L, preview.TotalPlataA);
    }

    private static AppDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }
}
