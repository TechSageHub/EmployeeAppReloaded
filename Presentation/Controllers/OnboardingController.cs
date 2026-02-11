using System.Security.Claims;
using Application.Dtos;
using Application.Services.Onboarding;
using Application.Services.Employee;
using Application.Services.Document;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Presentation.Models;

namespace Presentation.Controllers;

[Authorize]
[IgnoreAntiforgeryToken]
public class OnboardingController : BaseController
{
    private readonly IOnboardingService _onboardingService;
    private readonly IEmployeeService _employeeService;
    private readonly IDocumentService _documentService;
    private readonly INotyfService _notyf;

    public OnboardingController(IOnboardingService onboardingService, IEmployeeService employeeService, IDocumentService documentService, INotyfService notyf)
    {
        _onboardingService = onboardingService;
        _employeeService = employeeService;
        _documentService = documentService;
        _notyf = notyf;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var snapshot = await _onboardingService.GetSnapshotAsync(userId!);
        if (snapshot == null)
        {
            _notyf.Error("Employee profile not found. Please contact HR.");
            return RedirectToAction("Login", "Account");
        }

        var employee = await _employeeService.GetEmployeeByUserIdAsync(userId!);
        var documents = employee == null
            ? new List<EmployeeDocumentDto>()
            : await _documentService.GetEmployeeDocumentsAsync(employee.Id);

        var model = OnboardingViewModel.FromSnapshot(snapshot);
        model.Documents = documents;
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> PersonalDetails([Bind(Prefix = "PersonalDetails")] PersonalDetailsViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ToErrorResponse("Please complete all required fields.", ModelState));
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var progress = await _onboardingService.UpdatePersonalDetailsAsync(userId!, model.ToDto());
        return Json(progress);
    }

    [HttpPost]
    public async Task<IActionResult> Qualification([Bind(Prefix = "Qualification")] QualificationViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ToErrorResponse("Please provide valid qualification details.", ModelState));
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var progress = await _onboardingService.AddQualificationAsync(userId!, model.ToDto());
        return Json(progress);
    }

    [HttpPost]
    public async Task<IActionResult> NextOfKin([Bind(Prefix = "NextOfKin")] NextOfKinViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ToErrorResponse("Please provide your next of kin details.", ModelState));
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var progress = await _onboardingService.SaveNextOfKinAsync(userId!, model.ToDto());
        return Json(progress);
    }

    [HttpPost]
    public async Task<IActionResult> HrInfo([Bind(Prefix = "HrInfo")] HrInfoViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ToErrorResponse("Please provide the required HR information.", ModelState));
        }

        if (model.DateOfBirth.Date >= DateTime.Today)
        {
            return BadRequest(new { message = "Date of birth must be in the past." });
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var progress = await _onboardingService.SaveHrInfoAsync(userId!, model.ToDto());
        return Json(progress);
    }

    [HttpPost]
    public async Task<IActionResult> UploadDocument(UploadDocumentDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { message = "Please provide a document type and file." });
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var employee = await _employeeService.GetEmployeeByUserIdAsync(userId!);
        if (employee == null)
        {
            return BadRequest(new { message = "Employee profile not found." });
        }

        dto.EmployeeId = employee.Id;

        try
        {
            await _documentService.UploadDocumentAsync(dto);
            var progress = await _onboardingService.UpdateDocumentsProgressAsync(userId!);
            return Json(progress);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    private static object ToErrorResponse(string message, ModelStateDictionary modelState)
    {
        var errors = modelState
            .Where(kvp => kvp.Value?.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? []);

        return new { message, errors };
    }
}
