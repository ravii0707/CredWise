using AutoMapper;
using CredWiseAdmin.Core.DTOs;
using CredWiseAdmin.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CredWiseAdmin.Services.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // User mappings
            CreateMap<RegisterUserDto, User>();
            CreateMap<User, UserResponseDto>();
            CreateMap<UpdateUserDto, User>();

            // Loan product mappings
            CreateMap<LoanProduct, LoanProductResponseDto>();
            CreateMap<PersonalLoanDetail, PersonalLoanDetailDto>();
            CreateMap<HomeLoanDetail, HomeLoanDetailDto>();
            CreateMap<GoldLoanDetail, GoldLoanDetailDto>();

            // Loan application mappings
            CreateMap<LoanApplicationDto, LoanApplication>();
            CreateMap<LoanApplication, LoanApplicationResponseDto>();

            // Loan bank statement mappings
            CreateMap<LoanBankStatement, BankStatementResponseDto>();

            // Loan repayment mappings
            CreateMap<LoanRepaymentSchedule, LoanRepaymentDto>();
            CreateMap<PaymentTransaction, PaymentTransactionDto>();
            CreateMap<PaymentTransactionDto, PaymentTransaction>();
            CreateMap<PaymentTransaction, PaymentTransactionResponseDto>();

            // FD mappings
            CreateMap<FDTypeDto, Fdtype>();
            CreateMap<Fdtype, FDTypeResponseDto>();
            CreateMap<FDApplicationDto, Fdapplication>();
            CreateMap<Fdapplication, FDApplicationResponseDto>();

            // Loan enquiry mappings
            CreateMap<LoanEnquiryDto, LoanEnquiry>();
        }
    }
}
