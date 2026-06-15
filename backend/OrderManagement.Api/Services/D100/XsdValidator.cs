using System.Xml;
using System.Xml.Schema;
using Microsoft.Extensions.Options;
using OrderManagement.Api.Configuration;

namespace OrderManagement.Api.Services.D100;

public class XsdValidator(IOptions<D100Options> options, IWebHostEnvironment environment)
{
    private readonly D100Options _options = options.Value;

    public ValidationResult Validate(string xmlContent)
    {
        var errors = new List<string>();
        var xsdPath = ResolveXsdPath();
        if (!File.Exists(xsdPath))
        {
            return ValidationResult.Fail([$"Schema XSD lipsă: {xsdPath}"]);
        }

        var schemas = new XmlSchemaSet();
        schemas.Add("mfp:anaf:dgti:d100:declaratie:v2", xsdPath);

        var settings = new XmlReaderSettings
        {
            ValidationType = ValidationType.Schema,
            Schemas = schemas
        };
        settings.ValidationEventHandler += (_, e) =>
        {
            errors.Add(e.Message);
        };

        try
        {
            using var reader = XmlReader.Create(new StringReader(xmlContent), settings);
            while (reader.Read())
            {
            }
        }
        catch (Exception ex)
        {
            errors.Add(ex.Message);
        }

        return errors.Count == 0
            ? ValidationResult.Success()
            : ValidationResult.Fail(errors);
    }

    private string ResolveXsdPath()
    {
        var fromContent = Path.Combine(environment.ContentRootPath, "Anaf", "D100", "Schemas", _options.XsdFileName);
        if (File.Exists(fromContent))
        {
            return fromContent;
        }

        return Path.Combine(AppContext.BaseDirectory, "Anaf", "D100", "Schemas", _options.XsdFileName);
    }
}

public sealed class ValidationResult
{
    public bool IsValid { get; init; }
    public IReadOnlyList<string> Errors { get; init; } = [];

    public static ValidationResult Success() => new() { IsValid = true };

    public static ValidationResult Fail(IReadOnlyList<string> errors) =>
        new() { IsValid = false, Errors = errors };
}
