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
    public class PaymentTransactionRepository : IPaymentTransactionRepository
    {
        private readonly AppDbContext _context;

        public PaymentTransactionRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<PaymentTransaction>> GetByLoanApplicationIdAsync(int loanApplicationId)
        {
            try
            {
                return await _context.PaymentTransactions
                    .Where(pt => pt.LoanApplicationId == loanApplicationId)
                    .Include(pt => pt.LoanApplication)
                    .Include(pt => pt.Repayment)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving transactions by loan application ID.", ex);
            }
        }

        public async Task<PaymentTransaction> GetByIdAsync(int id)
        {
            try
            {
                var transaction = await _context.PaymentTransactions
                    .Include(pt => pt.LoanApplication)
                    .Include(pt => pt.Repayment)
                    .FirstOrDefaultAsync(pt => pt.TransactionId == id);

                if (transaction == null)
                    throw new NotFoundException($"Payment transaction with ID {id} was not found.");

                return transaction;
            }
            catch (CustomException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving payment transaction by ID.", ex);
            }
        }

        public async Task AddAsync(PaymentTransaction transaction)
        {
            if (transaction == null)
                throw new BadRequestException("Payment transaction cannot be null.");

            try
            {
                await _context.PaymentTransactions.AddAsync(transaction);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error adding payment transaction.", ex);
            }
        }

        public async Task UpdateAsync(PaymentTransaction transaction)
        {
            if (transaction == null)
                throw new BadRequestException("Payment transaction cannot be null.");

            try
            {
                _context.PaymentTransactions.Update(transaction);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error updating payment transaction.", ex);
            }
        }
    }
}