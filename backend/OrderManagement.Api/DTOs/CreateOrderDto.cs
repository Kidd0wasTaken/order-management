using System.ComponentModel.DataAnnotations;
using OrderManagement.Api.Constants;

namespace OrderManagement.Api.DTOs;

public class CreateOrderDto
{
    [Required(ErrorMessage = "Numele clientului este obligatoriu.")]
    [StringLength(200, ErrorMessage = "Numele clientului nu poate depăși 200 de caractere.")]
    public string CustomerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Produsul este obligatoriu.")]
    [StringLength(200, ErrorMessage = "Produsul nu poate depăși 200 de caractere.")]
    public string Product { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Cantitatea trebuie să fie cel puțin 1.")]
    public int Quantity { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Prețul nu poate fi negativ.")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Statusul este obligatoriu.")]
    public string Status { get; set; } = OrderStatus.Pending;

    [StringLength(1000, ErrorMessage = "Notele nu pot depăși 1000 de caractere.")]
    public string? Notes { get; set; }
}
