using Domain.Dto;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IDividendRepository
    {
        Task<bool> RegisterDividendAsync(Guid userId, CreateDividendDto dto);
        Task<bool> DividendExistsAsync(Guid assetId, DateTime exDate);
    }
}
