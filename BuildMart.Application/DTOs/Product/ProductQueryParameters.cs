namespace BuildMart.Application.DTOs.Product;

/// <summary>
/// Binds directly from the query string, e.g.
/// /api/products?search=drill&amp;categoryId=1&amp;minPrice=10&amp;sortBy=price_asc&amp;page=1&amp;pageSize=12
/// </summary>
public class ProductQueryParameters
{
    public string? Search { get; set; }
    public int? CategoryId { get; set; }
    public string? Brand { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool? IsAvailable { get; set; }

    /// <summary>One of: price_asc, price_desc, newest, name_asc.</summary>
    public string? SortBy { get; set; }

    private int _page = 1;
    public int Page
    {
        get => _page;
        set => _page = value < 1 ? 1 : value;
    }

    private int _pageSize = 12;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value is < 1 or > 100 ? 12 : value;
    }
}
