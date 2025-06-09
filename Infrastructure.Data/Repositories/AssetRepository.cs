using Domain.Dto;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories
{
    public class AssetRepository : IAssetRepository
    {
        private readonly AppDbContext _context;

        public AssetRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AssetDto>> GetAllAssetsAsync()
        {
            return await _context.Assets
                .Select(a => new AssetDto
                {
                    Id = a.Id,
                    Code = a.Code,
                    Name = a.Name
                })
                .ToListAsync();
        }

        public async Task<Asset> GetBySymbolAsync(string assetSymbol)
        {
            return await _context.Assets
                .FirstOrDefaultAsync(a => a.Code.ToUpper() == assetSymbol.ToUpper());
        }

        public async Task<List<Quote>> GetQuotesHistoryAsync(Guid assetId, DateTime fromDate)
        {
            return await _context.Quotes
                .Where(q => q.AssetId == assetId && q.QuoteTime >= fromDate)
                .OrderBy(q => q.QuoteTime)
                .ToListAsync();
        }

        public async Task AddAssetAsync(CreateAssetDto assetDto)
        {
            var asset = new Asset
            {
                Code = assetDto.Code,
                Name = assetDto.Name,
            };

            _context.Assets.Add(asset);
            await _context.SaveChangesAsync();
        }

        public async Task<Asset> GetAssetByIdAsync(Guid assetId)
        {
            return await _context.Assets.FindAsync(assetId);
        }
    }

}
