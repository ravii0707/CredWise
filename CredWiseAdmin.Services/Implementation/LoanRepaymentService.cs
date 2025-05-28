using AutoMapper;
using CredWiseAdmin.Core.DTOs;
using CredWiseAdmin.Core.Entities;
using CredWiseAdmin.Repository.Interfaces;
using CredWiseAdmin.Services.Interfaces;
using CredWiseAdmin.Core.Exceptions; // Assuming your exceptions are here
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace CredWiseAdmin.Services.Implementation
{
    public class LoanRepaymentService : ILoanRepaymentService
    {
        private readonly ILoanRepaymentRepository _loanRepaymentRepository;
        private readonly IPaymentTransactionRepository _paymentTransactionRepository;
        private readonly ILoanApplicationRepository _loanApplicationRepository;
        private readonly IMapper _mapper;

        public LoanRepaymentService(
            ILoanRepaymentRepository loanRepaymentRepository,
            IPaymentTransactionRepository paymentTransactionRepository,
            ILoanApplicationRepository loanApplicationRepository,
            IMapper mapper)
        {
            _loanRepaymentRepository = loanRepaymentRepository;
            _paymentTransactionRepository = paymentTransactionRepository;
            _loanApplicationRepository = loanApplicationRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LoanRepaymentDto>> GetRepaymentsByLoanIdAsync(int loanApplicationId)
        {
            var repayments = await _loanRepaymentRepository.GetByLoanApplicationIdAsync(loanApplicationId);
            return _mapper.Map<IEnumerable<LoanRepaymentDto>>(repayments);
        }

        public async Task<PaymentTransactionResponseDto> ProcessPaymentAsync(PaymentTransactionDto paymentDto)
        {
            var repayment = await _loanRepaymentRepository.GetByIdAsync(paymentDto.RepaymentId);
            if (repayment == null)
            {
                throw new NotFoundException("Repayment schedule not found");
            }

            if (repayment.Status == "Paid")
            {
                throw new InvalidOperationException("This installment is already paid");
            }

            // Create payment transaction
            var transaction = new PaymentTransaction
            {
                LoanApplicationId = paymentDto.LoanApplicationId,
                RepaymentId = paymentDto.RepaymentId,
                Amount = paymentDto.Amount,
                PaymentDate = DateTime.UtcNow,
                PaymentMethod = paymentDto.PaymentMethod,
                TransactionStatus = "Completed",
                TransactionReference = Guid.NewGuid().ToString(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
                CreatedBy = "System",
                ModifiedBy = "System"
            };

            await _paymentTransactionRepository.AddAsync(transaction);

            // Update repayment status
            repayment.Status = "Paid";
            repayment.ModifiedAt = DateTime.UtcNow;
            repayment.ModifiedBy = "System";

            await _loanRepaymentRepository.UpdateAsync(repayment);

            return _mapper.Map<PaymentTransactionResponseDto>(transaction);
        }

        public async Task<bool> ApplyPenaltyAsync(int repaymentId)
        {
            var repayment = await _loanRepaymentRepository.GetByIdAsync(repaymentId);
            if (repayment == null)
            {
                throw new NotFoundException("Repayment schedule not found");
            }

            // Add penalty amount
            repayment.TotalAmount += 500; // ₹500 penalty
            repayment.ModifiedAt = DateTime.UtcNow;
            repayment.ModifiedBy = "System";

            await _loanRepaymentRepository.UpdateAsync(repayment);
            return true;
        }

        public async Task<IEnumerable<LoanRepaymentDto>> GetPendingRepaymentsAsync(int userId)
        {
            var repayments = await _loanRepaymentRepository.GetPendingRepaymentsAsync(userId);
            return _mapper.Map<IEnumerable<LoanRepaymentDto>>(repayments);
        }

        public async Task<IEnumerable<LoanRepaymentDto>> GetOverdueRepaymentsAsync()
        {
            var repayments = await _loanRepaymentRepository.GetOverdueRepaymentsAsync();
            return _mapper.Map<IEnumerable<LoanRepaymentDto>>(repayments);
        }

        public async Task<IEnumerable<LoanRepaymentDto>> GetAllRepaymentsAsync()
        {
            try
            {
                var repayments = await _loanRepaymentRepository.GetAllRepaymentsAsync();
                if (!repayments.Any())
                {
                    throw new NotFoundException("No loan repayments found in the system.");
                }
                return _mapper.Map<IEnumerable<LoanRepaymentDto>>(repayments);
            }
            catch (CustomException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ServiceException("Failed to retrieve loan repayments. Please try again later.", ex);
            }
        }
    }
}