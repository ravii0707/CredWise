using AutoMapper;
using CredWiseAdmin.Core.DTOs;
using CredWiseAdmin.Core.Entities;
using CredWiseAdmin.Core.Exceptions;
using CredWiseAdmin.Repository.Interfaces;
using CredWiseAdmin.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CredWiseAdmin.Services.Implementation
{
    public class LoanProductService : ILoanProductService
    {
        private readonly ILoanProductRepository _loanProductRepository;
        private readonly IMapper _mapper;

        public LoanProductService(ILoanProductRepository loanProductRepository, IMapper mapper)
        {
            _loanProductRepository = loanProductRepository ?? throw new ArgumentNullException(nameof(loanProductRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<LoanProductResponseDto>> GetAllLoanProductsAsync()
        {
            try
            {
                var products = await _loanProductRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<LoanProductResponseDto>>(products);
            }
            catch (Exception ex)
            {
                // Log exception here if you have logging
                throw new Exception("An error occurred while retrieving loan products.", ex);
            }
        }

        public async Task<LoanProductResponseDto> GetLoanProductByIdAsync(int id)
        {
            try
            {
                var product = await _loanProductRepository.GetByIdAsync(id);
                if (product == null)
                {
                    throw new NotFoundException($"Loan product with ID {id} was not found.");
                }
                return _mapper.Map<LoanProductResponseDto>(product);
            }
            catch (NotFoundException)
            {
                throw; // rethrow NotFoundException without wrapping
            }
            catch (Exception ex)
            {
                // Log exception here if you have logging
                throw new Exception($"An error occurred while retrieving loan product with ID {id}.", ex);
            }
        }

        public async Task<LoanProductResponseDto> CreateLoanProductAsync(LoanProductDto loanProductDto)
        {
            try
            {
                if (loanProductDto == null)
                {
                    throw new BadRequestException("Loan product data cannot be null");
                }

                // Map DTO to entity
                var loanProduct = _mapper.Map<LoanProduct>(loanProductDto);

                // Set common properties
                loanProduct.IsActive = true;
                loanProduct.CreatedAt = DateTime.UtcNow;
                loanProduct.ModifiedAt = DateTime.UtcNow;
                loanProduct.CreatedBy = "Admin"; // Replace with actual user from auth context
                loanProduct.ModifiedBy = "Admin";

                // Create specific loan detail based on type
                switch (loanProductDto.LoanType?.ToUpper())
                {
                    case "PERSONAL":
                        loanProduct.PersonalLoanDetail = new PersonalLoanDetail
                        {
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow,
                            ModifiedAt = DateTime.UtcNow,
                            CreatedBy = "Admin",
                            ModifiedBy = "Admin"
                        };
                        break;
                    case "HOME":
                        loanProduct.HomeLoanDetail = new HomeLoanDetail
                        {
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow,
                            ModifiedAt = DateTime.UtcNow,
                            CreatedBy = "Admin",
                            ModifiedBy = "Admin"
                        };
                        break;
                    case "GOLD":
                        loanProduct.GoldLoanDetail = new GoldLoanDetail
                        {
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow,
                            ModifiedAt = DateTime.UtcNow,
                            CreatedBy = "Admin",
                            ModifiedBy = "Admin"
                        };
                        break;
                    default:
                        throw new BadRequestException("Invalid loan type specified");
                }

                await _loanProductRepository.AddAsync(loanProduct);
                return _mapper.Map<LoanProductResponseDto>(loanProduct);
            }
            catch (Exception ex) when (ex is not BadRequestException and not NotFoundException)
            {
                throw new Exception("An error occurred while creating the loan product.", ex);
            }
        }
    }
}