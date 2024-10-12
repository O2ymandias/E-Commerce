using ECommerce.Core.Entities;
using System.Linq.Expressions;

namespace ECommerce.Core.Specifications
{
	public interface ISpecifications<T> where T : BaseEntity
	{
		Expression<Func<T, bool>>? Criteria { get; set; }
		List<Expression<Func<T, object>>> Includes { get; set; }
		Expression<Func<T, object>> SortAsc { get; set; }
		Expression<Func<T, object>> SortDesc { get; set; }
		public int Take { get; set; }
		public int Skip { get; set; }
		public bool IsPaginated { get; set; }
	}
}
