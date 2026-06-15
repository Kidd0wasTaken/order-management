using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Api.Constants;
using OrderManagement.Api.Data;
using OrderManagement.Api.DTOs;
using OrderManagement.Api.Models;
using Xunit;

namespace OrderManagement.Api.Tests;

public class D100ApiTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public D100ApiTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Generate_WithoutCompany_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/d100/generate", new D100PeriodDto { Luna = 6, An = 2025 });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Preview_ReturnsAggregatedTotals()
    {
        await SeedCompanyAndOrdersAsync();

        var response = await _client.PostAsJsonAsync("/api/d100/preview", new D100PeriodDto { Luna = 6, An = 2025 });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var preview = await response.Content.ReadFromJsonAsync<D100PreviewDto>(JsonOptions);
        Assert.NotNull(preview);
        Assert.Equal(2, preview.OrderCount);
        Assert.Equal(300L, preview.SumaDat);
    }

    [Fact]
    public async Task Generate_ValidPeriod_ReturnsValidatedDeclarationWithXml()
    {
        await SeedCompanyAndOrdersAsync();

        var response = await _client.PostAsJsonAsync("/api/d100/generate", new D100PeriodDto { Luna = 6, An = 2025 });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var declaration = await response.Content.ReadFromJsonAsync<D100DeclarationDto>(JsonOptions);
        Assert.NotNull(declaration);
        Assert.True(declaration.HasXml);
        Assert.Equal("Validated", declaration.Status);

        var xmlResponse = await _client.GetAsync($"/api/d100/{declaration.Id}/xml");
        Assert.Equal(HttpStatusCode.OK, xmlResponse.StatusCode);
        Assert.Equal("application/xml", xmlResponse.Content.Headers.ContentType?.MediaType);
    }

    private async Task SeedCompanyAndOrdersAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        db.CompanyProfiles.Add(new CompanyProfile
        {
            Cui = "12345678",
            Denumire = "Demo SRL",
            Adresa = "Str. Test 1",
            NumeDeclar = "Popescu",
            PrenumeDeclar = "Ion",
            FunctieDeclar = "Administrator"
        });

        var start = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc);
        db.Orders.AddRange(
            new Order
            {
                CustomerName = "A",
                Product = "P1",
                Quantity = 2,
                Price = 100,
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
                CreatedAt = start.AddDays(5)
            });

        await db.SaveChangesAsync();
    }
}
