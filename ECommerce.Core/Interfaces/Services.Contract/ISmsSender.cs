using ECommerce.Core.Entities.Helpers;

namespace ECommerce.Core.Interfaces.Services.Contract
{
	public interface ISmsSender
	{
		Task SendSmsAsync(Sms sms);
	}
}
