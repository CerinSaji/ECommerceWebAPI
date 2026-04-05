public class PaginationParameters
{
    private const int MaxPageSize = 3;
    public int PageNumber { get; set; } = 1;

    private int _pageSize = 2;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }
}

public class ProductParameters : PaginationParameters
{
    public string? SearchTerm { get; set; }
    public string? CategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? SortBy { get; set;}
}