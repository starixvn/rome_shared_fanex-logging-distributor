
using Fanex.Logging.Distributor.Extensions;
using Fanex.Logging.Distributor.Services;
using Microsoft.AspNetCore.Builder;

namespace Fanex.Logging.Distributor;

public class Startup
{
    public Startup(IConfiguration configuration)
        => Configuration = configuration;

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddConfig(Configuration)
            .AddFanexData(Configuration)
            .AddSingleton<ILogService, Services.LogService>()
            .AddHostedService<ConsumeRabbitMQHostedService>()
            .AddHealthChecks();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHealthChecks("/health").AllowAnonymous();
        });
    }
}