using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SampleProject.Infrastructure.HealthMonitoring;
using System.Text;

namespace SampleProject.API.InjectionUsages;
public static class AppUseExtensions
{
    public static IApplicationBuilder AppUse(this IApplicationBuilder app, IConfiguration configuration)
    {
        //app.MigratingDatabase();
        UsingJobs(configuration);
        _ = Task.Run(() => UsingRabbitMQ(configuration));
        return app;
    }

    private async static Task UsingRabbitMQ(IConfiguration configuration)
    {
        var factory = new ConnectionFactory()
        {
            HostName = configuration["RabbitMQSettings:HostName"] ?? "",
            UserName = configuration["RabbitMQSettings:UserName"] ?? "",
            Password = configuration["RabbitMQSettings:Password"] ?? ""
        };

        var connection = await factory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();
        await channel.QueueDeclareAsync(queue: "TestModel_Notifications", durable: false, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"Received Message: {message}");
            return Task.CompletedTask;
        };

        await channel.BasicConsumeAsync(queue: "TestModel_Notifications", autoAck: true, consumer: consumer);
    }

    private static void UsingJobs(IConfiguration configuration)
    {
        //RecurringJob.AddOrUpdate<HealthCheckJob>("SampleJob", x => HealthCheckJob.CheckStatus(configuration["HealthCheck:Uri"]!), "* * * * *");
    }

    //private static IApplicationBuilder MigratingDatabase(this IApplicationBuilder app)
    //{
    //    using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
    //    var context = serviceScope.ServiceProvider.GetService<SampleProjectDBContext>();
    //    context?.Database.Migrate();

    //    return app;
    //}
}
