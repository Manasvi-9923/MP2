using EmployeeManagement.Api.DTOs.Common;
using EmployeeManagement.Api.DTOs.Dashboard;
using EmployeeManagement.Api.DTOs.Employees;
using EmployeeManagement.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController(EmployeeService employeeService) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Admin,Viewer")]
    public async Task<ActionResult<PagedResult<EmployeeResponse>>> GetAll([FromQuery] EmployeeQueryParameters query)
    {
        return Ok(await employeeService.GetAllAsync(query));
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,Viewer")]
    public async Task<ActionResult<EmployeeResponse>> GetById(int id)
    {
        var employee = await employeeService.GetByIdAsync(id);
        if (employee is null)
        {
            return NotFound(new { message = "Employee not found." });
        }

        return Ok(employee);
    }

    [HttpGet("dashboard")]
    [Authorize(Roles = "Admin,Viewer")]
    public async Task<ActionResult<DashboardSummaryResponse>> GetDashboard()
    {
        return Ok(await employeeService.GetDashboardAsync());
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<EmployeeResponse>> Create(EmployeeRequest request)
    {
        var result = await employeeService.CreateAsync(request);
        return ToActionResult(result, nameof(GetById), result.Data is null ? null : new { id = result.Data.Id });
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<EmployeeResponse>> Update(int id, EmployeeRequest request)
    {
        return ToActionResult(await employeeService.UpdateAsync(id, request));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await employeeService.DeleteAsync(id);
        return StatusCode(result.StatusCode, new { success = result.Success, message = result.Message });
    }

    private ActionResult<EmployeeResponse> ToActionResult(ServiceResult<EmployeeResponse> result, string? createdAction = null, object? routeValues = null)
    {
        if (!result.Success)
        {
            return StatusCode(result.StatusCode, new { success = false, message = result.Message });
        }

        if (result.StatusCode == StatusCodes.Status201Created && createdAction is not null && routeValues is not null)
        {
            return CreatedAtAction(createdAction, routeValues, result.Data);
        }

        return StatusCode(result.StatusCode, result.Data);
    }
}
