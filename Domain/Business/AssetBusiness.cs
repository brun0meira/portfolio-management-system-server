using Domain.Dto;
using Domain.Interfaces;

namespace Domain.Business
{
    public class AssetBusiness : IAssetBusiness
    {
        private readonly IAssetRepository _assetRepository;
        private readonly ITradeRepository _tradeRepository;

        public AssetBusiness(IAssetRepository assetRepository, ITradeRepository tradeRepository)
        {
            _assetRepository = assetRepository;
            _tradeRepository = tradeRepository;
        }

        public async Task<List<QuoteHistoryDto>> GetQuoteHistoryAsync(string assetSymbol, string period)
        {
            var asset = await _assetRepository.GetBySymbolAsync(assetSymbol);

            if (asset == null)
                throw new KeyNotFoundException($"Asset with symbol '{assetSymbol}' not found.");

            DateTime fromDate = period.ToLower() switch
            {
                "7d" => DateTime.UtcNow.AddDays(-7),
                "30d" => DateTime.UtcNow.AddDays(-30),
                "1y" => DateTime.UtcNow.AddYears(-1),
                "5y" => DateTime.UtcNow.AddYears(-5),
                _ => throw new ArgumentException("Invalid period. Allowed values: 7d, 30d, 1y, 5y.")
            };

            var quotes = await _assetRepository.GetQuotesHistoryAsync(asset.Id, fromDate);

            return quotes.Select(q => new QuoteHistoryDto
            {
                QuoteTime = q.QuoteTime,
                UnitPrice = q.UnitPrice
            }).ToList();
        }

        public async Task<decimal> CalculateVolatilityAsync(string assetSymbol, string period = "30d")
        {
            var asset = await _assetRepository.GetBySymbolAsync(assetSymbol);
            if (asset == null)
                throw new KeyNotFoundException($"Asset with symbol '{assetSymbol}' not found.");

            DateTime fromDate = period.ToLower() switch
            {
                "7d" => DateTime.UtcNow.AddDays(-7),
                "30d" => DateTime.UtcNow.AddDays(-30),
                "1y" => DateTime.UtcNow.AddYears(-1),
                "5y" => DateTime.UtcNow.AddYears(-5),
                _ => throw new ArgumentException("Invalid period. Allowed values: 7d, 30d, 1y, 5y.")
            };

            var quotes = await _assetRepository.GetQuotesHistoryAsync(asset.Id, fromDate);

            if (quotes.Count < 2)
                throw new InvalidOperationException("Not enough data to calculate volatility.");

            var logReturns = new List<double>();

            for (int i = 1; i < quotes.Count; i++)
            {
                var previous = quotes[i - 1].UnitPrice;
                var current = quotes[i].UnitPrice;

                if (previous <= 0 || current <= 0)
                    continue;

                var logReturn = Math.Log((double)(current / previous));
                logReturns.Add(logReturn);
            }

            if (logReturns.Count == 0)
                throw new InvalidOperationException("Not enough valid data to calculate returns.");

            double avg = logReturns.Average();
            double sumSquares = logReturns.Sum(r => Math.Pow(r - avg, 2));
            double variance = sumSquares / (logReturns.Count - 1);

            double volatility = Math.Sqrt(variance);

            decimal annualizedVolatility = (decimal)(volatility * Math.Sqrt(252));

            return annualizedVolatility;
        }

        public async Task<decimal> CalculateWeightedAveragePriceAsync(Guid userId, Guid assetId)
        {
            var buyTrades = await _tradeRepository.GetBuyTradesByAssetAsync(userId, assetId);

            if (buyTrades == null || !buyTrades.Any())
                throw new InvalidOperationException("Nenhuma operação de compra encontrada para este ativo.");

            decimal totalCost = 0;
            int totalQuantity = 0;

            foreach (var trade in buyTrades)
            {
                if (trade.Quantity <= 0 || trade.UnitPrice <= 0)
                    throw new InvalidOperationException("Operação com dados inválidos encontrada.");

                totalCost += (trade.UnitPrice * trade.Quantity) + trade.Fee;
                totalQuantity += trade.Quantity;
            }

            if (totalQuantity == 0)
                throw new InvalidOperationException("A quantidade total não pode ser zero.");

            return totalCost / totalQuantity;
        }
    }
}
