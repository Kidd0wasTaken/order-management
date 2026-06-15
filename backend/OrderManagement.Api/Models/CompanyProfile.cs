namespace OrderManagement.Api.Models;

public class CompanyProfile
{
    public int Id { get; set; }
    public string Cui { get; set; } = string.Empty;
    public string Denumire { get; set; } = string.Empty;
    public string Adresa { get; set; } = string.Empty;
    public string? Telefon { get; set; }
    public string? Email { get; set; }
    public string NumeDeclar { get; set; } = string.Empty;
    public string PrenumeDeclar { get; set; } = string.Empty;
    public string FunctieDeclar { get; set; } = string.Empty;
}
