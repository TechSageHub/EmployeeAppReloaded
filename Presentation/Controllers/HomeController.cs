using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;
using Application.Services.Employee;
using Application.Services.Department;
using System.Security.Claims;

namespace Presentation.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IEmployeeService _employeeService;
    private readonly IDepartmentService _departmentService;

    public HomeController(ILogger<HomeController> logger, IEmployeeService employeeService, IDepartmentService departmentService)
    {
        _logger = logger;
        _employeeService = employeeService;
        _departmentService = departmentService;
    }

    public async Task<IActionResult> Index()
    {
        if (User.Identity!.IsAuthenticated)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employees = await _employeeService.GetAllEmployeesAsync(userId);
            var departments = await _departmentService.GetAllDepartmentsAsync();

            ViewBag.TotalEmployees = employees.Employees.Count;
            ViewBag.TotalDepartments = departments.Departments.Count;
            ViewBag.RecentEmployees = employees.Employees.OrderByDescending(e => e.HireDate).Take(5).ToList();
        }

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
