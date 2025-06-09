using Domain.Dto;

namespace Domain.Interfaces
{
    public interface ITradeBusiness
    {
        Task<SimulateOperationResponse> SimulateOperationAsync(Guid userId, SimulateOperationRequest request);
        Task<bool> CreateTradeAsync(Guid userId, CreateTradeDto dto);
    }
}
