using CredWiseAdmin.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CredWiseAdmin.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(int id);
        Task<User> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllAsync();
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(int id);
        Task<bool> EmailExists(string email);
        Task<bool> AdminExists();
        Task<int> CountAdmins();
        Task<bool> UserExists(int id);

        //Test
        Task<bool> AnyUserExists();
    }
}