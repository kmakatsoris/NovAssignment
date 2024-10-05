using EcbCurrencyRateGateway.Implementation;
using EcbCurrencyRateGateway.Models;

class Program
{
    static async Task Main(string[] args)
    {
        EcbGateway ecbGateway = new EcbGateway();
        List<CurrencyRate> rates = await ecbGateway.GetCurrencyRatesAsync();

        foreach (var rate in rates)
        {
            Console.WriteLine($"Currency: {rate.Currency}, Rate: {rate.Rate}, Date: {rate.Date}");
        }
    }
}
