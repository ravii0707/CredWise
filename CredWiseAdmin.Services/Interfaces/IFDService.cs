using CredWiseAdmin.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CredWiseAdmin.Services.Interfaces
{
    public interface IFDService
    {
        Task<IEnumerable<FDTypeResponseDto>> GetAllFDTypesAsync();
        Task<FDApplicationResponseDto> CreateFDApplicationAsync(FDApplicationDto fdApplicationDto);
        Task<IEnumerable<FDApplicationResponseDto>> GetFDApplicationsByUserIdAsync(int userId);
        Task<FDApplicationResponseDto> GetFDApplicationByIdAsync(int id);
    }
}
