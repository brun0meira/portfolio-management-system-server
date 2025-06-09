using Domain.Dto;

namespace Domain.Interfaces
{
    public interface IPositionRepository
    {
        Task<IEnumerable<PositionDto>> GetPositionsByUserIdAsync(Guid userId);
        Task<PositionDto> GetUserAssetPositionAsync(Guid userId, string assetCode);
        Task AddPositionAsync(Position position);
    }
}
