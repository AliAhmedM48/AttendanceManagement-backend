using Core.Interfaces;
using Core.Interfaces.Services;
using Core.Interfaces.Services.Auth;
using Core.Models;
using Core.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


namespace Service;

public class EmployeeService : IEmployeeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthenticationService _authenticationService;

    public EmployeeService(IUnitOfWork unitOfWork, IAuthenticationService authenticationService)
    {
        this._unitOfWork = unitOfWork;
        this._authenticationService = authenticationService;
    }

    public async Task<int> CreateAsync(EmployeeCreateViewModel employeeCreateViewModel, IFormFile signatureFile)
    {
        var employeeRepo = _unitOfWork.GetRepository<Employee>();

        if (await employeeRepo.AnyAsync(u => u.Email == employeeCreateViewModel.Email))
            throw new Exception("Email already exists");

        var (passwordHash, passwordSalt) = _authenticationService.CreatePasswordHash(employeeCreateViewModel.Password);
        var nationalIdInfo = ServiceHelpers.ExtractNationalIdInfoFromNationalId(employeeCreateViewModel.NationalId);

        string? savedSignatureFileName = null;

        if (signatureFile != null && signatureFile.Length > 0)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "signatures");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileExtension = Path.GetExtension(signatureFile.FileName);
            var fileName = $"signature_{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await signatureFile.CopyToAsync(stream);
            }

            savedSignatureFileName = fileName;
        }

        var employee = new Employee
        {
            FirstName = employeeCreateViewModel.FirstName,
            LastName = employeeCreateViewModel.LastName,
            BirthDate = nationalIdInfo.BirthDate,
            Gender = nationalIdInfo.Gender,
            Governorate = nationalIdInfo.Governorate,
            Email = employeeCreateViewModel.Email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            NationalId = employeeCreateViewModel.NationalId,
            PhoneNumber = employeeCreateViewModel.PhoneNumber,
            SignaturePath = savedSignatureFileName,
        };

        await employeeRepo.AddAsync(employee);
        await _unitOfWork.SaveChangesAsync();

        return employee.Id;
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
            CreatedAt = e.CreatedAt,
            Email = e.Email,
            BirthDate = e.BirthDate,
            Gender = e.Gender.ToString(),
            Governorate = e.Governorate,
            NationalId = e.NationalId,
            PhoneNumber = e.PhoneNumber,
            SignaturePath = string.IsNullOrWhiteSpace(e.SignaturePath) ? null : e.SignaturePath,
            UpdatedAt = e.UpdatedAt >= e.CreatedAt ? e.UpdatedAt : null
        });

        return employeeViewModels;
    }

    public async Task<EmployeeViewModel> GetEmployeeById(int id)
    {
        var employee = await _unitOfWork.GetRepository<Employee>().GetByIdAsync(id);

        if (employee == null)
            throw new KeyNotFoundException($"Employee with ID {id} not found.");

        var employeeViewModel = new EmployeeViewModel
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            FullName = employee.FirstName + " " + employee.LastName,
            CreatedAt = employee.CreatedAt,
            Email = employee.Email,
            BirthDate = employee.BirthDate,
            Gender = employee.Gender.ToString(),
            Governorate = employee.Governorate,
            NationalId = employee.NationalId,
            PhoneNumber = employee.PhoneNumber,
            SignaturePath = string.IsNullOrWhiteSpace(employee.SignaturePath) ? null : employee.SignaturePath,
            UpdatedAt = employee.UpdatedAt >= employee.CreatedAt ? employee.UpdatedAt : null
        };

        return employeeViewModel;
    }


    public async Task<bool> UpdateAsync(int id, EmployeeUpdateViewModel employeeUpdateViewModel, IFormFile signatureFile)
    {
        var nationalIdInfo = ServiceHelpers.ExtractNationalIdInfoFromNationalId(employeeUpdateViewModel.NationalId);

        var employee = await _unitOfWork.GetRepository<Employee>().GetByIdAsync(id);
        if (employee == null)
            throw new KeyNotFoundException($"Employee with ID {id} not found.");

        if (signatureFile != null && signatureFile.Length > 0)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "signatures");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            if (!string.IsNullOrWhiteSpace(employee.SignaturePath))
            {
                var oldFilePath = Path.Combine(uploadsFolder, employee.SignaturePath);
                if (File.Exists(oldFilePath))
                {
                    File.Delete(oldFilePath);
                }
            }

            var fileExtension = Path.GetExtension(signatureFile.FileName);
            var fileName = $"signature_{id}_{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await signatureFile.CopyToAsync(stream);
            }

            employee.SignaturePath = fileName;
        }

        employee.FirstName = employeeUpdateViewModel.FirstName;
        employee.LastName = employeeUpdateViewModel.LastName;
        employee.Email = employeeUpdateViewModel.Email;
        employee.NationalId = employeeUpdateViewModel.NationalId;
        employee.BirthDate = nationalIdInfo.BirthDate;
        employee.Gender = nationalIdInfo.Gender;
        employee.Governorate = nationalIdInfo.Governorate;
        employee.PhoneNumber = employeeUpdateViewModel.PhoneNumber;
        employee.UpdatedAt = DateTime.Now;

        _unitOfWork.GetRepository<Employee>().SaveInclude(employee,
           e => e.FirstName,
           e => e.LastName,
           e => e.Email,
           e => e.Gender,
           e => e.Governorate,
           e => e.BirthDate,
           e => e.NationalId,
           e => e.PhoneNumber,
           e => e.SignaturePath,
           e => e.UpdatedAt);

        await _unitOfWork.SaveChangesAsync();

        return true;
    }



    public async Task<IEnumerable<AttendanceViewModel>> GetAttendanceByEmployeeId(int employeeId)
    {
        var attendanceRecords = await _unitOfWork.GetRepository<Attendance>()
            .GetAll(a => a.EmployeeId == employeeId)
            .OrderByDescending(a => a.CheckInTime)
            .ToListAsync();

        if (attendanceRecords == null || !attendanceRecords.Any())
            return null;

        var attendanceViewModels = attendanceRecords.Select(a => new AttendanceViewModel
        {
            Id = a.Id,
            EmployeeId = a.EmployeeId,
            CheckInTime = a.CheckInTime,
            CheckOutTime = a.CheckOutTime,
            TotalWorkedHours = a.CheckOutTime.HasValue ? (a.CheckOutTime.Value - a.CheckInTime).TotalHours : 0
        });

        return attendanceViewModels;
    }

    public async Task<EmployeeViewModel> GetProfileDataByEmployeeId(int employeeId)
    {

        var employee = await _unitOfWork.GetRepository<Employee>().GetByIdAsync(employeeId);
        if (employee == null)
            throw new UnauthorizedAccessException("Employee not found.");

        var employeeViewModel = new EmployeeViewModel()
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            FullName = employee.FirstName + " " + employee.LastName,
            CreatedAt = employee.CreatedAt,
            Email = employee.Email,
            BirthDate = employee.BirthDate,
            Gender = employee.Gender.ToString(),
            Governorate = employee.Governorate,
            NationalId = employee.NationalId,
            PhoneNumber = employee.PhoneNumber,
            SignaturePath = string.IsNullOrWhiteSpace(employee.SignaturePath) ? null : employee.SignaturePath,
            UpdatedAt = employee.UpdatedAt >= employee.CreatedAt ? employee.UpdatedAt : null
        };

        return employeeViewModel;
    }


    public async Task<string> UpdateSignatureAsync(int id, IFormFile signature)
    {
        var employee = await _unitOfWork.GetRepository<Employee>().GetByIdAsync(id);
        if (employee == null)
            return null;

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "signatures");

        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        if (!string.IsNullOrWhiteSpace(employee.SignaturePath))
        {
            var oldFilePath = Path.Combine(uploadsFolder, employee.SignaturePath);
            if (File.Exists(oldFilePath))
            {
                File.Delete(oldFilePath);
            }
        }

        var fileExtension = Path.GetExtension(signature.FileName);
        var fileName = $"signature_{id}_{Guid.NewGuid()}{fileExtension}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await signature.CopyToAsync(stream);
        }

        employee.SignaturePath = fileName;
        employee.UpdatedAt = DateTime.Now;

        _unitOfWork.GetRepository<Employee>().SaveInclude(employee, e => e.SignaturePath, e => e.UpdatedAt);
        await _unitOfWork.SaveChangesAsync();

        return fileName;
    }


    public async Task<AttendanceViewModel?> GetTodayAttendanceByEmployeeId(int employeeId)
    {
        var today = DateTime.Today;

        var attendance = await _unitOfWork.GetRepository<Attendance>()
            .GetAll(a => a.EmployeeId == employeeId && a.CheckInTime.Date == today)
            .OrderByDescending(a => a.CheckInTime)
            .FirstOrDefaultAsync();

        if (attendance == null)
            return null;

        return new AttendanceViewModel
        {
            Id = attendance.Id,
            EmployeeId = attendance.EmployeeId,
            CheckInTime = attendance.CheckInTime,
            CheckOutTime = attendance.CheckOutTime,
            TotalWorkedHours = attendance.CheckOutTime.HasValue ? (attendance.CheckOutTime.Value - attendance.CheckInTime).TotalHours : 0
        };
    }

}
