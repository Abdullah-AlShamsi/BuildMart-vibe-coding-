using BuildMart.Application.DTOs.Common;
using BuildMart.Application.DTOs.Product;

namespace BuildMart.Application.Interfaces;

public interface IProductService
{
    Task<PagedResult<ProductDto>> GetAllAsync(ProductQueryParameters query);
    Task<ProductDto> GetByIdAsync(int id);
    Task<ProductDto> CreateAsync(CreateProductDto dto);
    Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto);
    Task DeleteAsync(int id);
}
