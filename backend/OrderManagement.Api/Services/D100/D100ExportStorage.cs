using Microsoft.Extensions.Options;
using OrderManagement.Api.Configuration;

namespace OrderManagement.Api.Services.D100;

public class D100ExportStorage(IOptions<D100Options> options)
{
    private readonly D100Options _options = options.Value;

    public string GetDeclarationDirectory(int declarationId)
    {
        var path = Path.Combine(_options.ExportRoot, "d100", declarationId.ToString());
        Directory.CreateDirectory(path);
        return path;
    }

    public async Task<string> WriteXmlAsync(int declarationId, string xmlContent, CancellationToken cancellationToken = default)
    {
        var dir = GetDeclarationDirectory(declarationId);
        var filePath = Path.Combine(dir, "D100.xml");
        await File.WriteAllTextAsync(filePath, xmlContent, cancellationToken);
        return filePath;
    }

    public string GetXmlPath(int declarationId) =>
        Path.Combine(GetDeclarationDirectory(declarationId), "D100.xml");

    public string GetPdfPath(int declarationId) =>
        Path.Combine(GetDeclarationDirectory(declarationId), "D100.pdf");
}
