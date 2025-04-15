using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;

namespace SampleProject.Infrastructure.RabbitMQ;

public class RabbitMQService : IRabbitMQService
{
    private readonly RabbitMQSettings settings;

    public RabbitMQService(IOptions<RabbitMQSettings> options)
    {
        settings = options.Value;
    }

    public async Task SendAddTestModelMessage(string name, CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory()
        {
            HostName = settings.HostName,
            UserName = settings.UserName,
            Password = settings.Password
        };

        // Create connection and channel synchronously
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync(); // Synchronous CreateChannel method

        // Declare the queue
        await channel.QueueDeclareAsync(queue: "TestModel_Notifications", durable: false, exclusive: false, autoDelete: false, arguments: null);

        string message = $"New entity added: {name}";
        var body = Encoding.UTF8.GetBytes(message);

        await channel.BasicPublishAsync(
            exchange: "",
            routingKey: "TestModel_Notifications",
            mandatory: true,
            body: body,
            cancellationToken: cancellationToken
        );

        Console.WriteLine($"Add TestModel with name {name} informed...");
    }
}
