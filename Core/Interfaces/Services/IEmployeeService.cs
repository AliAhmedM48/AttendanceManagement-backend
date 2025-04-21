using Core.ViewModels;

namespace Core.Interfaces.Services;
public interface IEmployeeService
{
    public Task<IEnumerable<EmployeeViewModel>> GetAll();

    public Task<EmployeeViewModel> CreateAsync(EmployeeCreateViewModel employeeCreateViewModel);

    public Task<bool> UpdateAsync(int id, EmployeeUpdateViewModel employeeUpdateViewModel);
    public Task<bool> UpdateSignatureAsync(int id, string signature);


    public Task<bool> DeleteOneAsync(int id);
}
