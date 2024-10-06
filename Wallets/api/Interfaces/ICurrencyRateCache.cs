using Wallets.Types.Models;

namespace CurrencyRateWorkerService.Interfaces
{
    public interface ICurrencyRateCache
    {
        Task<CurrencyRates?> GetCurrencyRateAsync(string currency, DateTime date);
        Task SetCurrencyRateAsync(CurrencyRates rate);
    }

}