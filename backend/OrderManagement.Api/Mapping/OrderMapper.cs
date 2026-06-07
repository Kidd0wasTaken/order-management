using OrderManagement.Api.Constants;
using OrderManagement.Api.DTOs;
using OrderManagement.Api.Models;

namespace OrderManagement.Api.Mapping;

public static class OrderMapper
{
    public static OrderResponseDto ToResponseDto(this Order order) =>
        new(
            order.Id,
            order.CustomerName,
            order.Product,
            order.Quantity,
            order.Price,
            order.Status,
            order.Notes,
            order.CreatedAt);

    public static Order ToEntity(this CreateOrderDto dto) =>
        new()
        {
            CustomerName = dto.CustomerName.Trim(),
            Product = dto.Product.Trim(),
            Quantity = dto.Quantity,
            Price = dto.Price,
            Status = dto.Status,
            Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim(),
            CreatedAt = DateTime.UtcNow
        };

    public static void ApplyUpdate(this Order order, UpdateOrderDto dto)
    {
        order.CustomerName = dto.CustomerName.Trim();
        order.Product = dto.Product.Trim();
        order.Quantity = dto.Quantity;
        order.Price = dto.Price;
        order.Status = dto.Status;
        order.Notes = string.IsNullOrWhiteSpace(dto.Notes) ? null : dto.Notes.Trim();
    }

    public static bool TryValidateStatus(string status, out string? error)
    {
        if (OrderStatus.IsValid(status))
        {
            error = null;
            return true;
        }

        error = $"Status invalid. Valori permise: {string.Join(", ", OrderStatus.All)}.";
        return false;
    }
}
