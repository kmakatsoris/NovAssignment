using Newtonsoft.Json;
using Wallets.Types.DTOs;
using Wallets.Types.Enumerations;
using Wallets.Types.Models;

namespace Wallets.Utils.Mapping
{
    public static class CurrencyRateMapper
    {
        #region Map CurrencyRateModel To CurrencyRateDTO
        public static CurrencyRateDTO ToMap(this CurrencyRates model)
        {
            if (model == null ||
                DefaultUtils.TryGet(model?.Currency, out CurrencyEnum currency) != null) return new CurrencyRateDTO();
            CurrencyRateDTO t = new CurrencyRateDTO
            {
                Currency = currency,
                Rate = model?.Rate ?? 0,
                Date = model?.Date ?? DateTime.Now
            };
            return t;
        }
        #endregion

        #region Map CurrencyRateDTO To CurrencyRateModel
        public static CurrencyRates ToMap(this CurrencyRateDTO dto)
        {
            if (dto == null) return new CurrencyRates();
            CurrencyRates t = new CurrencyRates
            {
                Currency = dto?.Currency.ToString() ?? "",
                Rate = dto?.Rate ?? 0,
                Date = dto?.Date ?? DateTime.Now
            };
            return t;
        }
        #endregion
    }
}