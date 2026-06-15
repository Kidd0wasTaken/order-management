using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OrderManagement.Api.Configuration;
using OrderManagement.Api.Constants;
using OrderManagement.Api.Data;
using OrderManagement.Api.DTOs;
using OrderManagement.Api.Models;

namespace OrderManagement.Api.Services.D100;

public class D100CalculationService(
    AppDbContext db,
    IOptions<D100Options> options)
{
    private readonly D100Options _options = options.Value;

    public async Task<D100PreviewDto> PreviewAsync(int luna, int an, CancellationToken cancellationToken = default)
    {
        var result = await CalculateAsync(luna, an, cancellationToken);
        return new D100PreviewDto
        {
            Luna = luna,
            An = an,
            OrderCount = result.OrderCount,
            TotalSales = result.TotalSales,
            SumaDat = result.SumaDat,
            TotalPlataA = result.TotalPlataA,
            ObligationLine = result.ObligationLine,
            SimulationNote = _options.SimulationNote
        };
    }

    public async Task<D100Declaration> CreateOrUpdateDeclarationAsync(
        int luna,
        int an,
        CancellationToken cancellationToken = default)
    {
        var calculated = await CalculateAsync(luna, an, cancellationToken);
        var existing = await db.D100Declarations
            .FirstOrDefaultAsync(d => d.Luna == luna && d.An == an, cancellationToken);

        if (existing is null)
        {
            existing = new D100Declaration
            {
                Luna = luna,
                An = an,
                CreatedAt = DateTime.UtcNow
            };
            db.D100Declarations.Add(existing);
        }

        existing.OrderCount = calculated.OrderCount;
        existing.TotalSales = calculated.TotalSales;
        existing.SumaDat = calculated.SumaDat;
        existing.SumaDed = 0;
        existing.SumaPlata = calculated.SumaDat;
        existing.SumaRest = 0;
        existing.TotalPlataA = calculated.TotalPlataA;
        existing.CodOblig = _options.CodOblig;
        existing.CodBugetar = PadBudgetCode(_options.CodBugetar);
        existing.Scadenta = calculated.Scadenta;
        existing.NrEvidenta = calculated.NrEvidenta;
        existing.Status = D100DeclarationStatus.Draft;
        existing.ValidationErrors = null;
        existing.XmlPath = null;
        existing.PdfPath = null;
        existing.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(cancellationToken);
        return existing;
    }

    private async Task<CalculationResult> CalculateAsync(int luna, int an, CancellationToken cancellationToken)
    {
        var (start, end) = GetMonthRangeUtc(luna, an);
        var orders = await db.Orders
            .AsNoTracking()
            .Where(o => o.Status == OrderStatus.Completed && o.CreatedAt >= start && o.CreatedAt < end)
            .ToListAsync(cancellationToken);

        var totalSales = orders.Sum(o => o.Quantity * o.Price);
        var sumaDat = ToLeiInteger(totalSales);
        var totalPlataA = sumaDat;

        var company = await db.CompanyProfiles.AsNoTracking().FirstOrDefaultAsync(cancellationToken);
        var cui = company?.Cui ?? "12345678";

        return new CalculationResult
        {
            OrderCount = orders.Count,
            TotalSales = totalSales,
            SumaDat = sumaDat,
            TotalPlataA = totalPlataA,
            Scadenta = ComputeScadenta(luna, an),
            NrEvidenta = BuildNrEvidenta(cui, luna, an, _options.CodOblig),
            ObligationLine = new D100ObligationLineDto
            {
                CodOblig = _options.CodOblig,
                CodBugetar = PadBudgetCode(_options.CodBugetar),
                Scadenta = ComputeScadenta(luna, an),
                SumaDat = sumaDat,
                SumaPlata = sumaDat,
                NrEvidenta = BuildNrEvidenta(cui, luna, an, _options.CodOblig)
            }
        };
    }

    internal static (DateTime Start, DateTime End) GetMonthRangeUtc(int luna, int an)
    {
        var start = new DateTime(an, luna, 1, 0, 0, 0, DateTimeKind.Utc);
        var end = start.AddMonths(1);
        return (start, end);
    }

    internal static long ToLeiInteger(decimal amount) =>
        (long)Math.Round(amount, MidpointRounding.AwayFromZero);

    internal static string ComputeScadenta(int luna, int an)
    {
        var next = new DateTime(an, luna, 1).AddMonths(1);
        return $"25.{next.Month:D2}.{next.Year}";
    }

    internal static string PadBudgetCode(string code)
    {
        var trimmed = code.Trim();
        return trimmed.Length >= 10 ? trimmed[..10] : trimmed.PadRight(10, 'X');
    }

    internal static string BuildNrEvidenta(string cui, int luna, int an, string codOblig)
    {
        var cuiDigits = new string(cui.Where(char.IsDigit).ToArray());
        if (cuiDigits.Length == 0)
        {
            cuiDigits = "12345678";
        }

        var cod = codOblig.PadLeft(3, '0')[..3];
        var baseValue = $"{cod}{luna:D2}{an % 100:D2}{cuiDigits}";
        var digits = new string(baseValue.Where(char.IsDigit).ToArray());
        if (digits.Length > 23)
        {
            digits = digits[..23];
        }

        if (digits.Length == 0)
        {
            return "0";
        }

        if (digits.Length > 1)
        {
            digits = digits.TrimStart('0');
            if (digits.Length == 0)
            {
                digits = "0";
            }
        }

        return digits;
    }

    private sealed class CalculationResult
    {
        public int OrderCount { get; init; }
        public decimal TotalSales { get; init; }
        public long SumaDat { get; init; }
        public long TotalPlataA { get; init; }
        public string Scadenta { get; init; } = string.Empty;
        public string NrEvidenta { get; init; } = string.Empty;
        public D100ObligationLineDto ObligationLine { get; init; } = new();
    }
}
