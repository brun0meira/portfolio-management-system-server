using Domain.Dto;
using Domain.Entities;
using Domain.Enum;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories
{
    public class DividendRepository : IDividendRepository
    {
        private readonly AppDbContext _context;

        public DividendRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> RegisterDividendAsync(Guid userId, CreateDividendDto dto)
        {
            var user = await _context.Users.FindAsync(userId);
            var asset = await _context.Assets.FindAsync(dto.AssetId);

            if (user == null || asset == null)
                return false;

            var trades = await _context.Trades
                .Where(t => t.UserId == userId && t.AssetId == dto.AssetId && t.TradeTime <= dto.ExDate)
                .ToListAsync();

            if (!trades.Any())
                return false;

            int quantityAtExDate = trades.Sum(t => t.TradeType == TradeType.Buy ? t.Quantity : -t.Quantity);

            if (quantityAtExDate <= 0)
                return false;

            var dividend = new Dividend
            {
                AssetId = dto.AssetId,
                DividendType = dto.DividendType,
                ValuePerShare = dto.ValuePerShare,
                ExDate = dto.ExDate,
                PaymentDate = dto.PaymentDate,
            };

            _context.Dividends.Add(dividend);
            await _context.SaveChangesAsync();

            var position = await _context.Positions
                .FirstOrDefaultAsync(p => p.UserId == userId && p.AssetId == dto.AssetId);

            if (position != null)
            {
                var totalDividend = quantityAtExDate * dto.ValuePerShare;
                position.PnL += totalDividend;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DividendExistsAsync(Guid assetId, DateTime exDate)
        {
            return await _context.Dividends.AnyAsync(q => q.AssetId == assetId && q.ExDate == exDate);
        }
    }
}
