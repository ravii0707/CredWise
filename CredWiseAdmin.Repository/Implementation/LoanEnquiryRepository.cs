using CredWiseAdmin.Core.Entities;
using CredWiseAdmin.Core.Exceptions;
using CredWiseAdmin.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CredWiseAdmin.Repository.Implementation
{
    public class LoanEnquiryRepository : ILoanEnquiryRepository
    {
        private readonly AppDbContext _context;

        public LoanEnquiryRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<LoanEnquiry>> GetAllAsync()
        {
            try
            {
                return await _context.LoanEnquiries
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException(
                    "Unable to retrieve loan enquiries. Please try again later or contact support if the problem persists.", 
                    ex);
            }
        }

        public async Task<LoanEnquiry> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new BadRequestException("Invalid enquiry ID. Please provide a valid positive number.");

            try
            {
                return await _context.LoanEnquiries
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.EnquiryId == id);
            }
            catch (Exception ex)
            {
                throw new RepositoryException(
                    $"Unable to retrieve loan enquiry with ID {id}. Please try again later or contact support if the problem persists.", 
                    ex);
            }
        }

        public async Task AddAsync(LoanEnquiry enquiry)
        {
            try
            {
                if (enquiry == null)
                    throw new BadRequestException("Loan enquiry details cannot be empty. Please provide valid information.");

                await _context.LoanEnquiries.AddAsync(enquiry);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new RepositoryException(
                    "Unable to save the loan enquiry. Please check your input data and try again. If the problem persists, contact support.", 
                    ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException(
                    "An unexpected error occurred while saving the loan enquiry. Please try again later or contact support.", 
                    ex);
            }
        }
    }
} 