using System.Xml.Linq;
using EcbCurrencyRateGateway.Models;

namespace EcbCurrencyRateGateway.Implementation
{
    public class EcbGateway
    {
        private static readonly HttpClient httpClient = new HttpClient();

        private const string EcbUrl = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml";

        public async Task<List<CurrencyRate>> GetCurrencyRatesAsync()
        {
            var response = await httpClient.GetStringAsync(EcbUrl);
            return ParseCurrencyRates(response);
        }

        private List<CurrencyRate> ParseCurrencyRates(string xmlContent)
        {
            XDocument xmlDocument = XDocument.Parse(xmlContent);
            List<CurrencyRate> rates = new List<CurrencyRate>();

            var rateCube = xmlDocument.Descendants(XName.Get("Cube", "http://www.ecb.int/vocabulary/2002-08-01/eurofxref"))
                                       .Elements(XName.Get("Cube", "http://www.ecb.int/vocabulary/2002-08-01/eurofxref"))
                                       .FirstOrDefault();

            if (rateCube != null)
            {
                foreach (var element in rateCube.Elements(XName.Get("Cube", "http://www.ecb.int/vocabulary/2002-08-01/eurofxref")))
                {
                    var currency = element.Attribute("currency")?.Value;
                    var rate = element.Attribute("rate")?.Value;

                    if (currency != null && rate != null)
                    {
                        rates.Add(new CurrencyRate
                        {
                            Currency = currency,
                            Rate = rate,
                            Date = rateCube.Attribute("time")?.Value // Get the date from the outer Cube's attribute
                        });
                    }
                }
            }

            return rates;
        }
    }
}
