using AutoMapper;
using IB.Core.Application.Dtos.Account;
using IB.Core.Application.ViewModels.Beneficiary;
using IB.Core.Application.ViewModels.CreditCard;
using IB.Core.Application.ViewModels.Loan;
using IB.Core.Application.ViewModels.Product;
using IB.Core.Application.ViewModels.SavingsAccount;
using IB.Core.Application.ViewModels.Transaction;
using IB.Core.Application.ViewModels.User;
using IB.Core.Domain.Entities;

namespace IB.Core.Application.Mappings
{
    public class GeneralProfile : Profile
    {
        public GeneralProfile()
        {
            #region UserProfile
            CreateMap<AuthenticationRequest, LoginViewModel>()
                .ForMember(x => x.HasError, opt => opt.Ignore())
                .ForMember(x => x.Error, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<RegisterRequest, SaveUserViewModel>()
                .ForMember(x => x.InitialAmount, opt => opt.Ignore())
                .ForMember(x => x.HasError, opt => opt.Ignore())
                .ForMember(x => x.Error, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(x => x.Role, opt => opt.MapFrom(src => src.UserType.ToString()));

            CreateMap<AuthenticationResponse, SaveUserViewModel>()
                .ForMember(x => x.UserType, opt => opt.MapFrom(src => src.Roles.FirstOrDefault()))
                .ForMember(x => x.HasError, opt => opt.Ignore())
                .ForMember(x => x.Error, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(x => x.Roles, opt => opt.MapFrom(src => new List<string> { src.UserType.ToString() }));

            CreateMap<AuthenticationResponse, UserViewModel>()
                .ForMember(x => x.UserType, opt => opt.MapFrom(src => src.Roles.FirstOrDefault()))
                .ForMember(x => x.HasError, opt => opt.Ignore())
                .ForMember(x => x.Error, opt => opt.Ignore())
                .ForMember(x => x.Products, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(x => x.Roles, opt => opt.MapFrom(src => new List<string> { src.UserType }));
            #endregion

            #region SavingsAccountProfile
            CreateMap<SavingsAccount, SavingsAccountViewModel>()
                .ForMember(dest => dest.UserName, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<SavingsAccount, SaveSavingsAccountViewModel>().ReverseMap();
            #endregion

            #region CreditCardProfile
            CreateMap<CreditCard, CreditCardViewModel>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserId))
                .ReverseMap();

            CreateMap<CreditCard, SaveCreditCardViewModel>().ReverseMap();
            #endregion

            #region LoanProfile
            CreateMap<Loan, LoanViewModel>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserId))
                .ReverseMap();

            CreateMap<Loan, SaveLoanViewModel>().ReverseMap();
            #endregion

            #region TransactionProfile
            CreateMap<Transaction, TransactionViewModel>()
                .ForMember(dest => dest.FromUserName, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.ToUserName, opt => opt.Ignore())

                // Manejar el numero de cuenta de origen dependiendo del tipo de transaccion
                .ForMember(dest => dest.FromAccountNumber, opt => opt.MapFrom(src =>
                    src.SavingsAccount != null ? src.SavingsAccount.AccountNumber :
                    src.CreditCard != null ? src.CreditCard.CardNumber :
                    src.Loan != null ? $"LOAN-{src.Loan.Id}" : "N/A"))

                // Manejar el numero de cuenta de destino dependiendo del tipo de transaccion
                .ForMember(dest => dest.ToAccountNumber, opt => opt.MapFrom(src =>
                    src.Beneficiary != null && src.Beneficiary.SavingsAccount != null ? src.Beneficiary.SavingsAccount.AccountNumber :
                    src.Loan != null ? $"LOAN-{src.Loan.Id}" :
                    src.CreditCard != null ? src.CreditCard.CardNumber :
                    "N/A"))

                .ReverseMap();

            CreateMap<Transaction, SaveTransactionViewModel>()
                .ForMember(dest => dest.ToAccountNumber, opt => opt.Ignore())
                .ReverseMap();
            #endregion

            #region BeneficiaryProfile
            CreateMap<Beneficiary, BeneficiaryViewModel>()
                .ForMember(dest => dest.UserOwnerId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.BeneficiaryName))
                .ForMember(dest => dest.AccountNumber, opt => opt.MapFrom(src => src.SavingsAccount.AccountNumber))
                .ReverseMap();

            CreateMap<Beneficiary, SaveBeneficiaryViewModel>().ReverseMap();
            #endregion

            #region ProductsProfile
            CreateMap<SavingsAccount, ProductsViewModel>()
                .ForMember(dest => dest.ProductType, opt => opt.MapFrom(src => "Cuenta de ahorro"))
                .ForMember(dest => dest.AccountNumber, opt => opt.MapFrom(src => src.AccountNumber))
                .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.Balance))
                .ForMember(dest => dest.Debt, opt => opt.MapFrom(src => 0)) // Las cuentas de ahorro no tienen deuda
                .ForMember(dest => dest.IsPrimary, opt => opt.MapFrom(src => src.IsPrimary))
                .ForMember(dest => dest.UserName, opt => opt.Ignore()) 
                .ReverseMap();

            CreateMap<SaveSavingsAccountViewModel, SaveProductViewModel>()
                .ForMember(dest => dest.ProductType, opt => opt.MapFrom(src => "Cuenta de ahorro"))
                .ForMember(dest => dest.IsPrimary, opt => opt.MapFrom(src => true)) // Siempre principal al crear
                .ReverseMap();

            CreateMap<CreditCard, ProductsViewModel>()
                .ForMember(dest => dest.ProductType, opt => opt.MapFrom(src => "Tarjeta de crédito"))
                .ForMember(dest => dest.AccountNumber, opt => opt.MapFrom(src => src.CardNumber))
                .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.CreditLimit)) // Puede ser CreditLimit o el saldo disponible
                .ForMember(dest => dest.Debt, opt => opt.MapFrom(src => src.Debt))
                .ForMember(dest => dest.IsPrimary, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.UserName, opt => opt.Ignore()) 
                .ReverseMap();

            CreateMap<SaveCreditCardViewModel, SaveProductViewModel>()
                .ForMember(dest => dest.ProductType, opt => opt.MapFrom(src => "Tarjeta de crédito"))
                .ReverseMap();


            CreateMap<Loan, ProductsViewModel>()
                .ForMember(dest => dest.ProductType, opt => opt.MapFrom(src => "Préstamo"))
                .ForMember(dest => dest.AccountNumber, opt => opt.MapFrom(src => $"LOAN-{src.Id}"))
                .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.Debt, opt => opt.MapFrom(src => src.RemainingBalance))
                .ForMember(dest => dest.IsPrimary, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.UserName, opt => opt.Ignore()) 
                .ReverseMap();
            #endregion


        }
    }

}
