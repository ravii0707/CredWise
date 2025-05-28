using CredWiseAdmin.Core.Entities;
using CredWiseAdmin.Core.Exceptions;
using CredWiseAdmin.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CredWiseAdmin.Repository.Implementation
{
    public class LoanRepaymentRepository : ILoanRepaymentRepository
    {
        private readonly AppDbContext _context;

        public LoanRepaymentRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<LoanRepaymentSchedule>> GetByLoanApplicationIdAsync(int loanApplicationId)
        {
            try
            {
                return await _context.LoanRepaymentSchedules
                    .Where(r => r.LoanApplicationId == loanApplicationId)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error fetching loan repayment schedules by LoanApplicationId.", ex);
            }
        }

        public async Task<LoanRepaymentSchedule> GetByIdAsync(int id)
        {
            try
            {
                var repayment = await _context.LoanRepaymentSchedules
                    .Include(r => r.LoanApplication)
                    .FirstOrDefaultAsync(r => r.RepaymentId == id);

                if (repayment == null)
                    throw new NotFoundException($"Loan repayment with ID {id} was not found.");

                return repayment;
            }
            catch (CustomException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error fetching loan repayment schedule by ID.", ex);
            }
        }

        public async Task AddAsync(LoanRepaymentSchedule repayment)
        {
            if (repayment == null)
                throw new BadRequestException("Repayment schedule cannot be null.");

            try
            {
                await _context.LoanRepaymentSchedules.AddAsync(repayment);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error adding loan repayment schedule.", ex);
            }
        }

        public async Task UpdateAsync(LoanRepaymentSchedule repayment)
        {
            if (repayment == null)
                throw new BadRequestException("Repayment schedule cannot be null.");

            try
            {
                _context.LoanRepaymentSchedules.Update(repayment);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error updating loan repayment schedule.", ex);
            }
        }

        public async Task<IEnumerable<LoanRepaymentSchedule>> GetPendingRepaymentsAsync(int userId)
        {
            try
            {
                return await _context.LoanRepaymentSchedules
                    .Where(r => r.LoanApplication.UserId == userId &&
                                r.Status == "Pending")
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error fetching pending loan repayments.", ex);
            }
        }

        public async Task<IEnumerable<LoanRepaymentSchedule>> GetOverdueRepaymentsAsync()
        {
            try
            {
                var today = DateOnly.FromDateTime(DateTime.UtcNow);

                return await _context.LoanRepaymentSchedules
                    .Where(r => r.Status == "Pending" &&
                                r.DueDate < today)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error fetching overdue loan repayments.", ex);
            }
        }

        public async Task<IEnumerable<LoanRepaymentSchedule>> GetAllRepaymentsAsync()
        {
            try
            {
                return await _context.LoanRepaymentSchedules
                    .Include(r => r.LoanApplication)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Unable to fetch loan repayments. Please try again later.", ex);
            }
        }
    }
}