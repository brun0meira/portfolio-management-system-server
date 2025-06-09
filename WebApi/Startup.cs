using Domain.Business;
using Domain.Interfaces;
using Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;

public class Startup
{
    public IConfiguration Configuration { get; }
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        Console.WriteLine("Startup constructor called. Configuration loaded.", Configuration.GetConnectionString("DefaultConnection"));
    }
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "Corretagem API",
                Version = "v1"
            });
        });

        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(
                Configuration.GetConnectionString("DefaultConnection"),
                ServerVersion.AutoDetect(Configuration.GetConnectionString("DefaultConnection"))
            ));

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPositionRepository, PositionRepository>();
        services.AddScoped<ITradeRepository, TradeRepository>();
        services.AddScoped<IDividendRepository, DividendRepository>();
        services.AddScoped<IAssetRepository, AssetRepository>();
        services.AddScoped<IQuoteRepository, QuoteRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<ITradeBusiness, TradeBusiness>();
        services.AddScoped<IAssetBusiness, AssetBusiness>();
        services.AddScoped<IReportBusiness, ReportBusiness>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Corretagem API v1");
                c.RoutePrefix = "swagger";
            });
        }

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}