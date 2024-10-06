using CurrencyRateWorkerService.Interfaces;
using Microsoft.EntityFrameworkCore;
using Wallets.Exceptions;
using Wallets.Implementation.Context;
using Wallets.Interfaces.Services;
using Wallets.Types.DTOs;
using Wallets.Types.Enumerations;
using Wallets.Types.Models;
using Wallets.Utils;
using Wallets.Utils.Mapping;

namespace Wallets.Implementation.Services
{
    public class WalletService : IWalletService
    {
        private readonly bool EN_CACHE = true;
        private readonly WalletsDbContext _dbContext;
        private readonly ICurrencyRateCache _currencyRateCache;
        public WalletService(WalletsDbContext dbContext, ICurrencyRateCache currencyRateCache)
        {
            _dbContext = dbContext;
            _currencyRateCache = currencyRateCache;
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

                if (wallet == null) return new BalanceResponse { Balance = 0, Success = new BasicResponse { Success = false } };

                decimal? convertedBalance = wallet.Balance;

                if (!string.IsNullOrEmpty(currency) && currency != wallet.Currency) convertedBalance = await ConvertCurrency(wallet.Balance, currency);

                return new BalanceResponse { Balance = convertedBalance ?? 0, Success = new BasicResponse { Success = convertedBalance == null ? false : true } };
            }
            catch (Exception ex)
            {
                ex.Throw(ex?.Message ?? "Error while retrieving wallet balance.");
                return new BalanceResponse { Balance = 0, Success = new BasicResponse { Success = false } };
            }
        }

        public async Task<BalanceResponse> AdjustWalletBalance(long walletId, decimal amount, string currency, BalanceAdjustmentStrategiesEnum strategy)
        {
            if (amount <= 0) return new BalanceResponse { Balance = 0, Success = new BasicResponse { Success = false, Message = "The amount must be a positive number." } };

            var wallet = await _dbContext.Wallets.FindAsync(walletId);
            if (wallet == null) return new BalanceResponse { Balance = 0, Success = new BasicResponse { Success = false, Message = "Wallet not found." } };

            try
            {
                string? indication = await AdjustStrategy(wallet, amount, currency, strategy);
                await _dbContext.SaveChangesAsync();

                return new BalanceResponse { Balance = wallet?.Balance ?? 0, Success = new BasicResponse { Success = indication == null ? true : false, Message = indication ?? "Thank you for using our service." } };
            }
            catch (Exception ex)
            {
                ex.Throw(ex?.Message ?? "Error while adjusting wallet balance.");
                return new BalanceResponse { Balance = 0, Success = new BasicResponse { Success = false, Message = "Error while adjusting wallet balance." } };
            }
        }
        #endregion

        #region Private Methods
        // [@Maintenance_Practise]: Its not well practise to use more than 3 arguments -> Need to create a new class and refere to that,...
        private async Task<string?> AdjustStrategy(Wallet wallet, decimal amount, string currency, BalanceAdjustmentStrategiesEnum strategy)
        {
            string? res = null;
            amount = await ConvertCurrency(amount, currency) ?? 0;
            switch (strategy)
            {
                case BalanceAdjustmentStrategiesEnum.AddFundsStrategy:
                    wallet.Balance += amount;
                    break;

                case BalanceAdjustmentStrategiesEnum.SubtractFundsStrategy:
                    if (wallet.Balance >= amount) wallet.Balance -= amount;
                    else res = "Insufficient funds in the wallet.";
                    break;

                case BalanceAdjustmentStrategiesEnum.ForceSubtractFundsStrategy:
                    wallet.Balance -= amount;
                    break;

                default:
                    res = "Invalid adjustment strategy.";
                    break;
            }
            return res;
        }
        private async Task<decimal?> ConvertCurrency(decimal amount, string toCurrency)
        {
            string formattedDate = "2024-10-04"; // DateTime.Now.ToString("yyyy-MM-dd"); @TODO if the api is working and fetch each day data
            DateTime dateTime = DateTime.Parse(formattedDate);
            CurrencyRates? currencyRate = null;
            if (EN_CACHE)
            {
                currencyRate = await _currencyRateCache.GetCurrencyRateAsync(toCurrency, dateTime.Date);
            }
            else
            {
                currencyRate = await _dbContext.CurrencyRates
                .FirstOrDefaultAsync(cr => cr.Date == dateTime.Date && cr.Currency == toCurrency); // If we want to raise exception if there are more than one items with the same Data we can use Select or ... Thare are other handling options
            }

            if (currencyRate == null) return null;

            decimal convertedAmount = amount * currencyRate.Rate;

            return convertedAmount;
        }

        #endregion
    }
}