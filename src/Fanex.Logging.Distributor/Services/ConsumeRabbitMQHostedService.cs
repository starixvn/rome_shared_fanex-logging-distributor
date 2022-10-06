using EasyNetQ;
using Fanex.Logging.Distributor.Configuration;
using Fanex.Logging.Distributor.Helpers;
using Microsoft.Extensions.Options;

namespace Fanex.Logging.Distributor.Services;

public class ConsumeRabbitMQHostedService : BackgroundService
{
    private const string RoutingKey = "RabbitMQTraceListener.LogEntryQueueRouteKey";
    private readonly RabbitMQOptions options;
    private readonly ILogger logger;
    private readonly ILogService logService;

    public ConsumeRabbitMQHostedService(IOptions<RabbitMQOptions> options, ILogger logger, ILogService logService)
    {
        this.options = options.Value;
        this.logger = logger;
        this.logService = logService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await InitRabbitMQ();
    }

    public async Task InitRabbitMQ()
    {
        await logger.InfoAsync(options.ConnectionString);
        var rabbitMQConnectionString = EasyNetQConnectionStringCryptography.DecryptConnectionString(options.ConnectionString);
        await logger.InfoAsync(rabbitMQConnectionString);
        using var rabbitMQBus = RabbitHutch.CreateBus(rabbitMQConnectionString);
        rabbitMQBus.SubscribeAsync<LogEntry>(RoutingKey, async (logEntry) =>
        {
            await logger.InfoAsync($"Received");
            if (logEntry != null)
            {
                await logService.WriteLog(logEntry);
            }
        }, config => config.WithTopic(RoutingKey));
    }
}