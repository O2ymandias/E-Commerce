using ECommerce.Core.Entities;
using ECommerce.Core.Specifications;

namespace ECommerce.Core.Interfaces.Repositories.Contract
{
	public interface IGenericRepository<T> where T : BaseEntity
	{
		#region Without Specifications

		Task<IReadOnlyList<T>> GetAllAsync();
		Task<T?> GetAsync(int id);

		#endregion

		#region With Specifications

		Task<IReadOnlyList<T>> GetAllWithSpecsAsync(ISpecifications<T> specifications);
		Task<T?> GetWithSpecsAsync(ISpecifications<T> specifications);
		Task<int> GetCountWithSpecsAsync(ISpecifications<T> specifications);

		#endregion

		void Add(T entity);
		void Update(T entity);
		void Delete(T entity);
	}
}
