using BuildMart.API.Common;
using BuildMart.Application.DTOs.Cart;
using BuildMart.Application.DTOs.Common;
using BuildMart.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuildMart.API.Controllers;

[ApiController]
[Route("api/cart")]
[Authorize(Roles = "Customer")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<CartDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCart()
    {
        var cart = await _cartService.GetCartAsync(User.GetUserId());
        return Ok(ApiResponse<CartDto>.SuccessResponse(cart));
    }

    [HttpPost("items")]
    [ProducesResponseType(typeof(ApiResponse<CartDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddItem(AddCartItemDto dto)
    {
        var cart = await _cartService.AddItemAsync(User.GetUserId(), dto);
        return Ok(ApiResponse<CartDto>.SuccessResponse(cart, "Item added to cart."));
    }

    [HttpPut("items/{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<CartDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateItem(int id, UpdateCartItemDto dto)
    {
        var cart = await _cartService.UpdateItemAsync(User.GetUserId(), id, dto);
        return Ok(ApiResponse<CartDto>.SuccessResponse(cart, "Cart updated."));
    }

    [HttpDelete("items/{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<CartDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveItem(int id)
    {
        var cart = await _cartService.RemoveItemAsync(User.GetUserId(), id);
        return Ok(ApiResponse<CartDto>.SuccessResponse(cart, "Item removed from cart."));
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ClearCart()
    {
        await _cartService.ClearCartAsync(User.GetUserId());
        return NoContent();
    }
}
