using CurrencyRateWorkerService.Interfaces;
using Newtonsoft.Json;
using Wallets.Types.Models;
using ILogger = NLog.ILogger;

namespace Wallets.Middlewares
{
    public class RateLimitingMiddlewareOptions
    {
        public RequestDelegate Next { get; set; }
        public ILogger Logger { get; set; }
        public ICurrencyRateCache CurrencyRateCache { get; set; }
    }
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly ICurrencyRateCache _currencyRateCache;
        private readonly int THRESHOLD_OF_CALLS = 10;

        public RateLimitingMiddleware(RequestDelegate next, RateLimitingMiddlewareOptions options)
        {
            _next = next;
            _logger = options.Logger;
            _currencyRateCache = options.CurrencyRateCache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                string txt = "Validation Error. Can not find information for the calling IP.";
                if (context?.Connection?.RemoteIpAddress != null)
                {
                    string ip = context.Connection.RemoteIpAddress.ToString();
                    RateLimiting? rateLimiting = await _currencyRateCache.GetRateLimitingAsync(ip);
                    if (rateLimiting != null && rateLimiting.NumberOfCalls <= THRESHOLD_OF_CALLS)
                    {
                        await _currencyRateCache.SetRateLimitingAsync(new RateLimiting { IP = ip, NumberOfCalls = rateLimiting.NumberOfCalls + 1 });
                        await _next(context);
                        return;
                    }
                    else if (rateLimiting == null)
                    {
                        await _currencyRateCache.SetRateLimitingAsync(new RateLimiting { IP = ip, NumberOfCalls = 1 });
                        await _next(context);
                        return;
                    }
                    else
                    {
                        txt = $"Validation Error. The IP:{rateLimiting?.IP} has executed so many calls.";
                    }
                }
                _logger.Error(txt);
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync(txt);
            }
            catch (Exception ex)
            {
                _logger.Error($"Internal Error Happened Inside The RateLimitingMiddleware\n\nException: {ex.Message}");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync(ex?.Message ?? "Internal Error Happened");
            }
        }
    }
}