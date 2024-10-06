using Microsoft.EntityFrameworkCore;
using Wallets.Exceptions;
using Wallets.Implementation.Context;
using Wallets.Interfaces.Services;
using Wallets.Types.DTOs;
using Wallets.Types.Models;
using Wallets.Utils.Mapping;

namespace Wallets.Implementation.Services
{
    public class WalletService : IWalletService
    {
        private readonly WalletsDbContext _dbContext;
        public WalletService(WalletsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #region Public Methods
        public async Task<BasicResponse> CreateWallet(WalletDTO wallet)
        {
            if (wallet == null || wallet.Balance < 0) return new BasicResponse() { Success = false };
            try
            {
                _dbContext.Wallets.Add(WalletMapper.ToMap(wallet));
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null &&
                    ex.InnerException.Message.Contains("Duplicate entry") &&
                    ex.InnerException.Message.Contains("for key 'Wallets.PRIMARY'")) return new BasicResponse() { Message = "This Entry Id already exists. Please provide a new one.", Success = false };
                ex.Throw(ex?.Message ?? "Error while save changes to the db");
                return new BasicResponse() { Message = "Error while save changes to the db", Success = false };
            }
            return new BasicResponse() { Success = true };
        }

        public async Task<BalanceResponse> RetrieveWalletBalance(long walletId, string currency)
        {
            try
            {
                Wallet? wallet = await _dbContext.Wallets.FindAsync(walletId);

                if (wallet == null) return new BalanceResponse { Balance = 0, Success = false };

                decimal? convertedBalance = wallet.Balance;

                if (!string.IsNullOrEmpty(currency) && currency != wallet.Currency) convertedBalance = await ConvertCurrency(wallet.Balance, wallet.Currency, currency);

                return new BalanceResponse { Balance = convertedBalance ?? 0, Success = convertedBalance == null ? false : true };
            }
            catch (Exception ex)
            {
                ex.Throw(ex?.Message ?? "Error while retrieving wallet balance.");
                return new BalanceResponse { Balance = 0, Success = false };
            }
        }
        #endregion

        #region Private Methods
        private async Task<decimal?> ConvertCurrency(decimal amount, string fromCurrency, string toCurrency)
        {
            string formattedDate = "2024-10-04"; // DateTime.Now.ToString("yyyy-MM-dd");
            DateTime dateTime = DateTime.Parse(formattedDate);
            CurrencyRates? currencyRate = await _dbContext.CurrencyRates
                .FirstOrDefaultAsync(cr => cr.Date == dateTime.Date && cr.Currency == toCurrency); // If we want to raise exception if there are more than one items with the same Data we can use Select or ... Thare are other handling options

            if (currencyRate == null) return null;

            decimal convertedAmount = amount * currencyRate.Rate;

            return convertedAmount;
        }

        #endregion
    }
}