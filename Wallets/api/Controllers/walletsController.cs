using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallets.Exceptions;
using Wallets.Interfaces.Services;
using Wallets.Types.DTOs;
using Wallets.Types.Enumerations;
using Wallets.Types.Models;

namespace Wallets.Controllers.Wallet
{
    [Route("api/[controller]")]
    [ApiController]
    public class walletsController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public walletsController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpPost()]
        public async Task<BasicResponse> CreateWallet([FromBody] WalletDTO wallet)
        {
            return await DefaultException.ExceptionControllerHandler(async () =>
            {
                return await _walletService?.CreateWallet(wallet);
            });
        }

        [HttpGet("{walletId}")]
        public async Task<BalanceResponse> RetrieveWalletBalance(int walletId, [FromQuery] string currency = "")
        {
            return await DefaultException.ExceptionControllerHandler(async () =>
            {
                return await _walletService?.RetrieveWalletBalance(walletId, currency);
            });
        }

        [HttpPost("{walletId}/adjustbalance")]
        public async Task<BalanceResponse> AdjustWalletBalance(long walletId, [FromQuery] decimal amount, [FromQuery] string currency, [FromQuery] BalanceAdjustmentStrategiesEnum strategy)
        {
            return await DefaultException.ExceptionControllerHandler(async () =>
            {
                return await _walletService?.AdjustWalletBalance(walletId, amount, currency, strategy);
            });
        }
    }
}