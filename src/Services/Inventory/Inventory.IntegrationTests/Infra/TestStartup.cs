namespace Inventory.IntegrationTests.Infra
{
    using Inventory.Domain.Repositories;
    using Inventory.Infra.Configurations;
    using Inventory.Infra.Repositories;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

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
            services.AddSingleton<IInventoryRepository>(provider =>
                new InventoryRepository(this.Configuration.GetSection("Repositories").Get<RepositoryConfiguration>()));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
        }
    }
}