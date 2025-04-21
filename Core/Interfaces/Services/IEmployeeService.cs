using Core.ViewModels;

namespace Core.Interfaces.Services;
public interface IEmployeeService
{
    public Task<IEnumerable<EmployeeViewModel>> GetAll();

    public Task<EmployeeViewModel> CreateAsync(EmployeeCreateViewModel employeeCreateViewModel);

    public Task<EmployeeViewModel> UpdateAsync(EmployeeUpdateViewModel employeeUpdateViewModel);

    public Task<bool> DeleteOneAsync(int id);
}
