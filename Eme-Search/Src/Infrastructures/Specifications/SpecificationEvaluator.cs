using Eme_Search.Common;
using Microsoft.EntityFrameworkCore;

namespace Eme_Search.Infrastructures.Specifications;

public class SpecificationEvaluator<T> where T : BaseEntity
{
    public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T>? specification)
    {
        var query = inputQuery;

        // modify the IQueryable using the specification's criteria expression
        if (specification?.Criteria != null)
        {
            query = query.Where(specification.Criteria);
        }

        // Includes all expression-based includes
        if (specification?.Includes != null)
        {
            query = specification?.Includes.Aggregate(query,
                (current, include) => current.Include(include));
        }

        // Include any string-based include statements
        if (specification?.IncludeStrings != null)
        {
            query = specification?.IncludeStrings.Aggregate(query,
                (current, include) => current.Include(include));
        }

        // Apply ordering if expressions are set
        if (specification?.OrderBy != null)
        {
            query = query.OrderBy(specification.OrderBy);
        }
        else if (specification?.OrderByDescending != null)
        {
            query = query.OrderByDescending(specification.OrderByDescending);
        }

        if (specification?.GroupBy != null)
        {
            query = query.GroupBy(specification.GroupBy).SelectMany(x => x);
        }

        // Apply paging if enabled
        if (specification?.IsPagingEnabled == true)
        {
            query = query.Skip(specification.Skip)
                .Take(specification.Take);
        }
        return query;
    }
}