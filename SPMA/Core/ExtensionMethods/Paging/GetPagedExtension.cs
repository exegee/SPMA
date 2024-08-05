using System;
using System.Linq;

namespace SPMA.Core.ExtensionMethods.Paging
{
    public static class GetPagedExtension
    {
        public static PagedResult<T> GetPaged<T>(this IQueryable<T> query, int page, int pageSize) where T : class
        {
            var result = new PagedResult<T>();
            result.CurrentPage = page;
            result.PageSize = pageSize;
            result.RowCount = query.Count();

            var pageCount = (double)result.RowCount / pageSize;
            result.PageSize = (int)Math.Ceiling(pageCount);

            int skip = ((page - 1) * pageSize) + pageSize;

            result.Results = query.Skip(skip).Take(pageSize).ToList();

            return result;
        }
    }
}
