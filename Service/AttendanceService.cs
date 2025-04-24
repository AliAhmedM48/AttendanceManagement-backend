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
    public async Task<AttendanceViewModel> CreateAsync(int employeeId)
    {
        var now = DateTime.Now;

        var today = now.Date;
        var repository = _unitOfWork.GetRepository<Attendance>();

        var hasCheckedInToday = await repository
            .AnyAsync(a => a.EmployeeId == employeeId && a.CheckInTime.Date == today);

        if (hasCheckedInToday)
            throw new InvalidOperationException("You have already checked in today.");

        var allowedStartTime = today.AddHours(7.5);
        var allowedEndTime = today.AddHours(9);

        if (now < allowedStartTime || now > allowedEndTime)
            throw new InvalidOperationException("Check-in is allowed only between 7:30 AM and 9:00 AM.");

        var attendance = new Attendance()
        {
            EmployeeId = employeeId,
            CheckInTime = now,
            CreatedAt = now,
        };

        await repository.AddAsync(attendance);
        await _unitOfWork.SaveChangesAsync();

        return new AttendanceViewModel
        {
            Id = attendance.Id,
            CheckInTime = attendance.CheckInTime,
            EmployeeId = attendance.EmployeeId,
            CheckOutTime = null
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

    public async Task<AttendanceViewModel> UpdateAsync(int employeeId, int attendanceId)
    {
        var now = DateTime.Now;
        var repository = _unitOfWork.GetRepository<Attendance>();

        var attendance = await repository.GetByIdAsync(attendanceId);

        if (attendance == null)
            throw new KeyNotFoundException($"Attendance with ID {attendanceId} not found.");

        if (attendance.EmployeeId != employeeId)
            throw new UnauthorizedAccessException("You are not authorized to update this attendance record.");

        attendance.CheckOutTime = now;
        attendance.TotalWorkedHours = attendance.CheckOutTime.HasValue ? (attendance.CheckOutTime.Value - attendance.CheckInTime).TotalHours : 0;
        attendance.UpdatedAt = now;

        repository.SaveInclude(attendance, e => e.CheckOutTime, e => e.TotalWorkedHours, e => e.UpdatedAt);
        await _unitOfWork.SaveChangesAsync();

        return new AttendanceViewModel()
        {
            Id = attendance.Id,
            EmployeeId = attendance.EmployeeId,
            CheckInTime = attendance.CheckInTime,
            CheckOutTime = attendance.CheckOutTime,
            TotalWorkedHours = attendance.TotalWorkedHours
        };
    }

}
