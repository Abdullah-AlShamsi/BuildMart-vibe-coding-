using BuildMart.Application.DTOs.Cart;
using BuildMart.Application.Interfaces;
using BuildMart.Domain.Entities;
using BuildMart.Domain.Exceptions;
using BuildMart.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BuildMart.Infrastructure.Services;

public class CartService : ICartService
{
    private readonly ApplicationDbContext _context;

    public CartService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CartDto> GetCartAsync(string userId)
    {
        var cart = await GetOrCreateCartAsync(userId);
        return MapToDto(cart);
    }

    public async Task<CartDto> AddItemAsync(string userId, AddCartItemDto dto)
    {
        var product = await _context.Products.FindAsync(dto.ProductId)
            ?? throw new NotFoundException("Product", dto.ProductId);

        if (!product.IsAvailable)
        {
            throw new BadRequestException($"'{product.Name}' is currently unavailable.");
        }

        var cart = await GetOrCreateCartAsync(userId);

        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);
        var requestedQuantity = (existingItem?.Quantity ?? 0) + dto.Quantity;

        if (requestedQuantity > product.StockQuantity)
        {
            throw new BadRequestException(
                $"Only {product.StockQuantity} unit(s) of '{product.Name}' are in stock.");
        }

        if (existingItem is not null)
        {
            existingItem.Quantity = requestedQuantity;
        }
        else
        {
            cart.Items.Add(new CartItem
            {
                CartId = cart.Id,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity
            });
        }

        await _context.SaveChangesAsync();

        return await GetCartAsync(userId);
    }

    public async Task<CartDto> UpdateItemAsync(string userId, int cartItemId, UpdateCartItemDto dto)
    {
        var cart = await GetOrCreateCartAsync(userId);

        var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId)
            ?? throw new NotFoundException("Cart item", cartItemId);

        if (dto.Quantity > item.Product.StockQuantity)
        {
            throw new BadRequestException(
                $"Only {item.Product.StockQuantity} unit(s) of '{item.Product.Name}' are in stock.");
        }

        item.Quantity = dto.Quantity;
        await _context.SaveChangesAsync();

        return await GetCartAsync(userId);
    }

    public async Task<CartDto> RemoveItemAsync(string userId, int cartItemId)
    {
        var cart = await GetOrCreateCartAsync(userId);

        var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId)
            ?? throw new NotFoundException("Cart item", cartItemId);

        cart.Items.Remove(item);
        _context.CartItems.Remove(item);
        await _context.SaveChangesAsync();

        return await GetCartAsync(userId);
    }

    public async Task ClearCartAsync(string userId)
    {
        var cart = await GetOrCreateCartAsync(userId);
        _context.CartItems.RemoveRange(cart.Items);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Every user gets exactly one cart, created lazily on first use.
    /// </summary>
    private async Task<Cart> GetOrCreateCartAsync(string userId)
    {
        var cart = await _context.Carts
            .Include(c => c.Items).ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart is not null)
        {
            return cart;
        }

        cart = new Cart { UserId = userId };
        _context.Carts.Add(cart);
        await _context.SaveChangesAsync();

        return cart;
    }

    private static CartDto MapToDto(Cart cart)
    {
        var items = cart.Items.Select(i => new CartItemDto
        {
            Id = i.Id,
            ProductId = i.ProductId,
            ProductName = i.Product.Name,
            ProductImageUrl = i.Product.ImageUrl,
            UnitPrice = i.Product.EffectivePrice,
            Quantity = i.Quantity,
            Subtotal = i.Product.EffectivePrice * i.Quantity,
            AvailableStock = i.Product.StockQuantity
        }).ToList();

        var rawSubtotal = items.Sum(i => i.Quantity * i.UnitPrice);
        var discount = cart.Items.Sum(i => (i.Product.Price - i.Product.EffectivePrice) * i.Quantity);

        return new CartDto
        {
            Id = cart.Id,
            Items = items,
            Subtotal = cart.Items.Sum(i => i.Product.Price * i.Quantity),
            Discount = discount,
            Total = rawSubtotal,
            TotalItems = items.Sum(i => i.Quantity)
        };
    }
}
