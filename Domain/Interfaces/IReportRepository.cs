using Domain.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IReportRepository
    {
        Task<List<(User user, decimal totalPosition)>> GetTopClientsByPositionAsync(int limit);
        Task<List<TopClientByBrokerageDto>> GetTopClientsByBrokerageAsync(int limit);
        Task<List<UserMonthlySummaryDto>> GetMonthlySummaryAsync(int year, int month);
    }
}
