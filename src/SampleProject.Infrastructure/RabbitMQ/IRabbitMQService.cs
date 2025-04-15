namespace SampleProject.Infrastructure.RabbitMQ;

public interface IRabbitMQService
{
    Task SendAddTestModelMessage(string name, CancellationToken cancellationToken);
}
