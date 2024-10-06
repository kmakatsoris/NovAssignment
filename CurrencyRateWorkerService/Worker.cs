using CurrencyRateWorkerService.Interfaces;
using EcbCurrencyRateGateway.Implementation;
using EcbCurrencyRateGateway.Models;
using MySql.Data.MySqlClient;

namespace CurrencyRateWorkerService;
public class Worker : BackgroundService
{
    private readonly bool EN_CACHE = true;
    private readonly ILogger<Worker> _logger;
    private readonly EcbGateway _ecbGateway;
    private readonly int _fetchInterval;
    private readonly string _connectionString;
    private readonly ICurrencyRateCache _currencyRateCache;

    public Worker(ILogger<Worker> logger, IConfiguration configuration, string connectionString, ICurrencyRateCache currencyRateCache)
    {
        _logger = logger;
        _ecbGateway = new EcbGateway();
        _fetchInterval = configuration.GetValue<int>("Worker:FetchIntervalMinutes");
        _connectionString = connectionString;
        _currencyRateCache = currencyRateCache;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            try
            {
                List<CurrencyRate> rates = await _ecbGateway.GetCurrencyRatesAsync();
                if (rates.Any())
                {
                    await UpdateDatabaseAsync(rates);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching currency rates: {ex?.Message}");
            }

            await Task.Delay(TimeSpan.FromMinutes(_fetchInterval), stoppingToken);
        }
    }

    private async Task UpdateDatabaseAsync(List<CurrencyRate> rates)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            using (var transaction = await connection.BeginTransactionAsync())
            { // I have installed MySQL in my Ubuntu Machine so thats why i dont use the merge. Although i you use SQL Server instead: 
                /*
                var command = new SqlCommand(@"
                          MERGE INTO CurrencyRates AS target
                          USING (SELECT @Date AS Date, @Currency AS Currency, @Rate AS Rate) AS source
                          ON target.Date = source.Date AND target.Currency = source.Currency
                          WHEN MATCHED THEN
                              UPDATE SET Rate = source.Rate
                          WHEN NOT MATCHED THEN
                              INSERT (Date, Currency, Rate)
                              VALUES (source.Date, source.Currency, source.Rate);", connection, transaction);
                */
                foreach (var rate in rates)
                {
                    ExecuteCachingMechanism(rate);
                    var command = new MySqlCommand(@"
                    INSERT INTO CurrencyRates (Date, Currency, Rate)
                    VALUES (@Date, @Currency, @Rate)
                    ON DUPLICATE KEY UPDATE Rate = @Rate;", connection, transaction);

                    command.Parameters.AddWithValue("@Date", rate.Date);
                    command.Parameters.AddWithValue("@Currency", rate.Currency);
                    command.Parameters.AddWithValue("@Rate", rate.Rate);

                    await command.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
            }
        }
    }

    private void ExecuteCachingMechanism(CurrencyRate rate)
    {
        if (EN_CACHE == true && rate != null)
        {
            _currencyRateCache.SetCurrencyRateAsync(rate);
        }
    }

}
