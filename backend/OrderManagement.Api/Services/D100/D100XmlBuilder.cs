using System.Globalization;
using System.Text;
using System.Xml;
using OrderManagement.Api.Models;

namespace OrderManagement.Api.Services.D100;

public class D100XmlBuilder
{
    private const string NamespaceUri = "mfp:anaf:dgti:d100:declaratie:v2";

    public string Build(CompanyProfile company, D100Declaration declaration)
    {
        var settings = new XmlWriterSettings
        {
            Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
            Indent = true,
            OmitXmlDeclaration = false
        };

        using var stream = new MemoryStream();
        using (var writer = XmlWriter.Create(stream, settings))
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("declaratie100", NamespaceUri);
            writer.WriteAttributeString("luna", declaration.Luna.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("an", declaration.An.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("d_anulare", "0");
            writer.WriteAttributeString("nume_declar", company.NumeDeclar);
            writer.WriteAttributeString("prenume_declar", company.PrenumeDeclar);
            writer.WriteAttributeString("functie_declar", company.FunctieDeclar);
            writer.WriteAttributeString("cui", company.Cui);
            writer.WriteAttributeString("den", company.Denumire);
            writer.WriteAttributeString("adresa", company.Adresa);

            if (!string.IsNullOrWhiteSpace(company.Telefon))
            {
                writer.WriteAttributeString("telefon", company.Telefon);
            }

            if (!string.IsNullOrWhiteSpace(company.Email))
            {
                writer.WriteAttributeString("mai", company.Email);
            }

            writer.WriteAttributeString("totalPlata_A", declaration.TotalPlataA.ToString(CultureInfo.InvariantCulture));

            writer.WriteStartElement("obligatie", NamespaceUri);
            writer.WriteAttributeString("cod_oblig", declaration.CodOblig);
            writer.WriteAttributeString("cod_bugetar", declaration.CodBugetar);
            writer.WriteAttributeString("scadenta", declaration.Scadenta);
            writer.WriteAttributeString("nr_evid", declaration.NrEvidenta);
            writer.WriteAttributeString("suma_dat", declaration.SumaDat.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("suma_ded", declaration.SumaDed.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("suma_plata", declaration.SumaPlata.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("suma_rest", declaration.SumaRest.ToString(CultureInfo.InvariantCulture));
            writer.WriteEndElement();

            writer.WriteEndElement();
            writer.WriteEndDocument();
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }
}
