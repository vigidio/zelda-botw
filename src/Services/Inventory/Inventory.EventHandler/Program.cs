namespace Inventory.EventHandler
{
    using DataContracts;
    using Inventory.Domain.CommandHandlers;
    using Inventory.Domain.DomainEvents;
    using Inventory.Domain.Repositories;
    using Inventory.Domain.UseCases.NewGame;
    using Inventory.EventDispatcher;
    using Inventory.Infra.Configurations;
    using Inventory.Infra.Repositories;
    using KafkaFlow;
    using KafkaFlow.Serializer;
    using KafkaFlow.Serializer.Json;
    using KafkaFlow.TypedHandler;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<RepositoryConfiguration>(hostContext.Configuration.GetSection("RepositoryConfiguration"));
                    services.AddTransient(_ => _.GetRequiredService<IOptions<RepositoryConfiguration>>().Value);
                    services.AddSingleton<IDispatcherEvent, DispatcherEvent>();
                    services.AddSingleton<IInventoryRepository, InventoryRepository>();
                    services.AddSingleton<ICommandHandler<NewGameCommand>, NewGameCommandHandler>();
                    services.AddSingleton<IMessageHandler<NewGameStarted>, NewGameEventHandler>();
                    
                    const string consumerName = "inventory-event-handler";

                    const string gameDefaultTopic = "game.zelda.botw.outbox";

                    services
                        .AddKafka(kafka => kafka
                            .AddCluster(cluster => cluster
                                .WithBrokers(new[] {"localhost:9092"})
                                .AddConsumer(consumer => consumer
                                    .Topic(gameDefaultTopic)
                                    .WithGroupId(consumerName)
                                    .WithName(consumerName)
                                    .WithBufferSize(100)
                                    .WithWorkersCount(5)
                                    .WithAutoOffsetReset(AutoOffsetReset.Latest)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddSerializer<JsonMessageSerializer>()
                                            .AddTypedHandlers(
                                                handlers => handlers
                                                    .WithHandlerLifetime(InstanceLifetime.Singleton)
                                                    .AddHandler<NewGameEventHandler>())))));
            
                    var provider = services.BuildServiceProvider();

                    var bus = provider.CreateKafkaBus();
                    
                    bus.StartAsync().GetAwaiter().GetResult();
                    
                    services.AddHostedService<Worker>();
                });
    }
}