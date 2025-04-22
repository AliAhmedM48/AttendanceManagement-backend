using Core.Interfaces.Services;
using Core.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AttendancesController : ControllerBase
{
    private readonly IAttendanceService _attendanceService;

    public AttendancesController(IAttendanceService attendanceService)
    {
        this._attendanceService = attendanceService;
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

    [HttpPost("CheckIn")]
    public async Task<ActionResult<AttendanceViewModel>> CheckIn([FromBody] AttendanceCreateViewModel attendanceCreateViewModel)
    {
        try
        {
            var attendanceViewModel = await _attendanceService.CreateAsync(attendanceCreateViewModel);
            return Ok(attendanceViewModel);

        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}/CheckOut")]
    public async Task<ActionResult<bool>> CheckOut([FromRoute] int id)
    {
        try
        {
            var result = await _attendanceService.UpdateAsync(id);
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
}