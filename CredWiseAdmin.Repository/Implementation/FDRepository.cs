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
    public class FDRepository : IFDRepository
    {
        private readonly AppDbContext _context;

        public FDRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Fdtype>> GetAllFDTypesAsync()
        {
            try
            {
                return await _context.Fdtypes
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Failed to retrieve FD types", ex);
            }
        }

        public async Task<Fdtype> GetFDTypeByIdAsync(int id)
        {
            if (id <= 0)
                throw new BadRequestException("Invalid FD type ID");

            try
            {
                var fdType = await _context.Fdtypes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(f => f.FdtypeId == id);

                return fdType ?? throw new NotFoundException($"FD type with ID {id} not found");
            }
            catch (NotFoundException)
            {
                throw; // Re-throw custom exceptions
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Failed to retrieve FD type with ID {id}", ex);
            }
        }

        public async Task<IEnumerable<Fdapplication>> GetFDApplicationsByUserIdAsync(int userId)
        {
            if (userId <= 0)
                throw new BadRequestException("Invalid user ID");

            try
            {
                return await _context.Fdapplications
                    .Where(fd => fd.UserId == userId)
                    .Include(fd => fd.Fdtype)
                    .Include(fd => fd.Fdtransactions)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Failed to retrieve FD applications for user {userId}", ex);
            }
        }

        public async Task<Fdapplication> GetFDApplicationByIdAsync(int id)
        {
            if (id <= 0)
                throw new BadRequestException("Invalid FD application ID");

            try
            {
                var application = await _context.Fdapplications
                    .Include(fd => fd.Fdtype)
                    .Include(fd => fd.Fdtransactions)
                    .Include(fd => fd.User)
                    .FirstOrDefaultAsync(fd => fd.FdapplicationId == id);

                return application ?? throw new NotFoundException($"FD application with ID {id} not found");
            }
            catch (NotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Failed to retrieve FD application with ID {id}", ex);
            }
        }

        public async Task AddFDApplicationAsync(Fdapplication application)
        {
            if (application == null)
                throw new BadRequestException("FD application cannot be null");

            try
            {
                await _context.Fdapplications.AddAsync(application);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new RepositoryException("Failed to create FD application - database error", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Failed to create FD application", ex);
            }
        }

        public async Task AddFDTransactionAsync(Fdtransaction transaction)
        {
            if (transaction == null)
                throw new BadRequestException("FD transaction cannot be null");

            if (transaction.FdapplicationId <= 0)
                throw new BadRequestException("Invalid FD application reference");

            try
            {
                await _context.Fdtransactions.AddAsync(transaction);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new RepositoryException("Failed to add FD transaction - database error", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Failed to add FD transaction", ex);
            }
        }
    }
}