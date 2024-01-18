using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class ProductExtensions
{
    public static IQueryable<Product> Sort(this IQueryable<Product> query, string orderBy)
    {
        if (string.IsNullOrEmpty(orderBy))
        {
            orderBy = "name";
        }
        query = orderBy switch
        {
            "price" => query.OrderBy(x => x.Price),
            "priceDesc" => query.OrderByDescending(x => x.Price),
            _ => query.OrderBy(x => x.Name)
        };
        return query;
    }

    public static IQueryable<Product> Search(this IQueryable<Product> query, string searchTerm)
    {
        if (string.IsNullOrEmpty(searchTerm))
        {
            return query;
        }
        var lowerCaseSearchTerm = searchTerm.Trim().ToLower();

        return query.Where(p => p.Name.ToLower().Contains(lowerCaseSearchTerm)); //Client-side (in Memory) Evaluation
        //return query.Where(x => EF.Functions.Like(x.Name.ToLower(), $"%{lowerCaseSearchTerm}%"));
    }

    public static IQueryable<Product> Filter(this IQueryable<Product> query, string brands, string types)
    {
        var barndList = new List<string>();
        var typeList = new List<string>();
        if (!string.IsNullOrEmpty(brands))
        {
            barndList.AddRange(brands.ToLower().Split(','));
        }
        if (!string.IsNullOrEmpty(types))
        {
            typeList.AddRange(types.ToLower().Split(','));
        }

        query = query.Where(x => barndList.Count == 0 || barndList.Contains(x.Brand.ToLower()));
        query = query.Where(x => typeList.Count == 0 || typeList.Contains(x.Type.ToLower()));
        return query;
    }
}
