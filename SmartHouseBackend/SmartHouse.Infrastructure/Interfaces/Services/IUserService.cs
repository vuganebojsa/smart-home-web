
using FluentResults;
using SmartHouse.Core.Model;
using SmartHouse.DTOS;
using SmartHouse.Infrastructure.DTOS;

namespace SmartHouse.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<Result<TokenDTO>> Login(string email, string password);
        Task<Result<UserDTO>> Register(RegisterUserDTO newUser);
        Task<Result<UserDTO>> RegisterAdmin(RegisterUserDTO newAdmin);
        Task<Result<string>> ActivateUser(string userEmail);
        Task<Result<PagedList<UserPropertyDTO>>> GetUserProperties(Guid id, Page page);
        Task<Result<PagedList<PropertyDTO>>> GetPendingProperties(Page page);

        string GenerateJwtToken(string email, Guid id, Role role);
        Task<Result<string>> ProccesPendingPropertyRequest(Guid id, ProcessPropertyRequestDTO request);
        Task<Result<UserProfileDTO>> GetUserProfile(Guid userId);

        Task<Result<EditUserDTO>> EditProfile(EditUserDTO newUserInfo, Guid userId);
        Task<Result<bool>> EditPassword(EditPasswordDTO passwordInfo, Guid userId);
        Task<Result<TokenDTO>> EditFirstPassword(EditPasswordDTO passwordInfo, string email);
    }
}
