namespace OrderManagement.Api.DTOs;

public record OrderResponseDto(
    int Id,
    string CustomerName,
    string Product,
    int Quantity,
    decimal Price,
    string Status,
    string? Notes,
    DateTime CreatedAt);
