using ECommerce.Core.Entities;
using ECommerce.Core.Specifications;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Repository
{
	public static class SpecificationEvaluator<T> where T : BaseEntity
	{
		public static IQueryable<T> BuildQuery(IQueryable<T> inputQuery, ISpecifications<T> specifications)
		{
			var query = inputQuery;

			if (specifications.Criteria is not null)
				query = query.Where(specifications.Criteria);

			if (specifications.SortAsc is not null)
				query = query.OrderBy(specifications.SortAsc);

			else if (specifications.SortDesc is not null)
				query = query.OrderByDescending(specifications.SortDesc);

			if (specifications.IsPaginated)
				query = query.Skip(specifications.Skip).Take(specifications.Take);

			if (specifications.Includes.Count > 0)
				query = specifications.Includes.Aggregate(query, (currentQuery, nextInclude) => currentQuery.Include(nextInclude));

			return query;
		}

	}
}
