using CredWiseAdmin.Core.Entities;
using CredWiseAdmin.Core.Exceptions;
using CredWiseAdmin.Data;
using CredWiseAdmin.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CredWiseAdmin.Repository.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<User> GetByIdAsync(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);

                if (user == null || !(user.IsActive ?? false))
                    throw new NotFoundException($"User with ID {id} was not found or is inactive.");

                return user;
            }
            catch (CustomException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving user by ID.", ex);
            }
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new BadRequestException("Email cannot be null or empty.");

            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower().Trim() == email.ToLower().Trim());

                if (user == null || !(user.IsActive ?? false))
                    throw new NotFoundException($"User with email '{email}' not found or inactive.");

                return user;
            }
            catch (CustomException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving user by email.", ex);
            }
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            try
            {
                return await _context.Users
                    .Where(u => u.IsActive ?? false)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error retrieving all users.", ex);
            }
        }

        public async Task AddAsync(User user)
        {
            if (user == null)
                throw new BadRequestException("User cannot be null.");

            try
            {
                user.IsActive = true;
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error adding user.", ex);
            }
        }

        public async Task UpdateAsync(User user)
        {
            if (user == null)
                throw new BadRequestException("User cannot be null.");

            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error updating user.", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var user = await GetByIdAsync(id);
                user.IsActive = false; // Soft delete
                await UpdateAsync(user);
            }
            catch (CustomException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RepositoryException($"Error soft deleting user with ID {id}.", ex);
            }
        }

        public async Task<bool> EmailExists(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new BadRequestException("Email cannot be null or empty.");

            try
            {
                return await _context.Users
                    .AnyAsync(u => u.Email.ToLower().Trim() == email.ToLower().Trim() && (u.IsActive ?? false));
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error checking if email exists.", ex);
            }
        }

        public async Task<bool> AdminExists()
        {
            try
            {
                return await _context.Users
                    .AnyAsync(u => u.Role == "Admin" && (u.IsActive ?? false));
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error checking if any admin exists.", ex);
            }
        }

        public async Task<int> CountAdmins()
        {
            try
            {
                return await _context.Users
                    .CountAsync(u => u.Role == "Admin" && (u.IsActive ?? false));
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error counting active admins.", ex);
            }
        }

        public async Task<bool> UserExists(int id)
        {
            try
            {
                return await _context.Users
                    .AnyAsync(u => u.UserId == id && (u.IsActive ?? false));
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error checking if user exists.", ex);
            }
        }

        //Test
        public async Task<bool> AnyUserExists()
        {
            try
            {
                return await _context.Users.AnyAsync();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Error checking if any user exists", ex);
            }
        }
    }
}