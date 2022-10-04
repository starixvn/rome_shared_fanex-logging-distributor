using EasyNetQ;
using Fanex.Logging.Distributor.Configuration;
using Fanex.Logging.Distributor.Helpers;
using Microsoft.Extensions.Options;

namespace Fanex.Logging.Distributor.Services;

public class ConsumeRabbitMQHostedService : BackgroundService
{
    private const string RoutingKey = "RabbitMQTraceListener.LogEntryQueueRouteKey";
    private readonly RabbitMQOptions options;
    private readonly ILogger<ConsumeRabbitMQHostedService> logger;
    private readonly ILogService logService;

    public ConsumeRabbitMQHostedService(IOptions<RabbitMQOptions> options, ILogger<ConsumeRabbitMQHostedService> logger, ILogService logService)
    {
        this.options = options.Value;
        this.logger = logger;
        this.logService = logService;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        InitRabbitMQ();
        return Task.CompletedTask;
    }

    public void InitRabbitMQ()
    {
        var rabbitMQConnectionString = EasyNetQConnectionStringCryptography.DecryptConnectionString(options.ConnectionString);
        using var rabbitMQBus = RabbitHutch.CreateBus(rabbitMQConnectionString);
        rabbitMQBus.SubscribeAsync<LogEntry>(RoutingKey, (logEntry) =>
        {
            if (logEntry != null)
            {
                return logService.WriteLog(logEntry);
            }

            return Task.CompletedTask;
        }, config => config.WithTopic(RoutingKey));
    }
}