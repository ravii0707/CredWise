using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CredWiseAdmin.Data;
using CredWiseAdmin.Core.Entities;
using Microsoft.Extensions.Logging;
// using CredWiseAdmin.Models;

namespace CredWiseAdmin.Repository
{
    public class LoanEnquiryRepository : ILoanEnquiryRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<LoanEnquiryRepository> _logger;

        public LoanEnquiryRepository(
            AppDbContext context,
            ILogger<LoanEnquiryRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<LoanEnquiry>> GetAllEnquiriesAsync()
        {
            try
            {
                var result = await _context.LoanEnquiries.AsNoTracking().OrderByDescending(e => e.CreatedAt).ToListAsync();
                _logger.LogInformation("Fetched {Count} enquiries from DB", result.Count);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all enquiries");
                throw;
            }
        }

        public async Task<LoanEnquiry> GetEnquiryByIdAsync(int id)
        {
            try
            {
                return await _context.LoanEnquiries.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving enquiry with ID: {Id}", id);
                throw;
            }
        }

        public async Task<bool> ToggleEnquiryStatusAsync(int id)
        {
            try
            {
                var enquiry = await _context.LoanEnquiries.FindAsync(id);
                if (enquiry == null)
                    return false;

                // If you want to implement toggling, you need a different property, or skip this logic.

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while toggling status for enquiry with ID: {Id}", id);
                throw;
            }
        }
    }
}
