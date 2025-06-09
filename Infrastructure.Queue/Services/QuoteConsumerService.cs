using Domain.Dto;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Queue.Services
{
    public class QuoteConsumerService : KafkaConsumerService<KafkaQuoteMessageDto>
    {
        public QuoteConsumerService(QuoteConsumerConfig config, IServiceScopeFactory scopeFactory)
            : base(config, scopeFactory) { }

        protected override async Task ProcessMessageAsync(KafkaQuoteMessageDto quote)
        {
            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IQuoteRepository>();

            for (int attempt = 0; attempt < 5; attempt++)
            {
                try
                {
                    if (await repo.QuoteExistsAsync(quote.AssetId, quote.QuoteTime))
                    {
                        Console.WriteLine("Cotação duplicada.");
                        return;
                    }

                    var quoteEntity = new Quote
                    {
                        AssetId = quote.AssetId,
                        QuoteTime = quote.QuoteTime,
                        UnitPrice = quote.UnitPrice
                    };

                    await repo.AddQuoteAsync(quoteEntity);
                    Console.WriteLine("Cotação salva.");
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro tentativa {attempt + 1}: {ex.Message}");
                    await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt)));
                }
            }
        }
    }
}