using Wallets.Types.DTOs;
using Wallets.Types.Models;

namespace Wallets.Interfaces.Services
{
    public interface IWalletService
    {
        Task<BasicResponse> CreateWallet(WalletDTO wallet);
        Task<BalanceResponse> RetrieveWalletBalance(long walletId, string currency);
    }
}