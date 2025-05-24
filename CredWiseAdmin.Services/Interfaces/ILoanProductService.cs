using CredWiseAdmin.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CredWiseAdmin.Services.Interfaces
{
    public interface ILoanProductService
    {
        Task<IEnumerable<LoanProductResponseDto>> GetAllLoanProductsAsync();
        Task<LoanProductResponseDto> GetLoanProductByIdAsync(int id);
    }
}
