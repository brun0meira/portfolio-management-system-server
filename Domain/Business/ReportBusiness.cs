using Domain.Dto;
using Domain.Interfaces;

namespace Domain.Business
{
    public class ReportBusiness : IReportBusiness
    {
        private readonly IReportRepository _reportRepository;

        public ReportBusiness(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<List<TopClientDto>> GetTopClientsByPositionAsync(int limit)
        {
            var result = await _reportRepository.GetTopClientsByPositionAsync(limit);

            return result.Select(x => new TopClientDto
            {
                UserId = x.user.Id,
                Name = x.user.Name,
                Email = x.user.Email,
                TotalPositionValue = x.totalPosition
            }).ToList();
        }
    }
}
