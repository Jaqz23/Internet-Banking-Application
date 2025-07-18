﻿namespace IB.Core.Application.Interfaces.Services
{
    public interface IGenericService<SaveViewModel, ViewModel, Model>
           where SaveViewModel : class
           where ViewModel : class
           where Model : class
    {
        Task Update(SaveViewModel vm, int id);
        Task<SaveViewModel> Add(SaveViewModel vm);
        Task Delete(int id);
        Task<SaveViewModel> GetByIdSaveViewModel(int id);
        Task<List<ViewModel>> GetAllViewModel();
    }
}
