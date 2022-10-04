using Fanex.Logging.Distributor.Extensions;
using Fanex.Logging.Distributor.Services;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;
        services.AddConfig(configuration);
        services.AddFanexData(configuration);
        services.AddSingleton<ILogService, LogService>();
        services.AddHostedService<ConsumeRabbitMQHostedService>();
    });

var host = builder.Build();
await host.RunAsync();
