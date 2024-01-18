using Microsoft.EntityFrameworkCore;

namespace API.RequestHelpers;

public class PagedList<T> : List<T>
{
    public MetaData MetaData { get; set; }
    public PagedList(List<T> pageItems, int count, int pageNumber, int pageSize)
    {
        MetaData = new MetaData
        {
            TotalCount = count,
            PageSize = pageSize,
            CurrentPage = pageNumber,
            TotalPages = (int)Math.Ceiling(count / (double)pageSize)
        };
        AddRange(pageItems);
    }

    public static async Task<PagedList<T>> ToPageList(IQueryable<T> query,
        int pageNumber, int pageSize)
    {
        var count = await query.CountAsync();
        // var totalPages = (int)Math.Ceiling(count / (double)pageSize);
        // pageNumber = pageNumber > totalPages ? totalPages : pageNumber;
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedList<T>(items, count, pageNumber, pageSize);
    }
}
