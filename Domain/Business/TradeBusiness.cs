using Domain.Dto;
using Domain.Enum;
using Domain.Interfaces;
using Polly;
using Microsoft.Extensions.Logging;

namespace Domain.Business
{
    public class TradeBusiness : ITradeBusiness
    {
        private readonly ITradeRepository _tradeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAssetRepository _assetRepository;
        private readonly IPositionRepository _positionRepository;
        private readonly ILogger<TradeBusiness> _logger;

        public TradeBusiness(ITradeRepository tradeRepository, IUserRepository userRepository, IAssetRepository assetRepository, IPositionRepository positionRepository, ILogger<TradeBusiness> logger)
        {
            _tradeRepository = tradeRepository;
            _userRepository = userRepository;
            _assetRepository = assetRepository;
            _positionRepository = positionRepository;
            _logger = logger;
        }

        public async Task<SimulateOperationResponse> SimulateOperationAsync(Guid userId, SimulateOperationRequest request)
        {
            var position = await _tradeRepository.GetPositionByUserAndAssetAsync(userId, request.AssetId);

            int currentQuantity = position?.Quantity ?? 0;
            decimal currentAvgPrice = position?.AvgPrice ?? 0m;

            int newQuantity = currentQuantity;
            decimal newAvgPrice = currentAvgPrice;

            if (request.TradeType == TradeType.Buy)
            {
                decimal totalCost = currentAvgPrice * currentQuantity + request.UnitPrice * request.Quantity + request.Fee;
                newQuantity += request.Quantity;
                newAvgPrice = newQuantity > 0 ? totalCost / newQuantity : 0m;
            }
            else if (request.TradeType == TradeType.Sell)
            {
                newQuantity -= request.Quantity;
            }

            var policy = Policy<Quote>
                .Handle<Exception>()
                .CircuitBreakerAsync(handledEventsAllowedBeforeBreaking: 3, durationOfBreak: TimeSpan.FromSeconds(30));

            var fallbackPolicy = Policy<Quote>
                .Handle<Exception>()
                .FallbackAsync(
                    fallbackValue: new Quote { UnitPrice = 100 },
                    onFallbackAsync: async (exception, context) =>
                    {
                        _logger.LogWarning(exception.Exception, "Fallback chamado: falha ao buscar a última cotação.");
                        await Task.CompletedTask;
                    });

            var combinedPolicy = fallbackPolicy.WrapAsync(policy);

            var lastQuote = await combinedPolicy.ExecuteAsync(() =>
                _tradeRepository.GetLatestQuoteByAssetAsync(request.AssetId)
            );

            decimal marketPrice = lastQuote?.UnitPrice ?? request.UnitPrice;
            decimal estimatedPnL = (marketPrice - newAvgPrice) * newQuantity;

            return new SimulateOperationResponse
            {
                NewQuantity = newQuantity,
                NewAvgPrice = Math.Round(newAvgPrice, 4),
                EstimatedPnL = Math.Round(estimatedPnL, 2)
            };
        }

        public async Task<bool> CreateTradeAsync(Guid userId, CreateTradeDto dto)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            var asset = await _assetRepository.GetAssetByIdAsync(dto.AssetId);

            if (user == null || asset == null) return false;

            var value = dto.Quantity * dto.UnitPrice;
            var fee = value * (user.FeePercentage / 100);

            var trade = new Trade
            {
                UserId = userId,
                AssetId = dto.AssetId,
                Quantity = dto.Quantity,
                UnitPrice = dto.UnitPrice,
                TradeTime = dto.TradeTime,
                TradeType = dto.TradeType,
                Fee = fee
            };

            var position = await _tradeRepository.GetPositionByUserAndAssetAsync(userId, dto.AssetId);

            if (position == null)
            {
                position = new Position
                {
                    UserId = userId,
                    AssetId = dto.AssetId,
                    Quantity = dto.TradeType == TradeType.Buy ? dto.Quantity : -dto.Quantity,
                    AvgPrice = dto.UnitPrice,
                    PnL = 0m
                };
                await _positionRepository.AddPositionAsync(position);
            }
            else
            {
                if (dto.TradeType == TradeType.Buy)
                {
                    var totalQuantity = position.Quantity + dto.Quantity;
                    if (totalQuantity == 0)
                    {
                        position.Quantity = 0;
                        position.AvgPrice = 0;
                    }
                    else
                    {
                        position.AvgPrice = ((position.Quantity * position.AvgPrice) + (dto.Quantity * dto.UnitPrice)) / totalQuantity;
                        position.Quantity = totalQuantity;
                    }
                }
                else if (dto.TradeType == TradeType.Sell)
                {
                    position.Quantity -= dto.Quantity;
                }
            }

            var policy = Policy<Quote>
                .Handle<Exception>()
                .CircuitBreakerAsync(handledEventsAllowedBeforeBreaking: 3, durationOfBreak: TimeSpan.FromSeconds(30));

            var fallbackPolicy = Policy<Quote>
                .Handle<Exception>()
                .FallbackAsync(
                    fallbackValue: new Quote { UnitPrice = 100 },
                    onFallbackAsync: async (exception, context) =>
                    {
                        _logger.LogWarning(exception.Exception, "Fallback acionado na cotação ao criar trade.");
                        await Task.CompletedTask;
                    });

            var resiliencePolicy = fallbackPolicy.WrapAsync(policy);

            var lastQuote = await resiliencePolicy.ExecuteAsync(() =>
                _tradeRepository.GetLatestQuoteByAssetAsync(dto.AssetId)
            );

            if (lastQuote != null)
            {
                position.PnL = (lastQuote.UnitPrice - position.AvgPrice) * position.Quantity;
                _logger.LogInformation("PnL atualizado com última cotação: {UnitPrice}", lastQuote.UnitPrice);
            }

            await _tradeRepository.AddTradeAsync(trade);

            return true;
        }
    }
}
