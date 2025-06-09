using Domain.Dto;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories
{
    public class QuoteRepository : IQuoteRepository
    {
        private readonly AppDbContext _context;

        public QuoteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<QuoteDto> GetLatestQuoteByAssetSymbolAsync(string assetSymbol)
        {
            return await _context.Quotes
                .Where(q => q.Asset.Code == assetSymbol)
                .OrderByDescending(q => q.QuoteTime)
                .Select(q => new QuoteDto
                {
                    AssetCode = q.Asset.Code,
                    UnitPrice = q.UnitPrice,
                    QuoteTime = q.QuoteTime
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> QuoteExistsAsync(Guid assetId, DateTime quoteTime)
        {
            return await _context.Quotes.AnyAsync(q => q.AssetId == assetId && q.QuoteTime == quoteTime);
        }

        public async Task AddQuoteAsync(Quote quote)
        {
            _context.Quotes.Add(quote);
            await _context.SaveChangesAsync();

            // Recalcular PnL de todas as posições desse ativo
            var positions = await _context.Positions
                .Where(p => p.AssetId == quote.AssetId)
                .ToListAsync();

            foreach (var position in positions)
            {
                position.PnL = (quote.UnitPrice - position.AvgPrice) * position.Quantity;
            }

            await _context.SaveChangesAsync();
        }
    }

}
