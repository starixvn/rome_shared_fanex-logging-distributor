namespace Fanex.Logging.Distributor.Configuration;

public class RabbitMQOptions
{
    public const string RabbitMQ = "RabbitMQ";

    public string ConnectionString { get; set; } = string.Empty;
}