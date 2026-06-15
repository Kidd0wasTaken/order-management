using Microsoft.Extensions.Options;
using OrderManagement.Api.Configuration;
using OrderManagement.Api.Models;
using OrderManagement.Api.Services.D100;
using Xunit;

namespace OrderManagement.Api.Tests;

public class D100XmlBuilderTests
{
    private readonly D100XmlBuilder _builder = new();

    [Fact]
    public void Build_ContainsRequiredElementsAndTotals()
    {
        var company = new CompanyProfile
        {
            Cui = "12345678",
            Denumire = "Demo SRL",
            Adresa = "Str. Test 1",
            Telefon = "0712345678",
            Email = "demo@test.ro",
            NumeDeclar = "Popescu",
            PrenumeDeclar = "Ion",
            FunctieDeclar = "Administrator"
        };

        var declaration = new D100Declaration
        {
            Luna = 6,
            An = 2025,
            SumaDat = 1500,
            SumaDed = 0,
            SumaPlata = 1500,
            SumaRest = 0,
            TotalPlataA = 1500,
            CodOblig = "103",
            CodBugetar = "20A010101X",
            Scadenta = "25.07.2025",
            NrEvidenta = "103062512345678"
        };

        var xml = _builder.Build(company, declaration);

        Assert.Contains("declaratie100", xml);
        Assert.Contains("totalPlata_A=\"1500\"", xml);
        Assert.Contains("suma_dat=\"1500\"", xml);
        Assert.Contains("cod_oblig=\"103\"", xml);
        Assert.Contains("cui=\"12345678\"", xml);
        Assert.Contains("mai=\"demo@test.ro\"", xml);
    }
}

public class XsdValidatorTests
{
    [Fact]
    public void Validate_ValidXml_Passes()
    {
        var builder = new D100XmlBuilder();
        var validator = CreateValidator();

        var company = new CompanyProfile
        {
            Cui = "12345678",
            Denumire = "Demo SRL",
            Adresa = "Str. Test 1, Bucuresti",
            NumeDeclar = "Popescu",
            PrenumeDeclar = "Ion",
            FunctieDeclar = "Administrator"
        };

        var declaration = new D100Declaration
        {
            Luna = 6,
            An = 2025,
            SumaDat = 100,
            SumaPlata = 100,
            TotalPlataA = 100,
            CodOblig = "103",
            CodBugetar = "20A010101X",
            Scadenta = "25.07.2025",
            NrEvidenta = "103062512345678"
        };

        var xml = builder.Build(company, declaration);
        var result = validator.Validate(xml);

        Assert.True(result.IsValid, string.Join("; ", result.Errors));
    }

    [Fact]
    public void Validate_InvalidXml_Fails()
    {
        var validator = CreateValidator();
        var invalid = """
            <?xml version="1.0" encoding="utf-8"?>
            <declaratie100 xmlns="mfp:anaf:dgti:d100:declaratie:v2" luna="13" an="2025" d_anulare="0"
              nume_declar="X" prenume_declar="Y" functie_declar="Z" cui="1" den="D" adresa="A" totalPlata_A="1">
              <obligatie cod_oblig="103" cod_bugetar="20A010101X" scadenta="25.07.2025" nr_evid="1"
                suma_dat="1" suma_ded="0" suma_plata="1" suma_rest="0" />
            </declaratie100>
            """;

        var result = validator.Validate(invalid);

        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
    }

    private static XsdValidator CreateValidator()
    {
        var contentRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "OrderManagement.Api"));
        var environment = new TestWebHostEnvironment { ContentRootPath = contentRoot };
        return new XsdValidator(Options.Create(new D100Options()), environment);
    }

    private sealed class TestWebHostEnvironment : Microsoft.AspNetCore.Hosting.IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = "Tests";
        public Microsoft.Extensions.FileProviders.IFileProvider WebRootFileProvider { get; set; } = null!;
        public string WebRootPath { get; set; } = string.Empty;
        public string EnvironmentName { get; set; } = "Testing";
        public string ContentRootPath { get; set; } = string.Empty;
        public Microsoft.Extensions.FileProviders.IFileProvider ContentRootFileProvider { get; set; } = null!;
    }
}
