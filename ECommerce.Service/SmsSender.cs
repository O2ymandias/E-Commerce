using ECommerce.Core.Entities.Helpers;
using ECommerce.Core.Interfaces.Services.Contract;
using ECommerce.Service.Helpers;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace ECommerce.Service
{
	public class SmsSender : ISmsSender
	{
		private readonly TwilioSettings _twilioSettings;
		public SmsSender(IOptions<TwilioSettings> twilioSettingsOptions)
		{
			_twilioSettings = twilioSettingsOptions.Value;
		}
		public async Task SendSmsAsync(Sms sms)
		{
			TwilioClient.Init(_twilioSettings.AccountSID, _twilioSettings.AuthToken);
			var result = await MessageResource.CreateAsync
				(
				from: new PhoneNumber(_twilioSettings.TwilioPhoneNumber),
				to: new PhoneNumber(sms.To),
				body: sms.Message
				);
		}
	}
}
