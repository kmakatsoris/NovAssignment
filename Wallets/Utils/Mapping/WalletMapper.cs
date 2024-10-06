using Newtonsoft.Json;
using Wallets.Types.DTOs;
using Wallets.Types.Enumerations;
using Wallets.Types.Models;

namespace Wallets.Utils.Mapping
{
    public static class WalletMapper
    {
        #region Map WalletModel To WalletDTO
        public static WalletDTO ToMap(this Wallet model)
        {
            if (model == null) return new WalletDTO();
            WalletDTO t = new WalletDTO
            {
                Id = model?.Id ?? 0,
                Balance = model?.Balance ?? 0,
                Currency = model?.Currency ?? CurrencyEnum.USD.ToString()
            };
            return t;
        }
        #endregion

        #region Map CurrencyRateDTO To CurrencyRateModel
        public static Wallet ToMap(this WalletDTO dto)
        {
            if (dto == null) return new Wallet();
            Wallet t = new Wallet
            {
                Id = dto?.Id ?? 0,
                Balance = dto?.Balance ?? 0,
                Currency = dto?.Currency ?? CurrencyEnum.USD.ToString()
            };
            return t;
        }
        #endregion
    }
}