using MassTransit;
using NexusFit.EventBus.Worker;
using NexusFit.EventBus.Worker.Helpers;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.Configure<EventsDatabaseSettings>(
            context.Configuration.GetSection("EventsDatabaseSettings"));

        services.AddMassTransit(config =>
        {
            config.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(context.Configuration["EventBusSettings:HostAddress"]);
            });
        });

        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();
