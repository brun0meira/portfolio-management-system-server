using Domain.Dto;
using Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Queue.Services
{
    public class DividendConsumerService : KafkaConsumerService<KafkaDividendMessageDto>
    {
        public DividendConsumerService(DividendConsumerConfig config, IServiceScopeFactory scopeFactory)
            : base(config, scopeFactory) { }

        protected override async Task ProcessMessageAsync(KafkaDividendMessageDto dividend)
        {
            Console.WriteLine("RECEBI MENSAGEM");
            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IDividendRepository>();

            for (int attempt = 0; attempt < 5; attempt++)
            {
                try
                {
                    if (await repo.DividendExistsAsync(dividend.AssetId, dividend.ExDate))
                    {
                        Console.WriteLine("Dividendo duplicado.");
                        return;
                    }

                    var dto = new CreateDividendDto
                    {
                        AssetId = dividend.AssetId,
                        DividendType = dividend.DividendType,
                        ValuePerShare = dividend.ValuePerShare,
                        ExDate = dividend.ExDate,
                        PaymentDate = dividend.PaymentDate
                    };

                    await repo.RegisterDividendAsync(dividend.UserId, dto);
                    Console.WriteLine("Dividendo salvo.");
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