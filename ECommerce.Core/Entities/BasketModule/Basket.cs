namespace ECommerce.Core.Entities.BasketModule
{
	public class Basket
	{
		public Basket(string id)
		{
			Id = id;
			Items = new List<BasketItem>();
		}

		public string Id { get; set; }
		public List<BasketItem> Items { get; set; }

		public string? PaymentIntentId { get; set; }
		public string? ClientSecret { get; set; }
		public int? DeliveryMethodId { get; set; }
		public decimal? ShippingPrice { get; set; }
	}
}
