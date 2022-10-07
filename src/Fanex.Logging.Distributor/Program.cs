using Fanex.Logging;
using Fanex.Logging.Distributor;
using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.AspNetCore;
using Fanex.Logging.Sentry;
using Microsoft.AspNetCore.Hosting;
using Sentry;

var configuration = GetConfiguration();
InitFanexLogger(configuration);
var appName = typeof(Startup).Assembly.GetName().Name;

try
{
    Logger.Log.Info($"Starting {appName}");

    await Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration(x => x.AddConfiguration(configuration))
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
            webBuilder.UseUrls("http://0.0.0.0:5000");
        })
        .Build()
        .RunAsync();
}
catch (Exception ex)
{
    Logger.Log.Error($"Program terminated unexpectedly {appName}", ex);
}

static void InitFanexLogger(IConfiguration configuration)
{
    LogManager
        .SetDefaultLogCategory(configuration["Fanex.Logging:DefaultCategory"])
        .Use(new SentryLogging(new SentryEngineOptions
        {
            Dsn = new Dsn(configuration["Fanex.Logging:SentryUrl"])
        }));

    ExceptionHandler
        .UseConfig(new LoggingConfiguration().GetHttpHeaderInfo());
}

static IConfiguration GetConfiguration()
{
    var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();

    return builder.Build();
}