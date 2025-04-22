using Core.Interfaces;
using Core.Interfaces.Services;
using Core.Models;
using Core.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Service;

public class AttendanceService : IAttendanceService
{
    private readonly IUnitOfWork _unitOfWork;

    public AttendanceService(IUnitOfWork unitOfWork)
    {
        this._unitOfWork = unitOfWork;
    }
    public async Task<AttendanceViewModel> CreateAsync(AttendanceCreateViewModel attendanceCreateViewModel)
    {
        var attendance = new Attendance()
        {
            EmployeeId = attendanceCreateViewModel.EmployeeId,
            CheckInTime = DateTime.Now,
            CreatedAt = DateTime.Now,
        };

        await _unitOfWork.GetRepository<Attendance>().AddAsync(attendance);
        await _unitOfWork.SaveChangesAsync();

        return new AttendanceViewModel
        {

            Id = attendance.Id,
            CheckInTime = attendance.CheckInTime,
            EmployeeId = attendance.EmployeeId,
            CheckOutTime = attendance.CheckOutTime >= attendance.CheckInTime ? attendance.CheckOutTime : null,
        };
    }

    public async Task<bool> DeleteOneAsync(int id)
    {
        var repository = _unitOfWork.GetRepository<Attendance>();
        var attendance = await repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Attendance with ID {id} not found.");

        repository.Delete(attendance);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<AttendanceViewModel>> GetAll()
    {
        var attendances = await _unitOfWork.GetRepository<Attendance>().GetAll().ToListAsync();
        var attendanceViewModels = attendances.Select(a => new AttendanceViewModel()
        {
            Id = a.Id,
            CheckInTime = a.CheckInTime,
            CheckOutTime = a.CheckOutTime >= a.CheckInTime ? a.CheckOutTime : null,
            EmployeeId = a.EmployeeId
        });

        return attendanceViewModels;
    }

    public async Task<bool> UpdateAsync(int id)
    {
        var attendance = new Attendance()
        {
            Id = id,
            CheckOutTime = DateTime.Now,
            UpdatedAt = DateTime.Now,
        };

        _unitOfWork.GetRepository<Attendance>().SaveInclude(attendance,
            a => a.CheckOutTime,
           a => a.UpdatedAt);

        await _unitOfWork.SaveChangesAsync();

        return true;

    }
}
