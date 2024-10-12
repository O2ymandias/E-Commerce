using ECommerce.Core.Entities;
using ECommerce.Core.Interfaces.Repositories.Contract;

namespace ECommerce.Core.Interfaces
{
	public interface IUnitOfWork : IAsyncDisposable
	{
		IGenericRepository<T> Repository<T>() where T : BaseEntity;
		Task<int> CompleteAsync();
	}
}
