using AutoMapper;
using CredWiseAdmin.Core.DTOs;
using CredWiseAdmin.Core.Entities;
using CredWiseAdmin.Repository.Interfaces;
using CredWiseAdmin.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CredWiseAdmin.Services.Implementation
{
    public class FDService : IFDService
    {
        private readonly IFDRepository _fdRepository;
        private readonly IMapper _mapper;

        private const string DefaultStatus = "Active";
        private const string CreatedBySystem = "System";
        private const string Deposit = "Deposit";
        private const string Completed = "Completed";
        private const string PaymentMethod = "Bank Transfer";

        public FDService(IFDRepository fdRepository, IMapper mapper)
        {
            _fdRepository = fdRepository ?? throw new ArgumentNullException(nameof(fdRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<FDTypeResponseDto>> GetAllFDTypesAsync()
        {
            var fdTypes = await _fdRepository.GetAllFDTypesAsync();
            return _mapper.Map<IEnumerable<FDTypeResponseDto>>(fdTypes);
        }

        public async Task<FDApplicationResponseDto> CreateFDApplicationAsync(FDApplicationDto fdApplicationDto)
        {
            if (fdApplicationDto == null)
                throw new ArgumentNullException(nameof(fdApplicationDto));

            var fdType = await _fdRepository.GetFDTypeByIdAsync(fdApplicationDto.FDTypeId);
            if (fdType == null)
                throw new KeyNotFoundException("FD type not found.");

            if (fdApplicationDto.Amount < fdType.MinAmount || fdApplicationDto.Amount > fdType.MaxAmount)
                throw new ArgumentOutOfRangeException(nameof(fdApplicationDto.Amount), $"Amount must be between {fdType.MinAmount} and {fdType.MaxAmount}");

            var fdApplication = _mapper.Map<Fdapplication>(fdApplicationDto);

            fdApplication.InterestRate = fdType.InterestRate;
            fdApplication.Status = DefaultStatus;
            fdApplication.MaturityDate = DateTime.UtcNow.AddMonths(fdApplicationDto.Duration);
            fdApplication.MaturityAmount = CalculateMaturityAmount(fdApplicationDto.Amount, fdType.InterestRate, fdApplicationDto.Duration);
            fdApplication.IsActive = true;
            fdApplication.CreatedAt = DateTime.UtcNow;
            fdApplication.ModifiedAt = DateTime.UtcNow;
            fdApplication.CreatedBy = CreatedBySystem;
            fdApplication.ModifiedBy = CreatedBySystem;

            await _fdRepository.AddFDApplicationAsync(fdApplication);

            var transaction = new Fdtransaction
            {
                FdapplicationId = fdApplication.FdapplicationId,
                TransactionType = Deposit,
                Amount = fdApplicationDto.Amount,
                TransactionDate = DateTime.UtcNow,
                PaymentMethod = PaymentMethod,
                TransactionStatus = Completed,
                TransactionReference = Guid.NewGuid().ToString(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
                CreatedBy = CreatedBySystem,
                ModifiedBy = CreatedBySystem
            };

            await _fdRepository.AddFDTransactionAsync(transaction);

            var response = _mapper.Map<FDApplicationResponseDto>(fdApplication);
            response.FDTypeName = fdType.Name;

            return response;
        }

        public async Task<IEnumerable<FDApplicationResponseDto>> GetFDApplicationsByUserIdAsync(int userId)
        {
            var applications = await _fdRepository.GetFDApplicationsByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<FDApplicationResponseDto>>(applications);
        }

        public async Task<FDApplicationResponseDto> GetFDApplicationByIdAsync(int id)
        {
            var application = await _fdRepository.GetFDApplicationByIdAsync(id);
            if (application == null)
                throw new KeyNotFoundException($"FD application with ID {id} not found.");

            return _mapper.Map<FDApplicationResponseDto>(application);
        }

        private decimal CalculateMaturityAmount(decimal principal, decimal interestRate, int durationInMonths)
        {
            decimal interest = principal * (interestRate / 100m) * (durationInMonths / 12m);
            return principal + interest;
        }
    }
}