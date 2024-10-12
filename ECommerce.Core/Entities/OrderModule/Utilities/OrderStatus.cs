using System.Runtime.Serialization;

namespace ECommerce.Core.Entities.OrderModule.Utilities
{
	public enum OrderStatus
	{
		[EnumMember(Value = "Pending")]
		Pending,
		[EnumMember(Value = "Payment Received")]
		PaymentReceived,
		[EnumMember(Value = "Payment Failed")]
		PaymentFailed
	}
}
