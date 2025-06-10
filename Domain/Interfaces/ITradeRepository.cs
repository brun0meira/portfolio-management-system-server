using Domain.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ITradeRepository
    {
        Task<IEnumerable<TradeDto>> GetTradesByUserIdAsync(Guid userId);
        Task<Position> GetPositionByUserAndAssetAsync(Guid userId, Guid assetId);
        Task<Quote> GetLatestQuoteByAssetAsync(Guid assetId);
        Task<IEnumerable<Trade>> GetBuyTradesByAssetAsync(Guid userId, Guid assetId);
        Task AddTradeAsync(Trade trade);
    }
}
