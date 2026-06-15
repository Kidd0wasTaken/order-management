namespace OrderManagement.Api.Models;

public class D100Declaration
{
    public int Id { get; set; }
    public int Luna { get; set; }
    public int An { get; set; }
    public D100DeclarationStatus Status { get; set; } = D100DeclarationStatus.Draft;
    public int OrderCount { get; set; }
    public decimal TotalSales { get; set; }
    public long SumaDat { get; set; }
    public long SumaDed { get; set; }
    public long SumaPlata { get; set; }
    public long SumaRest { get; set; }
    public long TotalPlataA { get; set; }
    public string CodOblig { get; set; } = "103";
    public string CodBugetar { get; set; } = string.Empty;
    public string Scadenta { get; set; } = string.Empty;
    public string NrEvidenta { get; set; } = string.Empty;
    public string? XmlPath { get; set; }
    public string? PdfPath { get; set; }
    public string? ValidationErrors { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
