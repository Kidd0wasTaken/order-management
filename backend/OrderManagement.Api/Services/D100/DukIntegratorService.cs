using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using OrderManagement.Api.Configuration;

namespace OrderManagement.Api.Services.D100;

public class DukIntegratorService(
    HttpClient httpClient,
    IOptions<D100Options> options,
    D100ExportStorage exportStorage)
{
    private readonly D100Options _options = options.Value;

    public async Task<DukPdfResult> GeneratePdfAsync(int declarationId, CancellationToken cancellationToken = default)
    {
        var xmlPath = exportStorage.GetXmlPath(declarationId);
        if (!File.Exists(xmlPath))
        {
            return DukPdfResult.Fail("Fișierul XML nu există. Generați mai întâi declarația.");
        }

        var outputDir = exportStorage.GetDeclarationDirectory(declarationId);
        var request = new DukPdfRequest
        {
            XmlPath = ToContainerPath(xmlPath),
            OutputDir = ToContainerPath(outputDir)
        };

        try
        {
            var response = await httpClient.PostAsJsonAsync(
                $"{_options.DukIntegratorUrl.TrimEnd('/')}/generate-pdf",
                request,
                cancellationToken);

            var body = await response.Content.ReadFromJsonAsync<DukPdfResponse>(cancellationToken: cancellationToken);
            if (!response.IsSuccessStatusCode || body is null || !body.Success)
            {
                var message = body?.Error ?? $"DUKIntegrator a răspuns cu status {(int)response.StatusCode}";
                return DukPdfResult.Fail(message);
            }

            var pdfPath = exportStorage.GetPdfPath(declarationId);
            if (!File.Exists(pdfPath) && !string.IsNullOrWhiteSpace(body.PdfPath))
            {
                pdfPath = FromContainerPath(body.PdfPath);
            }

            if (!File.Exists(pdfPath))
            {
                return DukPdfResult.Fail("PDF-ul nu a fost generat de serviciul DUKIntegrator.");
            }

            return DukPdfResult.Ok(pdfPath, body.UsedDukJar);
        }
        catch (HttpRequestException ex)
        {
            return DukPdfResult.Fail($"Nu s-a putut contacta DUKIntegrator: {ex.Message}");
        }
    }

    private static string ToContainerPath(string hostPath) =>
        hostPath.Replace('\\', '/');

    private static string FromContainerPath(string containerPath) =>
        containerPath.Replace('/', Path.DirectorySeparatorChar);

    private sealed class DukPdfRequest
    {
        [JsonPropertyName("xmlPath")]
        public string XmlPath { get; set; } = string.Empty;

        [JsonPropertyName("outputDir")]
        public string OutputDir { get; set; } = string.Empty;
    }
}

public sealed class DukPdfResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("pdfPath")]
    public string? PdfPath { get; set; }

    [JsonPropertyName("error")]
    public string? Error { get; set; }

    [JsonPropertyName("usedDukJar")]
    public bool UsedDukJar { get; set; }
}

public sealed class DukPdfResult
{
    public bool Success { get; init; }
    public string? PdfPath { get; init; }
    public string? Error { get; init; }
    public bool UsedDukJar { get; init; }

    public static DukPdfResult Ok(string pdfPath, bool usedDukJar) =>
        new() { Success = true, PdfPath = pdfPath, UsedDukJar = usedDukJar };

    public static DukPdfResult Fail(string error) =>
        new() { Success = false, Error = error };
}
