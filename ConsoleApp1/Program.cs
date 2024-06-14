using MassTransit;
using Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConsoleApp1.Publisher;

internal class Program
{
    static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder();

        builder.Logging
            .AddConsole()
            .SetMinimumLevel(LogLevel.Information);

        builder.Services.AddPostgresMigrationHostedService(create: true, delete: false);

        builder.Services.AddOptions<SqlTransportOptions>().Configure(options =>
        {
            options.Host = "localhost";
            options.Database = "my_masstransit";
            options.Schema = "public";
            options.Role = "<role>";
            options.Username = "<username>";
            options.Password = "<password>";
            options.Port = 5432;

            options.AdminUsername = "<adminusername>";
            options.AdminPassword = "<adminpassword>";
        });

        builder.Services.AddMassTransit(config =>
        {
            config.UsingPostgres((context, configurator) => configurator.AutoStart = true);
        });

        using var host = builder.Build();
        var bus = host.Services.GetRequiredService<IBus>();

        await SendMessages(bus);
    }

    static async Task SendMessages(IBus bus)
    {
        Console.WriteLine("Publish [S]ingle or [M]ultiple?");

        int count = 1;
        ConsoleKey key;
        while ((key = Console.ReadKey().Key) is ConsoleKey.S or ConsoleKey.M)
        {
            count = key switch
            {
                ConsoleKey.S => await PublishSingle(bus, count),
                ConsoleKey.M => await PublishMultiple(bus, count),
                _ => throw new Exception("Oops")
            };

            Console.WriteLine("\nPublish [S]ingle or [M]ultiple?");
        }

        Console.WriteLine("\nGood bye!");
    }

    static async Task<int> PublishSingle(IBus bus, int count)
    {
        var createdAt = DateTimeOffset.UtcNow;
        await bus.Publish(new MyMessage(count, createdAt));
        Console.WriteLine($"\nPublished message #{count} at {createdAt:HH:mm:ss.fff}");

        return count + 1;
    }

    static async Task<int> PublishMultiple(IBus bus, int count)
    {
        var random = Random.Shared.Next(1, 10);

        for (int i = 0; i < random; i++)
        {
            count = await PublishSingle(bus, count);
            await Task.Delay(100);
        }

        return count;
    }
}
