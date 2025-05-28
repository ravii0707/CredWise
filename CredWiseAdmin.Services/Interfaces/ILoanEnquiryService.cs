using CredWiseAdmin.Core.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CredWiseAdmin.Services.Interfaces
{
    public interface ILoanEnquiryService
    {
        Task<IEnumerable<LoanEnquiryResponseDto>> GetAllEnquiriesAsync();
        Task<LoanEnquiryResponseDto> GetEnquiryByIdAsync(int id);
    }
} 