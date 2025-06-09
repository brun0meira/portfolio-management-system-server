using Domain.Dto;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories
{
    public class PositionRepository : IPositionRepository
    {
        private readonly AppDbContext _context;

        public PositionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PositionDto>> GetPositionsByUserIdAsync(Guid userId)
        {
            return await _context.Positions
                .Include(p => p.Asset)
                .Where(p => p.UserId == userId)
                .Select(p => new PositionDto
                {
                    AssetCode = p.Asset.Code,
                    AssetName = p.Asset.Name,
                    Quantity = p.Quantity,
                    AvgPrice = p.AvgPrice,
                    PnL = p.PnL
                })
                .ToListAsync();
        }

        public async Task<PositionDto> GetUserAssetPositionAsync(Guid userId, string assetCode)
        {
            var position = await _context.Positions
                .Include(p => p.Asset)
                .Where(p => p.UserId == userId && p.Asset.Code == assetCode)
                .Select(p => new PositionDto
                {
                    AssetCode = p.Asset.Code,
                    AssetName = p.Asset.Name,
                    Quantity = p.Quantity,
                    AvgPrice = p.AvgPrice,
                    PnL = p.PnL
                })
                .FirstOrDefaultAsync();

            return position;
        }

        public async Task AddPositionAsync(Position position)
        {
            await Task.Run(() => _context.Positions.Add(position));
            await _context.SaveChangesAsync();
        }
    }
}
