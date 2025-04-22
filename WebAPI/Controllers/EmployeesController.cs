using Core.Interfaces.Services;
using Core.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            var employeeViewModel = await _employeeService.CreateAsync(employeeCreateViewModel);
            return Ok(employeeViewModel);

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
    public async Task<ActionResult<bool>> UpdateEmployeeSignature([FromRoute] int id, [FromBody] string signature)
    {
        try
        {
            var result = await _employeeService.UpdateSignatureAsync(id, signature);
            return Ok(result);

        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
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
}