using Fanex.Logging.Distributor.Extensions;
using Fanex.Logging.Distributor.Services;
using Microsoft.AspNetCore.Hosting;

await Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;
        services.AddConfig(configuration);
        services.AddFanexData(configuration);
        services.AddSingleton<ILogService, LogService>();
        services.AddHostedService<ConsumeRabbitMQHostedService>();
    })
    .ConfigureWebHostDefaults(webBuilder => webBuilder.UseUrls("http://0.0.0.0:5000"))
    .Build()
    .RunAsync();
