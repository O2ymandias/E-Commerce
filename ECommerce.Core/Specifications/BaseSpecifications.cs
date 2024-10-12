using ECommerce.Core.Entities;
using System.Linq.Expressions;

namespace ECommerce.Core.Specifications
{
	public class BaseSpecifications<T> : ISpecifications<T> where T : BaseEntity
	{
		public BaseSpecifications()
		{
		}

		public BaseSpecifications(Expression<Func<T, bool>> criteria)
		{
			Criteria = criteria;
		}

		public Expression<Func<T, bool>>? Criteria { get; set; }
		public List<Expression<Func<T, object>>> Includes { get; set; } = new List<Expression<Func<T, object>>>();
		public Expression<Func<T, object>> SortAsc { get; set; }
		public Expression<Func<T, object>> SortDesc { get; set; }
		public int Take { get; set; }
		public int Skip { get; set; }
		public bool IsPaginated { get; set; }

		private protected void ApplyPagination(int pageIndex, int pageSize)
		{
			IsPaginated = true;
			Take = pageSize;
			Skip = (pageIndex - 1) * pageSize;
		}
	}
}
