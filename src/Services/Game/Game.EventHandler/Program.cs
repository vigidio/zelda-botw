namespace Game.EventHandler
{
    using KafkaFlow;
    using KafkaFlow.Serializer;
    using KafkaFlow.Serializer.Json;
    using KafkaFlow.TypedHandler;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

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
                    const string consumerName = "game-event-handler";

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
                                                    .AddHandler<NewGameHandler>())))));
            
                    var provider = services.BuildServiceProvider();

                    var bus = provider.CreateKafkaBus();
                    
                    bus.StartAsync().GetAwaiter().GetResult();

                    services.AddHostedService<Worker>();
                });
    }
}