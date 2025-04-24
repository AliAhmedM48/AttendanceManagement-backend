using Core.ViewModels;
using Microsoft.AspNetCore.Http;

namespace Core.Interfaces.Services;
public interface IEmployeeService
{
    public Task<IEnumerable<EmployeeViewModel>> GetAll();
    public Task<EmployeeViewModel> GetEmployeeById(int id);


    public Task<int> CreateAsync(EmployeeCreateViewModel employeeCreateViewModel, IFormFile signatureFile);

    public Task<bool> UpdateAsync(int id, EmployeeUpdateViewModel employeeUpdateViewModel, IFormFile signatureFile);

    public Task<string> UpdateSignatureAsync(int id, IFormFile signature);
    public Task<EmployeeViewModel> GetProfileDataByEmployeeId(int employeeId);

    public Task<IEnumerable<AttendanceViewModel>> GetAttendanceByEmployeeId(int employeeId);

    public Task<bool> DeleteOneAsync(int id);
    public Task<AttendanceViewModel?> GetTodayAttendanceByEmployeeId(int employeeId);

}
