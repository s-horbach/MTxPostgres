using MassTransit;
using Messages;
using Microsoft.Extensions.Logging;

namespace ConsoleApp2.Consumer;

public class MyMessageConsumer : IConsumer<MyMessage>
{
    private readonly ILogger<MyMessageConsumer> _logger;

    public MyMessageConsumer(ILogger<MyMessageConsumer> logger) =>
        _logger = logger;

    public Task Consume(ConsumeContext<MyMessage> context)
    {
        var receivedAt = DateTimeOffset.UtcNow;
        var delivery = (receivedAt - context.Message.CreatedAt).TotalMilliseconds;

        _logger.LogInformation("Delivery of {id} took {delivery}ms (sent at: {sent}, received at: {received})",
            context.Message.Id,
            delivery,
            context.Message.CreatedAt.ToString("HH:mm:ss.fff"),
            receivedAt.ToString("HH:mm:ss.fff"));

        return Task.CompletedTask;
    }
}
