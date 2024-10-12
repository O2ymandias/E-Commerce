using ECommerce.Core.Entities.Helpers;

namespace ECommerce.Core.Interfaces.Services.Contract
{
	public interface IEmailSender
	{
		Task SendEmailAsync(Email email);
	}
}
