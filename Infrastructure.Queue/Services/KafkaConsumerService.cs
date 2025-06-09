using Confluent.Kafka;
using Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace Infrastructure.Queue.Services
{
    public abstract class KafkaConsumerService<TMessage> : IKafkaConsumerService
    {
        private readonly KafkaConsumerConfig _config;
        protected readonly IServiceScopeFactory _scopeFactory;
        private const int MaxRetries = 5;

        protected KafkaConsumerService(KafkaConsumerConfig config, IServiceScopeFactory scopeFactory)
        {
            _config = config;
            _scopeFactory = scopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = _config.BootstrapServers,
                GroupId = _config.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
            consumer.Subscribe(_config.Topic);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(cancellationToken);
                    if (TryDeserialize(result.Message.Value, out TMessage message))
                    {
                        await ProcessMessageAsync(message);
                    }
                    else
                    {
                        Console.WriteLine("Mensagem inválida.");
                    }
                }
                catch (ConsumeException e)
                {
                    Console.WriteLine($"Erro ao consumir: {e.Error.Reason}");
                }
            }

            consumer.Close();
        }

        private static bool TryDeserialize(string json, out TMessage? result)
        {
            try
            {
                result = JsonSerializer.Deserialize<TMessage>(json);
                return result != null;
            }
            catch
            {
                result = default;
                return false;
            }
        }

        protected abstract Task ProcessMessageAsync(TMessage message);
    }
}
