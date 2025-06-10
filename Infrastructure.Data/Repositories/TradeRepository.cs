using Domain.Dto;
using Domain.Enum;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Repositories
{
    public class TradeRepository : ITradeRepository
    {
        private readonly AppDbContext _context;

        public TradeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TradeDto>> GetTradesByUserIdAsync(Guid userId)
        {
            var trades = await _context.Trades
                .Include(t => t.Asset)
                .Where(t => t.UserId == userId)
                .ToListAsync();

            return trades.Select(t => new TradeDto
            {
                Id = t.Id,
                TradeTime = t.TradeTime,
                AssetCode = t.Asset?.Code,
                AssetName = t.Asset?.Name,
                TradeType = t.TradeType, // ou mapear para "BUY"/"SELL"
                Quantity = t.Quantity,
                UnitPrice = t.UnitPrice
            });
        }

        public async Task<Position> GetPositionByUserAndAssetAsync(Guid userId, Guid assetId)
        {
            return await _context.Positions
                .FirstOrDefaultAsync(p => p.UserId == userId && p.AssetId == assetId);
        }

        public async Task<Quote> GetLatestQuoteByAssetAsync(Guid assetId)
        {
            return await _context.Quotes
                .Where(q => q.AssetId == assetId)
                .OrderByDescending(q => q.QuoteTime)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Trade>> GetBuyTradesByAssetAsync(Guid userId, Guid assetId)
        {
            return await _context.Trades
                .Where(t => t.UserId == userId && t.AssetId == assetId && t.TradeType == Domain.Enum.TradeType.Buy)
                .ToListAsync();
        }
        public async Task AddTradeAsync(Trade trade)
        {
            await Task.Run(() => _context.Trades.Add(trade));

            await _context.SaveChangesAsync();
        }
    }
}

