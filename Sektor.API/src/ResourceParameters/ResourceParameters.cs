namespace Sektor.API.src.ResourceParameters;

public abstract class ResourceParameters
{
    const int maxPageSize = 20;
    public string? SearchQuery { get; set; }
    public int PageNumber { get; set; } = 0;
    private int _pageSize = 10;
    public int PageSize 
    {
        get => _pageSize;
        set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
    }
}