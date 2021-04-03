namespace Sandbox.ConsoleApp
{
    using System;
    using System.Threading.Tasks;
    using DataContracts;
    using KafkaFlow;
    using KafkaFlow.Producers;
    using KafkaFlow.Serializer;
    using KafkaFlow.Serializer.Json;
    using Microsoft.Extensions.DependencyInjection;

    static class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();

            const string producerName = "sandbox-console-app";

            const string gameDefaultTopic = "game.zelda.botw.outbox";

            services
                .AddKafka(kafka => kafka
                    .AddCluster(cluster => cluster
                        .WithBrokers(new[] {"localhost:9092"})
                        .AddProducer(producerName,
                            producer => producer
                                .DefaultTopic(gameDefaultTopic)
                                .AddMiddlewares(middlewares => middlewares
                                    .AddSerializer<JsonMessageSerializer>())
                                .WithAcks(Acks.All))));
            
            var provider = services.BuildServiceProvider();

            var bus = provider.CreateKafkaBus();

            await bus.StartAsync();
            
            var producers = provider.GetRequiredService<IProducerAccessor>();

            while (true)
            {
                Console.Write("Message to produce, NewGame, AddItem, SaveGame:");
                var input = Console.ReadLine()?.ToLower();
                switch (input)
                {
                    case "newgame":
                        var newGame = new NewGameStarted {UserId = Guid.NewGuid()};
                        await producers[producerName]
                            .ProduceAsync(newGame.UserId.ToString(), newGame);
                        break;
                }
            }
        }
    }
}