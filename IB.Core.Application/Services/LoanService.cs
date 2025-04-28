using AutoMapper;
using IB.Core.Application.Interfaces.Repositories;
using IB.Core.Application.Interfaces.Services;
using IB.Core.Application.ViewModels.Loan;
using IB.Core.Domain.Entities;

namespace IB.Core.Application.Services
{
    public class LoanService : GenericService<SaveLoanViewModel, LoanViewModel, Loan>, ILoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IMapper _mapper;

        public LoanService(ILoanRepository loanRepository, IMapper mapper)
            : base(loanRepository, mapper)
        {
            _loanRepository = loanRepository;
            _mapper = mapper;
        }

        public async Task<List<LoanViewModel>> GetByUserIdAsync(string userId)
        {
            var entities = await _loanRepository.GetByUserIdAsync(userId);
            return _mapper.Map<List<LoanViewModel>>(entities);
        }

        public async Task<LoanViewModel?> GetByIdAsync(int id) 
        {
            var loan = await _loanRepository.GetByIdAsync(id);
            return _mapper.Map<LoanViewModel?>(loan);
        }
    }
}
