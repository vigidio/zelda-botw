namespace Inventory.IntegrationTests.Infra
{
    using Inventory.Domain.Models.AggregateRoot;
    using Inventory.Domain.Repositories;
    using Inventory.Infra.Configurations;
    using Inventory.Infra.Models;
    using Inventory.Infra.Repositories;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using MongoDB.Bson.Serialization;
    using MongoDB.Bson.Serialization.Serializers;
    using MongoDB.Driver;

    public class TestStartup
    {
        public TestStartup(IHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true);

            this.Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // TODO: Migrate this code for a further definitive Startup.cs
            var repositoryConfiguration = this.Configuration.GetSection("Repositories").Get<RepositoryConfiguration>();

            services.AddSingleton(_ =>
            {
                if (!BsonClassMap.IsClassMapRegistered(typeof(AggregateRoot)))
                {
                    BsonClassMap.RegisterClassMap<EventData>(cm =>
                    {
                        cm.AutoMap();
                        cm.MapIdProperty(c => c.NintendoUserId);
                        cm.MapIdProperty(c => c.Version);
                    });
                
                    BsonSerializer.RegisterSerializer(
                        new ImpliedImplementationInterfaceSerializer<IEventData, EventData>(
                            BsonSerializer.LookupSerializer<EventData>()));
                }
                
                var client = new MongoClient(repositoryConfiguration.Mongo.ConnectionString);
                var database = client.GetDatabase("botw");
                return database.GetCollection<IEventData>("inventory");
            });

            services.AddSingleton<IInventoryRepository>(provider =>
                new InventoryRepository(provider.GetRequiredService<IMongoCollection<IEventData>>()));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
        }
    }
}