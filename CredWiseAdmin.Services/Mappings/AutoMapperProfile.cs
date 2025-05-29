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
            CreateMap<RegisterUserDto, User>()
             .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.ToLower()));
            CreateMap<User, UserResponseDto>()
     .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));
            CreateMap<UpdateUserDto, User>();

   

            // Loan product mappings
            CreateMap<LoanProduct, LoanProductResponseDto>();
            CreateMap<PersonalLoanDetail, PersonalLoanDetailDto>();
            CreateMap<HomeLoanDetail, HomeLoanDetailDto>();
            CreateMap<GoldLoanDetail, GoldLoanDetailDto>();

            // Loan application mappings
            CreateMap<LoanApplicationDto, LoanApplication>()
                .ForMember(dest => dest.Dob, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.DOB.Date)));
            CreateMap<LoanApplication, LoanApplicationResponseDto>()
                .ForMember(dest => dest.DOB, opt => opt.MapFrom(src => src.Dob.ToDateTime(TimeOnly.MinValue)));

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

            // Enquiry mappings
         
        }
    }
}
