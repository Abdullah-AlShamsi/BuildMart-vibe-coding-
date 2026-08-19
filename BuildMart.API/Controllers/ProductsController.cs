using BuildMart.Application.DTOs.Common;
using BuildMart.Application.DTOs.Product;
using BuildMart.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuildMart.API.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Search, filter, sort and paginate the catalog.
    /// Example: /api/products?search=drill&amp;categoryId=1&amp;minPrice=10&amp;maxPrice=100&amp;sortBy=price_asc&amp;page=1&amp;pageSize=12
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ProductDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] ProductQueryParameters query)
    {
        var result = await _productService.GetAllAsync(query);
        return Ok(ApiResponse<PagedResult<ProductDto>>.SuccessResponse(result));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        return Ok(ApiResponse<ProductDto>.SuccessResponse(product));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(CreateProductDto dto)
    {
        var product = await _productService.CreateAsync(dto);
        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<ProductDto>.SuccessResponse(product, "Product created."));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(int id, UpdateProductDto dto)
    {
        var product = await _productService.UpdateAsync(id, dto);
        return Ok(ApiResponse<ProductDto>.SuccessResponse(product, "Product updated."));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id)
    {
        await _productService.DeleteAsync(id);
        return NoContent();
    }
}
