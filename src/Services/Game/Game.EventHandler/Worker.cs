using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Game.EventHandler
{
    using KafkaFlow;
    using KafkaFlow.Serializer;
    using KafkaFlow.Serializer.Json;
    using KafkaFlow.TypedHandler;
    using Microsoft.Extensions.DependencyInjection;

    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;

        private readonly IKafkaBus bus;
        public Worker(ILogger<Worker> logger)
        {
            this.logger = logger;
            
            var services = new ServiceCollection();
            
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

            bus = provider.CreateKafkaBus();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await bus.StartAsync(stoppingToken);
            
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }

            await bus.StopAsync();
        }
    }
}