namespace BlogApi.Application.Common;

public class PagedResponse<T>
{
    public PagedResponse(T data, int total, int page, int limit)
    {
        Data = data;
        Total = total;
        Page = page;
        Limit = limit;
        TotalPages = limit > 0 ? (int)Math.Ceiling((double)total / limit) : 0;
    }

    public T Data { get; set; }
    public int Total { get; set; }
    public int Page { get; set; }
    public int Limit { get; set; }
    public int TotalPages { get; set; }
}
