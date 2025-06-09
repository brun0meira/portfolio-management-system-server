using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Data.Repositories;
using Infrastructure.Queue;
using Infrastructure.Queue.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(
                Configuration.GetConnectionString("DefaultConnection"),
                ServerVersion.AutoDetect(Configuration.GetConnectionString("DefaultConnection"))
            ));

        // Repositories
        services.AddScoped<IQuoteRepository, QuoteRepository>();
        services.AddScoped<IDividendRepository, DividendRepository>();

        // Configurations
        services.AddSingleton<QuoteConsumerConfig>(sp =>
            new QuoteConsumerConfig
            {
                BootstrapServers = Configuration["Kafka:BootstrapServers"]
            });

        services.AddSingleton<DividendConsumerConfig>(sp =>
            new DividendConsumerConfig
            {
                BootstrapServers = Configuration["Kafka:BootstrapServers"]
            });

        // Services
        services.AddSingleton<QuoteConsumerService>();
        services.AddHostedService<QuoteWorker>();

        services.AddSingleton<DividendConsumerService>();
        services.AddHostedService<DividendWorker>();

    }
}
