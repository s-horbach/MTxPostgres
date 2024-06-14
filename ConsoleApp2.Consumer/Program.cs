using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConsoleApp2.Consumer;

internal class Program
{
    static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder();

        builder.Logging
            .AddConsole()
            .SetMinimumLevel(LogLevel.Information);

        builder.Services.AddOptions<SqlTransportOptions>().Configure(options =>
        {
            options.Host = "localhost";
            options.Database = "my_masstransit";
            options.Schema = "public";
            options.Role = "<role>";
            options.Username = "<username>";
            options.Password = "<password>";
            options.Port = 5432;
        });

        builder.Services.AddMassTransit(config =>
        {
            config.AddConsumer<MyMessageConsumer>();

            config.UsingPostgres((context, busFactoryConfig) =>
            {
                busFactoryConfig.AutoStart = true;
                busFactoryConfig.ConfigureEndpoints(context);
            });
        });

        using var host = builder.Build();
        await host.RunAsync();
    }
}
