using ECommerce.Core.Entities.OrderModule.Utilities;

namespace ECommerce.Core.Entities.OrderModule
{
	public class OrderItem : BaseEntity
	{
		public ProductAsItem Product { get; set; }
		public decimal Price { get; set; }
		public int Quantity { get; set; }
	}
}
