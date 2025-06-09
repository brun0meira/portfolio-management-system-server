using Domain.Dto;

namespace Domain.Interfaces
{
    public interface IReportBusiness
    {
        Task<List<TopClientDto>> GetTopClientsByPositionAsync(int limit);
    }
}
