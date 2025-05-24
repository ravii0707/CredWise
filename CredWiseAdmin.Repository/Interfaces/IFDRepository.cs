using CredWiseAdmin.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CredWiseAdmin.Repository.Interfaces
{
    public interface IFDRepository
    {
        Task<IEnumerable<Fdtype>> GetAllFDTypesAsync();
        Task<Fdtype> GetFDTypeByIdAsync(int id);
        Task<IEnumerable<Fdapplication>> GetFDApplicationsByUserIdAsync(int userId);
        Task<Fdapplication> GetFDApplicationByIdAsync(int id);
        Task AddFDApplicationAsync(Fdapplication application);
        Task AddFDTransactionAsync(Fdtransaction transaction);
    }
}
