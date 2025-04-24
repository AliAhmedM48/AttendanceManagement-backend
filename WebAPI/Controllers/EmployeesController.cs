using Core.Interfaces.Services;
using Core.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        this._employeeService = employeeService;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<EmployeeViewModel>>> GetAllEmployees()
    {
        try
        {
            var employeeViewModels = await _employeeService.GetAll();
            return Ok(employeeViewModels);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeViewModel>> CreateNewEmployee([FromBody] EmployeeCreateViewModel employeeCreateViewModel)
    {
        try
        {
            var employeeId = await _employeeService.CreateAsync(employeeCreateViewModel);
            return Created(string.Empty, new { employeeId });

        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<bool>> UpdateEmployee([FromRoute] int id, [FromBody] EmployeeUpdateViewModel employeeUpdateViewModel)
    {
        try
        {
            var result = await _employeeService.UpdateAsync(id, employeeUpdateViewModel);
            return Ok(result);

        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPatch("{id}/signature")]
    public async Task<IActionResult> UpdateEmployeeSignature([FromRoute] int id, [FromForm] IFormFile signature)
    {
        Console.WriteLine("Log: xxxxxxxxxxxxxxxxxx=>>");
        try
        {
            if (signature == null || signature.Length == 0)
                return BadRequest("No signature file uploaded.");

            var result = await _employeeService.UpdateSignatureAsync(id, signature);
            if (result == null)
                return NotFound("Employee not found.");

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("signatures/{fileName}")]
    public async Task<IActionResult> GetSignature([FromRoute] string fileName)
    {
        try
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "signatures", fileName);
            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found.");

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, "image/jpeg");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("me/attendances")]
    public async Task<ActionResult<IEnumerable<AttendanceViewModel>>> GetEmployeeAttendance()
    {
        try
        {
            var employeeId = GetEmployeeIdFromToken();

            var attendanceRecords = await _employeeService.GetAttendanceByEmployeeId(employeeId);

            if (attendanceRecords == null || !attendanceRecords.Any())
                return NotFound("No attendance records found for this employee.");

            return Ok(attendanceRecords);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("me/profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            var employeeId = GetEmployeeIdFromToken();

            var employeeViewModel = await _employeeService.GetProfileDataByEmployeeId(employeeId);
            return Ok(employeeViewModel);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("Invalid or expired token.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteEmployee([FromRoute] int id)
    {
        try
        {
            var result = await _employeeService.DeleteOneAsync(id);
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