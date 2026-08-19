using BuildMart.Application.DTOs.Category;
using BuildMart.Application.DTOs.Common;
using BuildMart.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuildMart.API.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<CategoryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _categoryService.GetAllAsync();
        return Ok(ApiResponse<List<CategoryDto>>.SuccessResponse(categories));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        return Ok(ApiResponse<CategoryDto>.SuccessResponse(category));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(CreateCategoryDto dto)
    {
        var category = await _categoryService.CreateAsync(dto);
        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<CategoryDto>.SuccessResponse(category, "Category created."));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(int id, UpdateCategoryDto dto)
    {
        var category = await _categoryService.UpdateAsync(id, dto);
        return Ok(ApiResponse<CategoryDto>.SuccessResponse(category, "Category updated."));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id)
    {
        await _categoryService.DeleteAsync(id);
        return NoContent();
    }
}
