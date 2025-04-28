using IB.Core.Application.Interfaces.Services;
using IB.Core.Application.ViewModels.Beneficiary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Internet_banking.Controllers
{
    [Authorize(Roles = "Client")]
    public class BeneficiaryController : Controller
    {
        private readonly IBeneficiaryService _beneficiaryService;

        public BeneficiaryController(IBeneficiaryService beneficiaryService)
        {
            _beneficiaryService = beneficiaryService;
        }

        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // ID del usuario autenticado
            var beneficiaries = await _beneficiaryService.GetAllBeneficiaryViewModel();

            // Filtrar beneficiarios que pertenecen al usuario
            var userBeneficiaries = beneficiaries.Where(b => b.UserOwnerId == userId).ToList();

            return View(userBeneficiaries);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SaveBeneficiaryViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Debe ingresar un número de cuenta válido.";
                return RedirectToAction("Index");
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            vm.UserOwnerId = userId;

            var response = await _beneficiaryService.Add(vm);

            if (response.HasError)
            {
                TempData["ErrorMessage"] = response.Error;
            }
            else
            {
                TempData["SuccessMessage"] = "Beneficiario agregado correctamente.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _beneficiaryService.Delete(id);
            TempData["SuccessMessage"] = "Beneficiario eliminado correctamente.";
            return RedirectToAction("Index");
        }
    }
}
