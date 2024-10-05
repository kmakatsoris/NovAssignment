using Microsoft.Extensions.Configuration;

namespace CurrencyRateWorkerService;
public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly EcbGateway _ecbGateway;
    private readonly int _fetchInterval;

    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        _logger = logger;
        _ecbGateway = new EcbGateway();
        _fetchInterval = configuration.GetValue<int>("Worker:FetchIntervalMinutes");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            try
            {
                List<CurrencyRate> rates = await _ecbGateway.GetCurrencyRatesAsync();
                foreach (var rate in rates)
                {
                    _logger.LogInformation($"Currency: {rate.Currency}, Rate: {rate.Rate}, Date: {rate.Date}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching currency rates");
            }

            await Task.Delay(TimeSpan.FromMinutes(_fetchInterval), stoppingToken);
        }
    }
}
