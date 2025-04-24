using Core.Interfaces.Services;
using Core.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AttendancesController : ControllerBase
{
    private readonly IAttendanceService _attendanceService;
    private readonly IEmployeeService _employeeService;

    public AttendancesController(IAttendanceService attendanceService, IEmployeeService employeeService)
    {
        this._attendanceService = attendanceService;
        this._employeeService = employeeService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AttendanceViewModel>>> GetAllAttendances()
    {
        try
        {
            var attendanceViewModels = await _attendanceService.GetAll();
            return Ok(attendanceViewModels);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("check-in")]
    public async Task<ActionResult<AttendanceViewModel>> CheckIn()
    {
        try
        {
            var employeeId = GetEmployeeIdFromToken();
            var attendanceViewModel = await _attendanceService.CreateAsync(employeeId);
            return Ok(attendanceViewModel);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPatch("check-out/{id}")]
    public async Task<ActionResult<bool>> CheckOut([FromRoute] int id)
    {
        try
        {
            var employeeId = GetEmployeeIdFromToken();
            var result = await _attendanceService.UpdateAsync(employeeId, id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAttendance([FromRoute] int id)
    {
        try
        {
            var result = await _attendanceService.DeleteOneAsync(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


    private int GetEmployeeIdFromToken()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null)
            throw new UnauthorizedAccessException("Employee ID not found in token.");

        return int.Parse(claim.Value);
    }
}