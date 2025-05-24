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
    public class LoanProductRepository : ILoanProductRepository
    {
        private readonly AppDbContext _context;

        public LoanProductRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<LoanProduct>> GetAllAsync()
        {
            try
            {
                return await _context.LoanProducts
                    .Include(lp => lp.PersonalLoanDetail)
                    .Include(lp => lp.HomeLoanDetail)
                    .Include(lp => lp.GoldLoanDetail)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Failed to retrieve loan products", ex);
            }
        }

        public async Task<LoanProduct> GetByIdAsync(int id)
        {
            try
            {
                var product = await _context.LoanProducts
                    .Include(lp => lp.PersonalLoanDetail)
                    .Include(lp => lp.HomeLoanDetail)
                    .Include(lp => lp.GoldLoanDetail)
                    .FirstOrDefaultAsync(lp => lp.LoanProductId == id);

                if (product == null)
                {
                    throw new NotFoundException($"Loan product with ID {id} not found");
                }

                return product;
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Failed to retrieve loan product with ID {id}", ex);
            }
        }

        public async Task<IEnumerable<LoanProduct>> GetActiveProductsAsync()
        {
            try
            {
                return await _context.LoanProducts
                    .Where(lp => lp.IsActive)
                    .Include(lp => lp.PersonalLoanDetail)
                    .Include(lp => lp.HomeLoanDetail)
                    .Include(lp => lp.GoldLoanDetail)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Failed to retrieve active loan products", ex);
            }
        }

        public async Task AddAsync(LoanProduct product)
        {
            try
            {
                if (product == null)
                {
                    throw new BadRequestException("Loan product cannot be null");
                }

                await _context.LoanProducts.AddAsync(product);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new RepositoryException("Failed to add loan product due to database error", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Failed to add loan product", ex);
            }
        }

        public async Task UpdateAsync(LoanProduct product)
        {
            try
            {
                if (product == null)
                {
                    throw new BadRequestException("Loan product cannot be null");
                }

                _context.LoanProducts.Update(product);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new RepositoryException("Concurrency conflict while updating loan product", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new RepositoryException("Failed to update loan product due to database error", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Failed to update loan product", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var product = await GetByIdAsync(id);
                if (product != null)
                {
                    product.IsActive = false;
                    await UpdateAsync(product);
                }
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Failed to delete loan product with ID {id}", ex);
            }
        }
    }
}