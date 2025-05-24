using CredWiseAdmin.Core.Entities;
using CredWiseAdmin.Core.Exceptions;
using CredWiseAdmin.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CredWiseAdmin.Repository.Implementation
{
    public class LoanBankStatementRepository : ILoanBankStatementRepository
    {
        private readonly AppDbContext _context;

        public LoanBankStatementRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<LoanBankStatement> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new BadRequestException("Invalid bank statement ID");

            try
            {
                var statement = await _context.LoanBankStatements
                    .Include(bs => bs.LoanApplication)
                    .Include(bs => bs.VerifiedByNavigation)
                    .FirstOrDefaultAsync(bs => bs.BankStatementId == id);

                return statement ?? throw new NotFoundException($"Bank statement with ID {id} not found");
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Failed to retrieve bank statement with ID {id}", ex);
            }
        }

        public async Task<IEnumerable<LoanBankStatement>> GetByLoanApplicationIdAsync(int loanApplicationId)
        {
            if (loanApplicationId <= 0)
                throw new BadRequestException("Invalid loan application ID");

            try
            {
                return await _context.LoanBankStatements
                    .Where(bs => bs.LoanApplicationId == loanApplicationId)
                    .Include(bs => bs.VerifiedByNavigation)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException(
                    $"Failed to retrieve bank statements for loan application {loanApplicationId}",
                    ex);
            }
        }

        public async Task AddAsync(LoanBankStatement statement)
        {
            if (statement == null)
                throw new BadRequestException("Bank statement cannot be null");

            if (statement.LoanApplicationId <= 0)
                throw new BadRequestException("Invalid loan application reference");

            try
            {
                await _context.LoanBankStatements.AddAsync(statement);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new RepositoryException("Failed to add bank statement - database error", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Failed to add bank statement", ex);
            }
        }

        public async Task UpdateAsync(LoanBankStatement statement)
        {
            if (statement == null)
                throw new BadRequestException("Bank statement cannot be null");

            if (statement.BankStatementId <= 0)
                throw new BadRequestException("Invalid bank statement ID");

            try
            {
                _context.LoanBankStatements.Update(statement);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new RepositoryException("Bank statement may have been modified or deleted", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new RepositoryException("Failed to update bank statement - database error", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Failed to update bank statement", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new BadRequestException("Invalid bank statement ID");

            try
            {
                var statement = await GetByIdAsync(id);
                _context.LoanBankStatements.Remove(statement);
                await _context.SaveChangesAsync();
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (DbUpdateException ex)
            {
                throw new RepositoryException("Failed to delete bank statement - database error", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Failed to delete bank statement with ID {id}", ex);
            }
        }
    }
}