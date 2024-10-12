using ECommerce.Core.Entities;
using ECommerce.Core.Interfaces.Repositories.Contract;
using ECommerce.Core.Specifications;
using ECommerce.Repository._Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Repository
{
	public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
	{
		private readonly StoreDbContext _context;

		public GenericRepository(StoreDbContext context)
		{
			_context = context;
		}

		#region Without Specification

		public async Task<IReadOnlyList<T>> GetAllAsync()
			=> await _context.Set<T>().AsNoTracking().ToListAsync();
		public async Task<T?> GetAsync(int id)
			=> await _context.Set<T>().FindAsync(id);

		#endregion

		#region With Specification

		public async Task<IReadOnlyList<T>> GetAllWithSpecsAsync(ISpecifications<T> specifications)
			=> await ApplySpecifications(specifications).AsNoTracking().ToListAsync();

		public async Task<T?> GetWithSpecsAsync(ISpecifications<T> specifications)
			=> await ApplySpecifications(specifications).FirstOrDefaultAsync();

		public async Task<int> GetCountWithSpecsAsync(ISpecifications<T> specifications)
			=> await ApplySpecifications(specifications).CountAsync();

		private IQueryable<T> ApplySpecifications(ISpecifications<T> specifications)
			=> SpecificationEvaluator<T>.BuildQuery(_context.Set<T>(), specifications);


		#endregion

		public void Add(T entity)
			=> _context.Set<T>().Add(entity);
		public void Delete(T entity)
			=> _context.Set<T>().Remove(entity);
		public void Update(T entity)
			=> _context.Set<T>().Update(entity);


	}
}
