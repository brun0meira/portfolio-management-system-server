using Domain.Dto;

namespace Domain.Interfaces
{
    public interface IAssetBusiness
    {
        Task<List<QuoteHistoryDto>> GetQuoteHistoryAsync(string assetSymbol, string period);
        Task<decimal> CalculateVolatilityAsync(string assetSymbol, string period = "30d");
        Task<decimal> CalculateWeightedAveragePriceAsync(Guid userId, Guid assetId);
    }
}
