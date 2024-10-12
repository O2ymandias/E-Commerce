using ECommerce.Core.Entities.OrderModule;

namespace ECommerce.Core.Specifications.OrderSpecifications
{
	public class OrderSpecifications : BaseSpecifications<Order>
	{
		public OrderSpecifications(string userEmail)
			: base(order => order.BuyerEmail == userEmail)
		{
			Includes.Add(order => order.DeliveryMethod);
			Includes.Add(order => order.Items);

			SortDesc = order => order.OrderDate;
		}

		public OrderSpecifications(string userEmail, int orderId)
			: base(order =>
				 order.BuyerEmail == userEmail && order.Id == orderId)
		{
			Includes.Add(order => order.DeliveryMethod);
			Includes.Add(order => order.Items);
		}
	}
}
