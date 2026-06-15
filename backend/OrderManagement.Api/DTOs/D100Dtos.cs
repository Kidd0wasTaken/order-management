using System.ComponentModel.DataAnnotations;
using OrderManagement.Api.Models;

namespace OrderManagement.Api.DTOs;

public class CompanyProfileDto
{
    [Required]
    [StringLength(13)]
    public string Cui { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Denumire { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string Adresa { get; set; } = string.Empty;

    [StringLength(15)]
    public string? Telefon { get; set; }

    [EmailAddress]
    [StringLength(200)]
    public string? Email { get; set; }

    [Required]
    [StringLength(75)]
    public string NumeDeclar { get; set; } = string.Empty;

    [Required]
    [StringLength(75)]
    public string PrenumeDeclar { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string FunctieDeclar { get; set; } = string.Empty;
}

public class D100PeriodDto
{
    [Range(1, 12)]
    public int Luna { get; set; }

    [Range(2016, 2100)]
    public int An { get; set; }
}

public class D100ObligationLineDto
{
    public string CodOblig { get; set; } = string.Empty;
    public string CodBugetar { get; set; } = string.Empty;
    public string Scadenta { get; set; } = string.Empty;
    public long SumaDat { get; set; }
    public long SumaPlata { get; set; }
    public string NrEvidenta { get; set; } = string.Empty;
}

public class D100PreviewDto
{
    public int Luna { get; set; }
    public int An { get; set; }
    public int OrderCount { get; set; }
    public decimal TotalSales { get; set; }
    public long SumaDat { get; set; }
    public long TotalPlataA { get; set; }
    public D100ObligationLineDto ObligationLine { get; set; } = new();
    public string SimulationNote { get; set; } = string.Empty;
}

public class D100DeclarationDto
{
    public int Id { get; set; }
    public int Luna { get; set; }
    public int An { get; set; }
    public string Status { get; set; } = string.Empty;
    public int OrderCount { get; set; }
    public decimal TotalSales { get; set; }
    public long SumaDat { get; set; }
    public long TotalPlataA { get; set; }
    public string? ValidationErrors { get; set; }
    public bool HasXml { get; set; }
    public bool HasPdf { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public static class D100Mapping
{
    public static CompanyProfileDto ToDto(this CompanyProfile entity) => new()
    {
        Cui = entity.Cui,
        Denumire = entity.Denumire,
        Adresa = entity.Adresa,
        Telefon = entity.Telefon,
        Email = entity.Email,
        NumeDeclar = entity.NumeDeclar,
        PrenumeDeclar = entity.PrenumeDeclar,
        FunctieDeclar = entity.FunctieDeclar
    };

    public static void ApplyUpdate(this CompanyProfile entity, CompanyProfileDto dto)
    {
        entity.Cui = dto.Cui.Trim();
        entity.Denumire = dto.Denumire.Trim();
        entity.Adresa = dto.Adresa.Trim();
        entity.Telefon = string.IsNullOrWhiteSpace(dto.Telefon) ? null : dto.Telefon.Trim();
        entity.Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim();
        entity.NumeDeclar = dto.NumeDeclar.Trim();
        entity.PrenumeDeclar = dto.PrenumeDeclar.Trim();
        entity.FunctieDeclar = dto.FunctieDeclar.Trim();
    }

    public static D100DeclarationDto ToDto(this D100Declaration entity) => new()
    {
        Id = entity.Id,
        Luna = entity.Luna,
        An = entity.An,
        Status = entity.Status.ToString(),
        OrderCount = entity.OrderCount,
        TotalSales = entity.TotalSales,
        SumaDat = entity.SumaDat,
        TotalPlataA = entity.TotalPlataA,
        ValidationErrors = entity.ValidationErrors,
        HasXml = !string.IsNullOrEmpty(entity.XmlPath),
        HasPdf = !string.IsNullOrEmpty(entity.PdfPath),
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt
    };
}
