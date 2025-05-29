using CredWiseAdmin.Core.Entities;
using CredWiseAdmin.Core.Exceptions;
using CredWiseAdmin.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CredWiseAdmin.Data.Repositories.Implementations
{
    public class LoanApplicationRepository : ILoanApplicationRepository
    {
        private readonly AppDbContext _context;

        public LoanApplicationRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<LoanApplication>> GetAllAsync()
        {
            try
            {
                return await _context.LoanApplications
                    .Include(la => la.User)
                    .Include(la => la.LoanProduct)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Failed to retrieve loan applications", ex);
            }
        }

        public async Task<LoanApplication> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new BadRequestException("Invalid loan application ID");

            try
            {
                var application = await _context.LoanApplications
                    .Include(la => la.User)
                    .Include(la => la.LoanProduct)
                    .Include(la => la.LoanBankStatements)
                    .Include(la => la.LoanRepaymentSchedules)
                    .Include(la => la.PaymentTransactions)
                    .FirstOrDefaultAsync(la => la.LoanApplicationId == id);

                return application ?? throw new NotFoundException($"Loan application with ID {id} not found");
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Failed to retrieve loan application with ID {id}", ex);
            }
        }

        public async Task<IEnumerable<LoanApplication>> GetByUserIdAsync(int userId)
        {
            if (userId <= 0)
                throw new BadRequestException("Invalid user ID");

            try
            {
                return await _context.LoanApplications
                    .Where(la => la.UserId == userId)
                    .Include(la => la.LoanProduct)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Failed to retrieve loan applications for user {userId}", ex);
            }
        }

        public async Task AddAsync(LoanApplication application)
        {
            if (application == null)
                throw new BadRequestException("Loan application cannot be null");

            if (application.UserId <= 0)
                throw new BadRequestException("Invalid user reference");

            try
            {
                await _context.LoanApplications.AddAsync(application);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new RepositoryException("Failed to create loan application - database error", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Failed to create loan application", ex);
            }
        }

        public async Task UpdateAsync(LoanApplication application)
        {
            if (application == null)
                throw new BadRequestException("Loan application cannot be null");

            if (application.LoanApplicationId <= 0)
                throw new BadRequestException("Invalid loan application ID");

            try
            {
                _context.LoanApplications.Update(application);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new RepositoryException("Loan application may have been modified or deleted", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new RepositoryException("Failed to update loan application - database error", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Failed to update loan application", ex);
            }
        }

        public async Task<bool> HasActiveLoan(int userId)
        {
            if (userId <= 0)
                throw new BadRequestException("Invalid user ID");

            try
            {
                return await _context.LoanApplications
                    .AnyAsync(la => la.UserId == userId &&
                                  (la.Status == "Approved" || la.Status == "Pending") &&
                                  la.IsActive == true);
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Failed to check active loans for user {userId}", ex);
            }
        }

        public async Task<IEnumerable<LoanApplication>> GetPendingApplicationsAsync()
        {
            try
            {
                return await _context.LoanApplications
                    .Where(la => la.Status == "Pending")
                    .Include(la => la.User)
                    .Include(la => la.LoanProduct)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Failed to retrieve pending loan applications", ex);
            }
        }

        public async Task<IEnumerable<LoanApplication>> GetApprovedApplicationsAsync()
        {
            try
            {
                return await _context.LoanApplications
                    .Where(la => la.Status == "Approved")
                    .Include(la => la.User)
                    .Include(la => la.LoanProduct)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Failed to retrieve approved loan applications", ex);
            }
        }

        public async Task<bool> IsAadhaarUsed(string aadhaar)
        {
            return await _context.LoanApplications
                .AnyAsync(l => l.Aadhaar == aadhaar);
        }

        public async Task<IEnumerable<LoanApplication>> GetAllLoanApplicationsAsync()
        {
            try
            {
                return await _context.LoanApplications
                    .Include(la => la.User)
                    .Include(la => la.LoanProduct)
                    .Include(la => la.LoanBankStatements)
                    .Include(la => la.LoanRepaymentSchedules)
                    .Include(la => la.PaymentTransactions)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Unable to fetch loan applications. Please try again later.", ex);
            }
        }
    }
}