using Application.Services.Address;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Application.Services.Employee;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Presentation.DtoMapping;
using Presentation.Models;

namespace Presentation.Controllers
{
    [Authorize]
    public class AddressController : Controller
    {
        private readonly IAddressService _addressService;
        private readonly IEmployeeService _employeeService;

        public AddressController(IAddressService addressService, IEmployeeService employeeService)
        {
            _addressService = addressService;
            _employeeService = employeeService;
        }

        // GET: Address/Add
        [HttpGet]
        public async Task<IActionResult> Add(Guid employeeId)
        {
            if (!User.IsInRole("Admin"))
            {
                var currentEmployeeId = await GetCurrentEmployeeIdAsync();
                if (!currentEmployeeId.HasValue || currentEmployeeId.Value != employeeId)
                {
                    return Forbid();
                }
            }

            var model = new CreateAddressViewModel
            {
                EmployeeId = employeeId
            };

            PopulateStatesDropdown();

            return View(model);
        }

        // POST: Address/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CreateAddressViewModel model)
        {
            if (!ModelState.IsValid)
            {
                PopulateStatesDropdown();
                return View(model);
            }

            if (!User.IsInRole("Admin"))
            {
                var currentEmployeeId = await GetCurrentEmployeeIdAsync();
                if (!currentEmployeeId.HasValue || currentEmployeeId.Value != model.EmployeeId)
                {
                    return Forbid();
                }
            }

            var dto = model.ToDto();
            await _addressService.AddAddressAsync(dto);

            return RedirectToAction("Details", "Employee", new { id = model.EmployeeId });
        }

        // GET: Address/Update
        [HttpGet]
        public async Task<IActionResult> Update(Guid employeeId)
        {
            if (!User.IsInRole("Admin"))
            {
                var currentEmployeeId = await GetCurrentEmployeeIdAsync();
                if (!currentEmployeeId.HasValue || currentEmployeeId.Value != employeeId)
                {
                    return Forbid();
                }
            }

            var dto = await _addressService.GetAddressByEmployeeIdAsync(employeeId);
            if (dto == null)
            {
                return RedirectToAction("Details", "Employee", new { id = employeeId });
            }

            var model = dto.ToUpdateAddressViewModel();
            PopulateStatesDropdown();

            return View(model);
        }

        // POST: Address/Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateAddressViewModel model)
        {
            if (!ModelState.IsValid)
            {
                PopulateStatesDropdown();
                return View(model);
            }

            if (!User.IsInRole("Admin"))
            {
                var currentEmployeeId = await GetCurrentEmployeeIdAsync();
                if (!currentEmployeeId.HasValue || currentEmployeeId.Value != model.EmployeeId)
                {
                    return Forbid();
                }
            }

            var dto = model.ToDto();
            await _addressService.UpdateAddressAsync(dto);

            return RedirectToAction("Details", "Employee", new { id = model.EmployeeId });
        }

        // POST: Address/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid employeeId)
        {
            if (!User.IsInRole("Admin"))
            {
                var currentEmployeeId = await GetCurrentEmployeeIdAsync();
                if (!currentEmployeeId.HasValue || currentEmployeeId.Value != employeeId)
                {
                    return Forbid();
                }
            }

            await _addressService.DeleteAddressAsync(employeeId);
            return RedirectToAction("Details", "Employee", new { id = employeeId });
        }

        private void PopulateStatesDropdown()
        {
            ViewBag.States = _addressService.GetAllStates()
                .Select(state => new SelectListItem
                {
                    Text = state,
                    Value = state
                })
                .ToList();
        }

        private async Task<Guid?> GetCurrentEmployeeIdAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return null;
            }

            var employee = await _employeeService.GetEmployeeByUserIdAsync(userId);
            return employee?.Id;
        }
    }
}
