using Domain.Dto;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly AppDbContext _context;

        public ReportRepository(AppDbContext context)
        {
            _context = context;
        }

        private class TopClientResult
        {
            public User User { get; set; }
            public decimal TotalPosition { get; set; }
        }

        public async Task<List<(User user, decimal totalPosition)>> GetTopClientsByPositionAsync(int limit)
        {
            var result = await _context.Positions
                .Include(p => p.User)
                .GroupBy(p => p.User)
                .Select(g => new TopClientResult
                {
                    User = g.Key,
                    TotalPosition = g.Sum(p => p.Quantity * p.AvgPrice)
                })
                .OrderByDescending(x => x.TotalPosition)
                .Take(limit)
                .ToListAsync();

            return result.Select(x => (x.User, x.TotalPosition)).ToList();
        }

        public async Task<List<TopClientByBrokerageDto>> GetTopClientsByBrokerageAsync(int limit)
        {
            return await _context.Trades
                .Include(t => t.User)
                .GroupBy(t => t.User)
                .Select(g => new TopClientByBrokerageDto
                {
                    UserId = g.Key.Id,
                    Name = g.Key.Name,
                    Email = g.Key.Email,
                    TotalBrokerage = g.Sum(t => t.Fee)
                })
                .OrderByDescending(x => x.TotalBrokerage)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<List<UserMonthlySummaryDto>> GetMonthlySummaryAsync(int year, int month)
        {
            var pnlQuery = _context.Positions
                .GroupBy(p => p.User)
                .Select(g => new
                {
                    User = g.Key,
                    TotalPnL = g.Sum(p => p.PnL)
                });

            var brokerageQuery = _context.Trades
                .Where(t => t.TradeTime.Year == year && t.TradeTime.Month == month)
                .GroupBy(t => t.User)
                .Select(g => new
                {
                    User = g.Key,
                    TotalBrokerage = g.Sum(t => t.Fee)
                });

            var dividendQuery = _context.Dividends
                .Where(d => d.PaymentDate.Year == year && d.PaymentDate.Month == month)
                .GroupBy(d => d.Asset.Positions.Select(p => p.User).FirstOrDefault())
                                                                                       
                .Select(g => new
                {
                    User = g.Key,
                    TotalDividends = g.Sum(d => d.ValuePerShare)
                });

            // PnL por usuário (total PnL das posições)
            var pnlByUser = await _context.Positions
                .GroupBy(p => p.User)
                .Select(g => new
                {
                    User = g.Key,
                    TotalPnL = g.Sum(p => p.PnL)
                })
                .ToListAsync();

            // Corretagem por usuário no mês
            var brokerageByUser = await _context.Trades
                .Where(t => t.TradeTime.Year == year && t.TradeTime.Month == month)
                .GroupBy(t => t.User)
                .Select(g => new
                {
                    User = g.Key,
                    TotalBrokerage = g.Sum(t => t.Fee)
                })
                .ToListAsync();

            // Trazer posições de todos os usuários para os ativos com dividendos no mês:
            var dividends = await _context.Dividends
                .Where(d => d.PaymentDate.Year == year && d.PaymentDate.Month == month)
                .ToListAsync();

            var positions = await _context.Positions
                .Include(p => p.User)
                .Include(p => p.Asset)
                .ToListAsync();

            // Mapear dividendos por ativo
            var dividendByAsset = dividends
                .GroupBy(d => d.AssetId)
                .ToDictionary(g => g.Key, g => g.Sum(d => d.ValuePerShare));

            // Somar proventos por usuário conforme quantidade
            var dividendByUser = positions
                .Where(p => dividendByAsset.ContainsKey(p.AssetId))
                .GroupBy(p => p.User)
                .Select(g => new
                {
                    User = g.Key,
                    TotalDividends = g.Sum(p => p.Quantity * dividendByAsset[p.AssetId])
                })
                .ToList();

            // Montar o return
            var users = pnlByUser.Select(p => p.User)
                .Union(brokerageByUser.Select(b => b.User))
                .Union(dividendByUser.Select(d => d.User))
                .Distinct();

            var result = users.Select(user => new UserMonthlySummaryDto
            {
                UserId = user.Id,
                UserName = user.Name,
                UserEmail = user.Email,
                Year = year,
                Month = month,
                TotalPnL = pnlByUser.FirstOrDefault(p => p.User.Id == user.Id)?.TotalPnL ?? 0m,
                TotalBrokerage = brokerageByUser.FirstOrDefault(b => b.User.Id == user.Id)?.TotalBrokerage ?? 0m,
                TotalDividends = dividendByUser.FirstOrDefault(d => d.User.Id == user.Id)?.TotalDividends ?? 0m,
            }).ToList();

            return result;
        }
    }

}
