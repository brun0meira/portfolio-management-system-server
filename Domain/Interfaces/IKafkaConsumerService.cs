namespace Domain.Interfaces
{
    public interface IKafkaConsumerService
    {
        Task StartAsync(CancellationToken cancellationToken);
    }
}
