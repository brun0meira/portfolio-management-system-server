using Domain.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IQuoteRepository
    {
        Task<QuoteDto> GetLatestQuoteByAssetSymbolAsync(string assetSymbol);
        Task<bool> QuoteExistsAsync(Guid assetId, DateTime quoteTime);
        Task AddQuoteAsync(Quote quote);
    }
}
