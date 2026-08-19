using BuildMart.Application.DTOs.Category;
using BuildMart.Application.Interfaces;
using BuildMart.Domain.Entities;
using BuildMart.Domain.Exceptions;
using BuildMart.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BuildMart.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _context;

    public CategoryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CategoryDto>> GetAllAsync()
    {
        return await _context.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ImageUrl = c.ImageUrl,
                ProductCount = c.Products.Count
            })
            .ToListAsync();
    }

    public async Task<CategoryDto> GetByIdAsync(int id)
    {
        var category = await _context.Categories
            .AsNoTracking()
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id)
            ?? throw new NotFoundException("Category", id);

        return MapToDto(category);
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
    {
        var nameExists = await _context.Categories.AnyAsync(c => c.Name == dto.Name);
        if (nameExists)
        {
            throw new ConflictException($"A category named '{dto.Name}' already exists.");
        }

        var category = new Category
        {
            Name = dto.Name,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return MapToDto(category);
    }

    public async Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto dto)
    {
        var category = await _context.Categories.FindAsync(id)
            ?? throw new NotFoundException("Category", id);

        var nameTaken = await _context.Categories.AnyAsync(c => c.Name == dto.Name && c.Id != id);
        if (nameTaken)
        {
            throw new ConflictException($"A category named '{dto.Name}' already exists.");
        }

        category.Name = dto.Name;
        category.Description = dto.Description;
        category.ImageUrl = dto.ImageUrl;

        await _context.SaveChangesAsync();

        return MapToDto(category);
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id)
            ?? throw new NotFoundException("Category", id);

        if (category.Products.Any())
        {
            throw new ConflictException("Cannot delete a category that still has products. Reassign or delete its products first.");
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
    }

    private static CategoryDto MapToDto(Category category) => new()
    {
        Id = category.Id,
        Name = category.Name,
        Description = category.Description,
        ImageUrl = category.ImageUrl,
        ProductCount = category.Products?.Count ?? 0
    };
}
