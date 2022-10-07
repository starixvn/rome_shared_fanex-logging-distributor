using EasyNetQ;
using Fanex.Logging.Distributor.Configuration;
using Fanex.Logging.Distributor.Helpers;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Fanex.Logging.Distributor.Services;



public class ConsumeRabbitMQHostedService : BackgroundService
{
    private const string RoutingKey = "RabbitMQTraceListener.LogEntryQueueRouteKey";
    private readonly ILogger logger;
    private readonly ILogService logService;

    private readonly IBus _rabbitMQBus;

    public ConsumeRabbitMQHostedService(RabbitMQOptions options, ILogger logger, ILogService logService)
    {
        this.logger = logger;
        this.logService = logService;
        _rabbitMQBus = RabbitHutch.CreateBus(EasyNetQConnectionStringCryptography.DecryptConnectionString(options.ConnectionString));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _rabbitMQBus.Subscribe<LogEntry>(RoutingKey, async (logEntry) =>
            {
                Console.WriteLine("Write Log: {0}", logEntry.Message);
                await logService.WriteLog(logEntry);
            }, config => config.WithTopic(RoutingKey));
        }
        catch (Exception ex)
        {
            await logger.ErrorAsync(ex.Message, ex);
        }
    }

    public override void Dispose()
    {
        _rabbitMQBus.Dispose();
        base.Dispose();
    }
}