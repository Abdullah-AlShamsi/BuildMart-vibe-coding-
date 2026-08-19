using BuildMart.Application.DTOs.Common;
using BuildMart.Application.DTOs.Product;
using BuildMart.Application.Interfaces;
using BuildMart.Domain.Entities;
using BuildMart.Domain.Enums;
using BuildMart.Domain.Exceptions;
using BuildMart.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BuildMart.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;

    public ProductService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<ProductDto>> GetAllAsync(ProductQueryParameters query)
    {
        var products = _context.Products.AsNoTracking().Include(p => p.Category).AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            products = products.Where(p =>
                p.Name.ToLower().Contains(search) ||
                (p.Description != null && p.Description.ToLower().Contains(search)) ||
                p.SKU.ToLower().Contains(search));
        }

        if (query.CategoryId.HasValue)
        {
            products = products.Where(p => p.CategoryId == query.CategoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Brand))
        {
            products = products.Where(p => p.Brand != null && p.Brand.ToLower() == query.Brand.ToLower());
        }

        if (query.MinPrice.HasValue)
        {
            products = products.Where(p => p.Price >= query.MinPrice.Value);
        }

        if (query.MaxPrice.HasValue)
        {
            products = products.Where(p => p.Price <= query.MaxPrice.Value);
        }

        if (query.IsAvailable.HasValue)
        {
            products = products.Where(p => p.IsAvailable == query.IsAvailable.Value);
        }

        products = query.SortBy switch
        {
            "price_asc" => products.OrderBy(p => p.Price),
            "price_desc" => products.OrderByDescending(p => p.Price),
            "name_asc" => products.OrderBy(p => p.Name),
            "newest" => products.OrderByDescending(p => p.CreatedAt),
            _ => products.OrderByDescending(p => p.CreatedAt)
        };

        var totalCount = await products.CountAsync();

        var items = await products
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                DiscountPrice = p.DiscountPrice,
                EffectivePrice = p.DiscountPrice.HasValue && p.DiscountPrice.Value < p.Price ? p.DiscountPrice.Value : p.Price,
                StockQuantity = p.StockQuantity,
                SKU = p.SKU,
                ImageUrl = p.ImageUrl,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
                Brand = p.Brand,
                Unit = p.Unit.ToString(),
                Weight = p.Weight,
                IsAvailable = p.IsAvailable,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
            .ToListAsync();

        return new PagedResult<ProductDto>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<ProductDto> GetByIdAsync(int id)
    {
        var product = await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id)
            ?? throw new NotFoundException("Product", id);

        return MapToDto(product);
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId);
        if (!categoryExists)
        {
            throw new NotFoundException("Category", dto.CategoryId);
        }

        var skuExists = await _context.Products.AnyAsync(p => p.SKU == dto.SKU);
        if (skuExists)
        {
            throw new ConflictException($"A product with SKU '{dto.SKU}' already exists.");
        }

        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            DiscountPrice = dto.DiscountPrice,
            StockQuantity = dto.StockQuantity,
            SKU = dto.SKU,
            ImageUrl = dto.ImageUrl,
            CategoryId = dto.CategoryId,
            Brand = dto.Brand,
            Unit = ParseUnit(dto.Unit),
            Weight = dto.Weight,
            IsAvailable = dto.IsAvailable
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(product.Id);
    }

    public async Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto)
    {
        var product = await _context.Products.FindAsync(id)
            ?? throw new NotFoundException("Product", id);

        var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId);
        if (!categoryExists)
        {
            throw new NotFoundException("Category", dto.CategoryId);
        }

        var skuTaken = await _context.Products.AnyAsync(p => p.SKU == dto.SKU && p.Id != id);
        if (skuTaken)
        {
            throw new ConflictException($"A product with SKU '{dto.SKU}' already exists.");
        }

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.DiscountPrice = dto.DiscountPrice;
        product.StockQuantity = dto.StockQuantity;
        product.SKU = dto.SKU;
        product.ImageUrl = dto.ImageUrl;
        product.CategoryId = dto.CategoryId;
        product.Brand = dto.Brand;
        product.Unit = ParseUnit(dto.Unit);
        product.Weight = dto.Weight;
        product.IsAvailable = dto.IsAvailable;

        await _context.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id)
            ?? throw new NotFoundException("Product", id);

        var referencedInOrders = await _context.OrderItems.AnyAsync(oi => oi.ProductId == id);
        if (referencedInOrders)
        {
            // Preserve order history: soft-delete by marking unavailable instead of a hard delete.
            product.IsAvailable = false;
            product.StockQuantity = 0;
            await _context.SaveChangesAsync();
            return;
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }

    private static MeasurementUnit ParseUnit(string unit) =>
        Enum.TryParse<MeasurementUnit>(unit, true, out var parsed)
            ? parsed
            : throw new BadRequestException($"Invalid unit '{unit}'.");

    private static ProductDto MapToDto(Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Description = p.Description,
        Price = p.Price,
        DiscountPrice = p.DiscountPrice,
        EffectivePrice = p.EffectivePrice,
        StockQuantity = p.StockQuantity,
        SKU = p.SKU,
        ImageUrl = p.ImageUrl,
        CategoryId = p.CategoryId,
        CategoryName = p.Category != null ? p.Category.Name : string.Empty,
        Brand = p.Brand,
        Unit = p.Unit.ToString(),
        Weight = p.Weight,
        IsAvailable = p.IsAvailable,
        CreatedAt = p.CreatedAt,
        UpdatedAt = p.UpdatedAt
    };
}
