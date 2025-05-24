using AutoMapper;
using CredWiseAdmin.Core.DTOs;
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
    }
}