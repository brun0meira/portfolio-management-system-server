using Domain.Dto;
using Domain.Enum;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    public UserRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<UserBrokerageTotalDto> GetTotalBrokerageAsync(Guid userId)
    {
        var user = await _context.Users
            .Where(u => u.Id == userId)
            .Select(u => new UserBrokerageTotalDto
            {
                UserId = u.Id,
                UserName = u.Name,
                TotalBrokeragePaid = u.Trades.Sum(t => t.Fee)
            })
            .FirstOrDefaultAsync();

        return user;
    }

    public async Task<List<UserProvisionDto>> GetUserProvisionsAsync(Guid userId)
    {
        var provisions = await _context.Positions
            .Where(p => p.UserId == userId && p.Quantity > 0)
            .SelectMany(p => p.Asset.Dividends.Select(d => new UserProvisionDto
            {
                AssetCode = p.Asset.Code,
                AssetName = p.Asset.Name,
                Quantity = p.Quantity,
                DividendType = d.DividendType.ToString(),
                ValuePerShare = d.ValuePerShare,
                ExDate = d.ExDate,
                PaymentDate = d.PaymentDate
            }))
            .Where(dto => dto.ExDate <= DateTime.UtcNow) // Apenas dividendos já anunciados
            .ToListAsync();

        return provisions;
    }

    public async Task<List<PnLHistoryDto>> GetUserPnLHistoryAsync(Guid userId)
    {
        var tradePnL = await _context.Trades
            .Where(t => t.UserId == userId)
            .GroupBy(t => t.TradeTime.Date)
            .Select(g => new PnLHistoryDto
            {
                Date = g.Key,
                PnL = g.Sum(t =>
                    t.TradeType == TradeType.Buy ? -t.Quantity * t.UnitPrice - t.Fee :
                    t.Quantity * t.UnitPrice - t.Fee)
            })
            .ToListAsync();

        var dividends = await _context.Dividends
            .Select(d => new
            {
                d.AssetId,
                d.ValuePerShare,
                d.PaymentDate
            })
            .ToListAsync();

        var dividendPnL = new Dictionary<DateTime, decimal>();

        foreach (var dividend in dividends)
        {
            var quantityHeld = await _context.Trades
                .Where(t => t.UserId == userId && t.AssetId == dividend.AssetId && t.TradeTime.Date <= dividend.PaymentDate.Date)
                .GroupBy(t => 1)
                .Select(g => g.Sum(t => t.TradeType == TradeType.Buy ? t.Quantity : -t.Quantity))
                .FirstOrDefaultAsync();

            var totalDividend = quantityHeld * dividend.ValuePerShare;

            if (dividendPnL.ContainsKey(dividend.PaymentDate.Date))
                dividendPnL[dividend.PaymentDate.Date] += totalDividend;
            else
                dividendPnL[dividend.PaymentDate.Date] = totalDividend;
        }

        var merged = tradePnL.Concat(dividendPnL.Select(d => new PnLHistoryDto { Date = d.Key, PnL = d.Value }))
            .GroupBy(x => x.Date)
            .Select(g => new PnLHistoryDto
            {
                Date = g.Key,
                PnL = g.Sum(x => x.PnL)
            })
            .OrderBy(x => x.Date)
            .ToList();

        return merged;
    }

    public async Task AddUserAsync(CreateUserDto userDto)
    {
        var user = new User
        {
            Name = userDto.Name,
            Email = userDto.Email,
            FeePercentage = userDto.FeePercentage
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }
    public async Task<User> GetUserByIdAsync(Guid userId)
    {
        return await _context.Users.FindAsync(userId);
    }
}
