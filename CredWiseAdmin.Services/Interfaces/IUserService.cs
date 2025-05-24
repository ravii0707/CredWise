using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CredWiseAdmin.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDto> RegisterUserAsync(RegisterUserDto registerDto);
        Task<UserResponseDto> GetUserByIdAsync(int id);
        Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
        Task<bool> DeactivateUserAsync(int id);
        Task<bool> UpdateUserAsync(int id, UpdateUserDto updateDto);
        Task<bool> AdminExists();
        Task<int> CountAdmins();
        Task<bool> ValidateUserCredentialsAsync(string email, string password);
    }
}
