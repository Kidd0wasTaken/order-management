using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Api.Data;
using OrderManagement.Api.DTOs;
using OrderManagement.Api.Models;
using OrderManagement.Api.Services.D100;

namespace OrderManagement.Api.Controllers;

[ApiController]
[Route("api/d100")]
[Produces("application/json")]
public class D100Controller(
    AppDbContext db,
    D100CalculationService calculationService,
    D100XmlBuilder xmlBuilder,
    XsdValidator xsdValidator,
    D100ExportStorage exportStorage,
    DukIntegratorService dukIntegratorService) : ControllerBase
{
    [HttpGet("company")]
    [ProducesResponseType(typeof(CompanyProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CompanyProfileDto>> GetCompany(CancellationToken cancellationToken)
    {
        var company = await db.CompanyProfiles.AsNoTracking().FirstOrDefaultAsync(cancellationToken);
        if (company is null)
        {
            return NotFound(new { message = "Profilul companiei nu este configurat." });
        }

        return Ok(company.ToDto());
    }

    [HttpPut("company")]
    [ProducesResponseType(typeof(CompanyProfileDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CompanyProfileDto>> UpsertCompany(
        [FromBody] CompanyProfileDto dto,
        CancellationToken cancellationToken)
    {
        var company = await db.CompanyProfiles.FirstOrDefaultAsync(cancellationToken);
        if (company is null)
        {
            company = new CompanyProfile();
            db.CompanyProfiles.Add(company);
        }

        company.ApplyUpdate(dto);
        await db.SaveChangesAsync(cancellationToken);
        return Ok(company.ToDto());
    }

    [HttpPost("preview")]
    [ProducesResponseType(typeof(D100PreviewDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<D100PreviewDto>> Preview(
        [FromBody] D100PeriodDto dto,
        CancellationToken cancellationToken)
    {
        var preview = await calculationService.PreviewAsync(dto.Luna, dto.An, cancellationToken);
        return Ok(preview);
    }

    [HttpPost("generate")]
    [ProducesResponseType(typeof(D100DeclarationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<D100DeclarationDto>> Generate(
        [FromBody] D100PeriodDto dto,
        CancellationToken cancellationToken)
    {
        var company = await db.CompanyProfiles.FirstOrDefaultAsync(cancellationToken);
        if (company is null)
        {
            return BadRequest(new { message = "Configurați mai întâi profilul companiei." });
        }

        var declaration = await calculationService.CreateOrUpdateDeclarationAsync(dto.Luna, dto.An, cancellationToken);
        var xml = xmlBuilder.Build(company, declaration);
        var validation = xsdValidator.Validate(xml);

        if (!validation.IsValid)
        {
            declaration.Status = D100DeclarationStatus.Failed;
            declaration.ValidationErrors = JsonSerializer.Serialize(validation.Errors);
            declaration.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync(cancellationToken);
            return BadRequest(new
            {
                message = "XML-ul generat nu trece validarea XSD.",
                errors = validation.Errors,
                declaration = declaration.ToDto()
            });
        }

        var xmlPath = await exportStorage.WriteXmlAsync(declaration.Id, xml, cancellationToken);
        declaration.XmlPath = xmlPath;
        declaration.Status = D100DeclarationStatus.Validated;
        declaration.ValidationErrors = null;
        declaration.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);

        return Ok(declaration.ToDto());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(D100DeclarationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<D100DeclarationDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var declaration = await db.D100Declarations.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        if (declaration is null)
        {
            return NotFound(new { message = $"Declarația cu ID {id} nu a fost găsită." });
        }

        return Ok(declaration.ToDto());
    }

    [HttpGet("{id:int}/xml")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadXml(int id, CancellationToken cancellationToken)
    {
        var declaration = await db.D100Declarations.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        if (declaration is null || string.IsNullOrWhiteSpace(declaration.XmlPath) || !System.IO.File.Exists(declaration.XmlPath))
        {
            return NotFound(new { message = "Fișierul XML nu este disponibil." });
        }

        var bytes = await System.IO.File.ReadAllBytesAsync(declaration.XmlPath, cancellationToken);
        return File(bytes, "application/xml", $"D100_{declaration.An}_{declaration.Luna:D2}.xml");
    }

    [HttpPost("{id:int}/pdf")]
    [ProducesResponseType(typeof(D100DeclarationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<D100DeclarationDto>> GeneratePdf(int id, CancellationToken cancellationToken)
    {
        var declaration = await db.D100Declarations.FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        if (declaration is null)
        {
            return NotFound(new { message = $"Declarația cu ID {id} nu a fost găsită." });
        }

        if (string.IsNullOrWhiteSpace(declaration.XmlPath))
        {
            return BadRequest(new { message = "Generați mai întâi XML-ul declarației." });
        }

        var result = await dukIntegratorService.GeneratePdfAsync(id, cancellationToken);
        if (!result.Success)
        {
            declaration.Status = D100DeclarationStatus.Failed;
            declaration.ValidationErrors = JsonSerializer.Serialize(new[] { result.Error });
            declaration.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync(cancellationToken);
            return BadRequest(new { message = result.Error });
        }

        declaration.PdfPath = result.PdfPath;
        declaration.Status = D100DeclarationStatus.PdfGenerated;
        declaration.ValidationErrors = null;
        declaration.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync(cancellationToken);

        return Ok(declaration.ToDto());
    }

    [HttpGet("{id:int}/pdf")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadPdf(int id, CancellationToken cancellationToken)
    {
        var declaration = await db.D100Declarations.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        if (declaration is null || string.IsNullOrWhiteSpace(declaration.PdfPath) || !System.IO.File.Exists(declaration.PdfPath))
        {
            return NotFound(new { message = "Fișierul PDF nu este disponibil." });
        }

        var bytes = await System.IO.File.ReadAllBytesAsync(declaration.PdfPath, cancellationToken);
        return File(bytes, "application/pdf", $"D100_{declaration.An}_{declaration.Luna:D2}.pdf");
    }
}
