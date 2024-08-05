using SPMA.Models.Production;
using System.IO;
using System.Text;
using System.Xml;

namespace SPMA.Core.Tools
{
    public class XmlGenerator
    {

        public string GenerateRw(InProductionXML[] wares, string rwDate, string orderNumber, string subOrderNumber, string componentNumber, string rwType, string magType)
        {
            XmlDocument xmlDocument = new XmlDocument();

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = ("    ");
            settings.CloseOutput = true;
            settings.OmitXmlDeclaration = true;

            string rwDescription = orderNumber + " ; " + subOrderNumber + " ; " + componentNumber + " ; " + rwType;

            StringBuilder builder = new StringBuilder();
            using (StringWriter stringWriter = new StringWriter(builder)) {
                using (XmlTextWriter writer = new XmlTextWriter(stringWriter))
                {
                    writer.WriteStartElement("ROOT", "http://www.cdn.com.pl/optima/dokument");

                    writer.WriteStartElement("DOKUMENT");

                    writer.WriteStartElement("NAGLOWEK");

                    writer.WriteElementString("GENERATOR", "Comarch Opt!ma");
                    writer.WriteElementString("TYP_DOKUMENTU", "304");
                    writer.WriteElementString("RODZAJ_DOKUMENTU", "304000");
                    writer.WriteElementString("FV_MARZA", "0");
                    writer.WriteElementString("FV_MARZA_RODZAJ", "0");
                    writer.WriteElementString("NUMER_PELNY", " "); // uzupelnic
                    writer.WriteElementString("DATA_DOKUMENTU", rwDate); // uzupelnic
                    writer.WriteElementString("DATA_WYSTAWIENIA", rwDate); // uzupelnic
                    writer.WriteElementString("DATA_OPERACJI", rwDate); // uzupelnic
                    writer.WriteElementString("TERMIN_ZWROTU_KAUCJI", " "); // uzupelnic
                    writer.WriteElementString("KOREKTA", "0");
                    writer.WriteElementString("DETAL", "0");
                    writer.WriteElementString("TYP_NETTO_BRUTTO", "1");
                    writer.WriteElementString("RABAT", "0.00");
                    writer.WriteElementString("OPIS", rwDescription); // uzupelnic

                    writer.WriteStartElement("PLATNIK");
                    writer.WriteElementString("KOD", "!NIEOKREŚLONY!");
                    writer.WriteElementString("NIP_KRAJ", "");
                    writer.WriteElementString("NIP", "");
                    writer.WriteElementString("GLN", "");
                    writer.WriteElementString("NAZWA", "");
                    writer.WriteStartElement("ADRES");
                    writer.WriteElementString("KOD_POCZTOWY", "");
                    writer.WriteElementString("MIASTO", "");
                    writer.WriteElementString("ULICA", "");
                    writer.WriteElementString("KRAJ", "");
                    writer.WriteEndElement();
                    writer.WriteEndElement();

                    writer.WriteStartElement("ODBIORCA");
                    writer.WriteElementString("KOD", "!NIEOKREŚLONY!");
                    writer.WriteElementString("NIP_KRAJ", "");
                    writer.WriteElementString("NIP", "");
                    writer.WriteElementString("GLN", "");
                    writer.WriteElementString("NAZWA", "");
                    writer.WriteStartElement("ADRES");
                    writer.WriteElementString("KOD_POCZTOWY", "");
                    writer.WriteElementString("MIASTO", "");
                    writer.WriteElementString("ULICA", "");
                    writer.WriteElementString("KRAJ", "");
                    writer.WriteEndElement();
                    writer.WriteEndElement();

                    writer.WriteStartElement("SPRZEDAWCA");
                    writer.WriteElementString("NIP_KRAJ", "PL");
                    writer.WriteElementString("NIP", "873-020-41-85");
                    writer.WriteElementString("GLN", "");
                    writer.WriteElementString("NAZWA", "SPÓŁKA STOLARCZYK Tomasz Stolarczyk, Janusz Stolarczyk");
                    writer.WriteStartElement("ADRES");
                    writer.WriteElementString("KOD_POCZTOWY", "33-100");
                    writer.WriteElementString("MIASTO", "Tarnów");
                    writer.WriteElementString("ULICA", "Kochanowskiego 30");
                    writer.WriteElementString("KRAJ", "Polska");
                    writer.WriteEndElement();
                    writer.WriteElementString("NUMER_KONTA_BANKOWEGO", "");
                    writer.WriteElementString("NAZWA_BANKU", "");
                    writer.WriteEndElement();

                    //writer.WriteStartElement("KATEGORIA");
                    //writer.WriteElementString("KOD", "");
                    //writer.WriteElementString("OPIS", ""); 
                    //writer.WriteEndElement();

                    //writer.WriteStartElement("PLATNOSC");
                    //writer.WriteElementString("FORMA", "gotówka");
                    //writer.WriteElementString("TERMIN", " "); // uzupelnic
                    //writer.WriteEndElement();

                    //writer.WriteStartElement("WALUTA");
                    //writer.WriteElementString("SYMBOL", "PLN");
                    //writer.WriteElementString("KURS_L", "1.00");
                    //writer.WriteElementString("KURS_M", "1");
                    //writer.WriteElementString("PLAT_WAL_OD_PLN", "0");
                    //writer.WriteElementString("KURS_NUMER", "3");
                    //writer.WriteElementString("KURS_DATA", " "); // uzupelnic
                    //writer.WriteEndElement();

                    //writer.WriteStartElement("KWOTY"); // ?????
                    //writer.WriteElementString("RAZEM_NETTO_WAL", "");
                    //writer.WriteElementString("RAZEM_NETTO", "");
                    //writer.WriteElementString("RAZEM_BRUTTO", "");
                    //writer.WriteElementString("RAZEM_VAT", "");
                    //writer.WriteEndElement();

                    writer.WriteElementString("MAGAZYN_ZRODLOWY", magType); // uzupelnic ??
                    //writer.WriteElementString("MAGAZYN_DOCELOWY", ""); // uzupelnic ??
                    //writer.WriteElementString("KAUCJE_PLATNOSCI", "0"); // uzupelnic ??
                    //writer.WriteElementString("BLOKADA_PLATNOSCI", "1"); // uzupelnic ??
                    //writer.WriteElementString("VAT_DLA_DOK_WAL", "0"); // uzupelnic ??
                    //writer.WriteElementString("TRYB_NETTO_VAT", "0"); // uzupelnic ??

                    writer.WriteEndElement();

                    writer.WriteStartElement("POZYCJE");


                    // =============== SZABLON POZYCJI =============== 
                    int index = 1;
                    foreach (InProductionXML item in wares)
                    {
                        //foreach (ComponentWareDto ware in item.ComponentWareDtos)
                        //{

                        writer.WriteStartElement("POZYCJA");
                        writer.WriteElementString("LP", index.ToString());

                        writer.WriteStartElement("TOWAR");
                        writer.WriteElementString("KOD", item.WareCode);
                        writer.WriteElementString("NAZWA", item.WareName);
                        writer.WriteElementString("OPIS", "");
                        writer.WriteElementString("EAN", "");
                        writer.WriteElementString("SWW", "");
                        writer.WriteElementString("NUMER_KATALOGOWY", "");
                        writer.WriteEndElement();

                        writer.WriteStartElement("STAWKA_VAT");
                        writer.WriteElementString("STAWKA", "0.00"); // uzupelnic ???
                        writer.WriteElementString("FLAGA", "0"); // uzupelnic ???
                        writer.WriteElementString("ZRODLOWA", "0.00"); // uzupelnic ???
                        writer.WriteEndElement();

                        //    //writer.WriteStartElement("CENY");
                        //    //writer.WriteElementString("CENAZCZTEREMAMIEJSCAMI", "0"); // uzupelnic ???
                        //    //writer.WriteElementString("POCZATKOWA_WAL_CENNIKA", "2.7500"); // uzupelnic ???
                        //    //writer.WriteElementString("POCZATKOWA_WAL_DOKUMENTU", "2.7500"); // uzupelnic ???
                        //    //writer.WriteElementString("PO_RABACIE_WAL_CENNIKA", "2.7500"); // uzupelnic ???
                        //    //writer.WriteElementString("PO_RABACIE_PLN", "2.7500"); // uzupelnic ???
                        //    //writer.WriteElementString("PO_RABACIE_WAL_DOKUMENTU", "2.7500"); // uzupelnic ???
                        //    //writer.WriteEndElement();

                        //    //writer.WriteStartElement("WALUTA");
                        //    //writer.WriteElementString("SYMBOL", "PLN"); // uzupelnic ???
                        //    //writer.WriteElementString("KURS_L", "1.00"); // uzupelnic ???
                        //    //writer.WriteElementString("KURS_M", "1"); // uzupelnic ???
                        //    //writer.WriteEndElement();

                        //    //writer.WriteElementString("RABAT", "0.00"); // uzupelnic ???
                        //    //writer.WriteElementString("WARTOSC_NETTO", ""); // uzupelnic ???
                        //    //writer.WriteElementString("WARTOSC_BRUTTO", ""); // uzupelnic ???
                        //    //writer.WriteElementString("WARTOSC_NETTO_WAL", ""); // uzupelnic ???
                        //    //writer.WriteElementString("WARTOSC_BRUTTO_WAL", ""); // uzupelnic ???
                        writer.WriteElementString("ILOSC", item.ToIssue.ToString()); 
                        //    //writer.WriteElementString("JM", "kg"); // uzupelnic ???
                        //    //writer.WriteElementString("JM_CALKOWITE", "0.00"); // uzupelnic ???

                        //    //writer.WriteStartElement("JM_ZLOZONA");
                        //    //writer.WriteElementString("JMZ", ""); // uzupelnic ???
                        //    //writer.WriteElementString("JM_PRZELICZNIK_L", ""); // uzupelnic ???
                        //    //writer.WriteElementString("JM_PRZELICZNIK_M", ""); // uzupelnic ???
                        //    //writer.WriteEndElement();

                        writer.WriteEndElement();
                        //}
                        index++;
                    }
                    // =============== SZABLON POZYCJI =============== 

                    writer.WriteEndElement();

                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.Flush();

                }
                return builder.ToString();
            }
            //var ns = XNamespace.Get("http://www.cdn.com.pl/optima/dokument");
            //XNamespace xmlns = "";
            //var doc = new XDocument(new XDeclaration("1.0", "utf-8", null));
            //var root = new XElement(ns + "ROOT",
            //    new XElement(ns + "DOKUMENT",
            //    new XElement(ns + "NAGLOWEK","h"
            //    )));
            //doc.Add(root);
            
            //Stream stream = new MemoryStream();

            
            //doc.Save(stream);
            
        }
    }
}
