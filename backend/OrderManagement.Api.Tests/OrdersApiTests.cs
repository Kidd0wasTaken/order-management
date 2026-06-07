using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Api.Constants;
using OrderManagement.Api.Data;
using OrderManagement.Api.DTOs;
using Xunit;

namespace OrderManagement.Api.Tests;

public class OrdersApiTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public OrdersApiTests(CustomWebApplicationFactory factory)
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
    public async Task GetAll_ReturnsOkAndEmptyList()
    {
        var response = await _client.GetAsync("/api/orders");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var orders = await response.Content.ReadFromJsonAsync<List<OrderResponseDto>>(JsonOptions);
        Assert.NotNull(orders);
        Assert.Empty(orders);
    }

    [Fact]
    public async Task Create_ValidOrder_ReturnsCreated()
    {
        var dto = new OrderUpsertDto
        {
            CustomerName = "Ana Test",
            Product = "Tastatură mecanică",
            Quantity = 2,
            Price = 299.99m,
            Status = OrderStatus.Pending,
            Notes = "Livrare rapidă"
        };

        var response = await _client.PostAsJsonAsync("/api/orders", dto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<OrderResponseDto>(JsonOptions);
        Assert.NotNull(created);
        Assert.True(created.Id > 0);
        Assert.Equal(dto.CustomerName, created.CustomerName);
        Assert.Equal(dto.Product, created.Product);
    }

    [Fact]
    public async Task Create_InvalidBody_ReturnsBadRequest()
    {
        var dto = new OrderUpsertDto
        {
            CustomerName = "",
            Product = "",
            Quantity = 0,
            Price = -1,
            Status = "InvalidStatus"
        };

        var response = await _client.PostAsJsonAsync("/api/orders", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("errors", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Update_NotFound_ReturnsNotFound()
    {
        var dto = new OrderUpsertDto
        {
            CustomerName = "Client",
            Product = "Produs",
            Quantity = 1,
            Price = 10,
            Status = OrderStatus.Pending
        };

        var response = await _client.PutAsJsonAsync("/api/orders/9999", dto);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ExistingOrder_ReturnsNoContent()
    {
        var createDto = new OrderUpsertDto
        {
            CustomerName = "De șters",
            Product = "Articol",
            Quantity = 1,
            Price = 50,
            Status = OrderStatus.Pending
        };

        var createResponse = await _client.PostAsJsonAsync("/api/orders", createDto);
        var created = await createResponse.Content.ReadFromJsonAsync<OrderResponseDto>(JsonOptions);
        Assert.NotNull(created);

        var deleteResponse = await _client.DeleteAsync($"/api/orders/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_NotFound_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync("/api/orders/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
