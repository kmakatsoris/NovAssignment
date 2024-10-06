using CurrencyRateWorkerService.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Threading.Tasks;
using Wallets.Types.Models;

public class RedisCurrencyRateCache : ICurrencyRateCache
{
    private readonly IDistributedCache _distributedCache;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(30);
    private readonly string PREFIX_CACHE = "Wallet_Cache_";

    public RedisCurrencyRateCache(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }
    public async Task<CurrencyRates?> GetCurrencyRateAsync(string currency, DateTime date)
    {
        string cacheKey = GenerateCacheKey(currency, date.ToString("yyyy-MM-dd"));
        var cachedRate = await _distributedCache.GetStringAsync(cacheKey);
        if (cachedRate != null)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            CurrencyRates? currencyRate = JsonSerializer.Deserialize<CurrencyRates>(cachedRate, options);

            return currencyRate;
        }
        return null;
    }

    public async Task SetCurrencyRateAsync(CurrencyRates rate)
    {
        string cacheKey = GenerateCacheKey(rate.Currency, rate.Date.ToString("yyyy-MM-dd"));
        var serializedRate = JsonSerializer.Serialize(rate);
        await _distributedCache.SetStringAsync(cacheKey, serializedRate, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _cacheExpiration
        });
    }

    private string GenerateCacheKey(string currency, string date)
    {
        return $"{PREFIX_CACHE}{currency}_{date}";
    }
}
