using Core.Interfaces;
using Core.Interfaces.Services;
using Core.Models;
using Core.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Service;

public class EmployeeService : IEmployeeService
{
    private readonly IUnitOfWork _unitOfWork;

    public EmployeeService(IUnitOfWork unitOfWork)
    {
        this._unitOfWork = unitOfWork;
    }
    public async Task<EmployeeViewModel> CreateAsync(EmployeeCreateViewModel employeeCreateViewModel)
    {
        var employee = new Employee()
        {
            FirstName = employeeCreateViewModel.FirstName,
            LastName = employeeCreateViewModel.LastName,
            Age = employeeCreateViewModel.Age,
            Email = employeeCreateViewModel.Email,
            NationalId = employeeCreateViewModel.NationalId,
            PhoneNumber = employeeCreateViewModel.PhoneNumber,
            SignaturePath = employeeCreateViewModel.SignaturePath ?? null,
        };

        await _unitOfWork.GetRepository<Employee>().AddAsync(employee);
        await _unitOfWork.SaveChangesAsync();

        return new EmployeeViewModel
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Age = employee.Age,
            Email = employee.Email,
            NationalId = employee.NationalId,
            PhoneNumber = employee.PhoneNumber,
            SignaturePath = employee.SignaturePath ?? null,
            FullName = employee.FirstName + " " + employee.LastName,
            CreatedAt = employee.CreatedAt,
            UpdatedAt = employee.UpdatedAt >= employee.CreatedAt ? employee.UpdatedAt : null
        };
    }

    public async Task<bool> DeleteOneAsync(int id)
    {
        var repository = _unitOfWork.GetRepository<Employee>();
        var employee = await repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Employee with ID {id} not found.");

        repository.Delete(employee);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<EmployeeViewModel>> GetAll()
    {
        var employees = await _unitOfWork.GetRepository<Employee>().GetAll().ToListAsync();
        var employeeViewModels = employees.Select(e => new EmployeeViewModel()
        {
            Id = e.Id,
            FirstName = e.FirstName,
            LastName = e.LastName,
            FullName = e.FirstName + " " + e.LastName,
            Age = e.Age,
            CreatedAt = e.CreatedAt,
            Email = e.Email,
            NationalId = e.NationalId,
            PhoneNumber = e.PhoneNumber,
            SignaturePath = e.SignaturePath,
            UpdatedAt = e.UpdatedAt >= e.CreatedAt ? e.UpdatedAt : null
        });

        return employeeViewModels;
    }

    public async Task<bool> UpdateAsync(int id, EmployeeUpdateViewModel employeeUpdateViewModel)
    {
        var employee = new Employee()
        {
            Id = id,
            FirstName = employeeUpdateViewModel.FirstName,
            LastName = employeeUpdateViewModel.LastName,
            Email = employeeUpdateViewModel.Email,
            Age = employeeUpdateViewModel.Age!.Value,
            NationalId = employeeUpdateViewModel.NationalId,
            PhoneNumber = employeeUpdateViewModel.PhoneNumber,
            SignaturePath = employeeUpdateViewModel.SignaturePath,
            UpdatedAt = DateTime.Now,
        };

        _unitOfWork.GetRepository<Employee>().SaveInclude(employee,
           e => e.FirstName,
           e => e.LastName,
           e => e.Email,
           e => e.Age,
           e => e.NationalId,
           e => e.PhoneNumber,
           e => e.SignaturePath,
           e => e.UpdatedAt);

        await _unitOfWork.SaveChangesAsync();

        return true;

    }

    public async Task<bool> UpdateSignatureAsync(int id, string signature)
    {
        var employee = new Employee()
        {
            Id = id,
            SignaturePath = signature,
        };
        _unitOfWork.GetRepository<Employee>().SaveInclude(employee, e => e.SignaturePath);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
