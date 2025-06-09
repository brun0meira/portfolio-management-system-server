using Infrastructure.Queue.Services;

public class QuoteWorker : BackgroundService
{
    private readonly QuoteConsumerService _consumerService; 

    public QuoteWorker(QuoteConsumerService consumerService)
    {
        _consumerService = consumerService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _consumerService.StartAsync(stoppingToken);
    }
}