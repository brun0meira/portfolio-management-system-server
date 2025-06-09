using Domain.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IAssetRepository
    {
        Task<IEnumerable<AssetDto>> GetAllAssetsAsync();
        Task<Asset> GetBySymbolAsync(string assetSymbol);
        Task<List<Quote>> GetQuotesHistoryAsync(Guid assetId, DateTime fromDate);
        Task AddAssetAsync(CreateAssetDto assetDto);
        Task<Asset> GetAssetByIdAsync(Guid assetId);
    }
}