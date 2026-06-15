namespace OrderManagement.Api.Configuration;

public class D100Options
{
    public const string SectionName = "D100";

    public string CodOblig { get; set; } = "103";
    public string CodBugetar { get; set; } = "20A010101X";
    public string TipOblig { get; set; } = "1";
    public string SimulationNote { get; set; } = "suma_dat = total vanzari comenzi finalizate (demo)";
    public string DukIntegratorUrl { get; set; } = "http://dukintegrator:8080";
    public string ExportRoot { get; set; } = "/exports";
    public string XsdFileName { get; set; } = "d100.xsd";
}
