using BuildMart.API.Common;
using BuildMart.Application.DTOs.Common;
using BuildMart.Application.DTOs.Order;
using BuildMart.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuildMart.API.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>Creates an order from the current user's cart (checkout).</summary>
    [HttpPost]
    [Authorize(Roles = "Customer")]
    [ProducesResponseType(typeof(ApiResponse<OrderDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(CreateOrderDto dto)
    {
        var order = await _orderService.CreateOrderAsync(User.GetUserId(), dto);
        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<OrderDto>.SuccessResponse(order, "Order placed successfully."));
    }

    /// <summary>Customers see their own orders; Admins see every order.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<OrderDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var isAdmin = User.IsInRole("Admin");
        var orders = isAdmin
            ? await _orderService.GetAllOrdersAsync()
            : await _orderService.GetUserOrdersAsync(User.GetUserId());

        return Ok(ApiResponse<List<OrderDto>>.SuccessResponse(orders));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<OrderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id, User.GetUserId(), User.IsInRole("Admin"));
        return Ok(ApiResponse<OrderDto>.SuccessResponse(order));
    }

    /// <summary>Admin only: transitions an order through its status lifecycle.</summary>
    [HttpPut("{id:int}/status")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<OrderDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateStatus(int id, UpdateOrderStatusDto dto)
    {
        var order = await _orderService.UpdateOrderStatusAsync(id, dto);
        return Ok(ApiResponse<OrderDto>.SuccessResponse(order, "Order status updated."));
    }
}
