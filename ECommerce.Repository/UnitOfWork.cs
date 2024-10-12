using ECommerce.Core.Entities;
using ECommerce.Core.Interfaces;
using ECommerce.Core.Interfaces.Repositories.Contract;
using ECommerce.Repository._Data;
using System.Collections;

namespace ECommerce.Repository
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly StoreDbContext _dbContext;
		private readonly Hashtable _repositories;

		public UnitOfWork(StoreDbContext context)
		{
			_dbContext = context;
			_repositories = new Hashtable();
		}

		public IGenericRepository<T> Repository<T>() where T : BaseEntity
		{
			string key = typeof(T).Name;
			if (!_repositories.ContainsKey(key))
			{
				var repo = new GenericRepository<T>(_dbContext);
				_repositories.Add(key, repo);
			}
			return (_repositories[key] as IGenericRepository<T>)!;
		}


		public async Task<int> CompleteAsync()
			=> await _dbContext.SaveChangesAsync();

		public async ValueTask DisposeAsync()
			=> await _dbContext.DisposeAsync();
	}
}
