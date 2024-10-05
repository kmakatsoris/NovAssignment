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

            foreach (var element in xmlDocument.Descendants("Cube").Elements("Cube").Elements("Rate"))
            {
                rates.Add(new CurrencyRate
                {
                    Currency = element.Attribute("currency").Value,
                    Rate = element.Value,
                    Date = xmlDocument.Root.Attribute("date").Value
                });
            }

            return rates;
        }
    }
}
