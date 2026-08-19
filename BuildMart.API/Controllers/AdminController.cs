using BuildMart.Application.DTOs.Admin;
using BuildMart.Application.DTOs.Common;
using BuildMart.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuildMart.API.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    private readonly ICustomerService _customerService;

    public AdminController(IDashboardService dashboardService, ICustomerService customerService)
    {
        _dashboardService = dashboardService;
        _customerService = customerService;
    }

    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(ApiResponse<DashboardStatsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboard()
    {
        var stats = await _dashboardService.GetStatsAsync();
        return Ok(ApiResponse<DashboardStatsDto>.SuccessResponse(stats));
    }

    [HttpGet("customers")]
    public async Task<IActionResult> GetCustomers()
    {
        var customers = await _customerService.GetAllCustomersAsync();
        return Ok(ApiResponse<object>.SuccessResponse(customers));
    }

    [HttpGet("customers/{id}/orders")]
    public async Task<IActionResult> GetCustomerOrders(string id)
    {
        var orders = await _customerService.GetCustomerOrdersAsync(id);
        return Ok(ApiResponse<object>.SuccessResponse(orders));
    }
}
