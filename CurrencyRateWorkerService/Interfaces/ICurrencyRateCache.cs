using EcbCurrencyRateGateway.Models;

namespace CurrencyRateWorkerService.Interfaces
{
    public interface ICurrencyRateCache
    {
        Task<CurrencyRate?> GetCurrencyRateAsync(string currency, DateTime date);
        Task SetCurrencyRateAsync(CurrencyRate rate);
    }

}