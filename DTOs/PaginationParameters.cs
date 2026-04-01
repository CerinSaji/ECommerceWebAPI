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