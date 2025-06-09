using Domain.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<UserBrokerageTotalDto> GetTotalBrokerageAsync(Guid userId);
        Task<List<UserProvisionDto>> GetUserProvisionsAsync(Guid userId);
        Task<List<PnLHistoryDto>> GetUserPnLHistoryAsync(Guid userId);
        Task AddUserAsync(CreateUserDto userDto);
        Task<User> GetUserByIdAsync(Guid userId);
    }
}