using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.Data.SqlServer;
using Fanex.Logging.Distributor.Configuration;

namespace Fanex.Logging.Distributor.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConfig(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMQOptions = new RabbitMQOptions();
        configuration.GetSection(RabbitMQOptions.RabbitMQ).Bind(rabbitMQOptions);
        services.AddSingleton(rabbitMQOptions);

        return services;
    }

    public static IServiceCollection AddFanexData(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionStrings = configuration
            .GetSection("ConnectionStrings")
            .GetChildren()
            .ToDictionary(
                connection => connection.Key,
                connection => new ConnectionConfiguration(connection.Key, connection.Value), StringComparer.OrdinalIgnoreCase);

        DbSettingProviderManager
            .StartNewSession()
            .WithSqlServer("App_Data/SpNames.xml")
            .Use(connectionStrings)
            .Run();

        return services.AddSingleton<IDynamicRepository, DynamicRepository>();
    }
}