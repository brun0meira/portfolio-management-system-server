using Infrastructure.Queue.Services;

public class DividendWorker : BackgroundService
{
    private readonly DividendConsumerService _consumerService;

    public DividendWorker(DividendConsumerService consumerService)
    {
        _consumerService = consumerService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _consumerService.StartAsync(stoppingToken);
    }
}