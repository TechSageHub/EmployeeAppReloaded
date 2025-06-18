using Application.Dtos;
using Microsoft.AspNetCore.Http;

namespace Application.Services.Employee
{
    public interface IEmployeeService
    {
        Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto dto);
        Task DeleteEmployeeAsync(Guid employeeId);
        Task<EmployeesDto> GetAllEmployeesAsync();
        Task<EmployeeDto> GetEmployeeByIdAsync(Guid employeeId);
        Task<EmployeeDto> UpdateEmployeeAsync(UpdateEmployeeDto employeeDto);
        Task<string> UploadImageAsync(IFormFile file);
    }
}
