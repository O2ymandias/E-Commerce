using ECommerce.Api.Errors;
using ECommerce.Core.Constants;
using ECommerce.Core.Entities.BasketModule;
using ECommerce.Core.Entities.Helpers;
using ECommerce.Core.Entities.IdentityModule;
using ECommerce.Core.Entities.OrderModule;
using ECommerce.Core.Interfaces.Services.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace ECommerce.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Roles = Roles.BasicUser)]
	public class PaymentController : ControllerBase
	{
		private readonly IPaymentService _paymentService;
		private readonly IConfiguration _configuration;
		private readonly IOrderService _orderService;
		private readonly IEmailSender _emailSender;
		private readonly ISmsSender _smsSender;
		private readonly UserManager<ApplicationUser> _userManager;

		public PaymentController(IPaymentService paymentService,
			IConfiguration configuration,
			IOrderService orderService,
			IEmailSender emailSender,
			ISmsSender smsSender,
			UserManager<ApplicationUser> userManager)
		{
			_paymentService = paymentService;
			_configuration = configuration;
			_orderService = orderService;
			_emailSender = emailSender;
			_smsSender = smsSender;
			_userManager = userManager;
		}


		[ProducesResponseType(typeof(Basket), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
		[Authorize]
		[HttpGet("{basketId}")]
		public async Task<ActionResult<Basket>> CreateOrUpdatePaymentIntent(string basketId)
		{
			var basket = await _paymentService.CreateOrUpdatePaymentIntent(basketId);
			return basket is null
				? BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest, "Unable To Create PaymentIntent"))
				: Ok(basket);
		}


		[ApiExplorerSettings(IgnoreApi = true)]
		[HttpPost("/webhook")]
		[AllowAnonymous]
		public async Task<IActionResult> WebHook()
		{
			var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
			var stripeEvent = EventUtility.ConstructEvent(json,
				Request.Headers["Stripe-Signature"], _configuration["Stripe:WebHookSecret"]);

			var PaymentIntent = stripeEvent.Data.Object as PaymentIntent;

			if (PaymentIntent is null)
				return BadRequest();

			Order? order;
			switch (stripeEvent.Type)
			{
				case Events.PaymentIntentSucceeded:
					order = await _orderService.UpdateOrderStatus(PaymentIntent.Id, true);
					if (order is not null)
					{
						await _emailSender.SendEmailAsync(new Email()
						{
							To = order.BuyerEmail,
							Subject = "Payment Status",
							Body = "Your Payment Is Succeeded, Please Wait For Customer Service Call To Confirm Shipping"
						});

						var user = await _userManager.FindByEmailAsync(order.BuyerEmail);
						if (user?.PhoneNumber is not null)
							await _smsSender.SendSmsAsync(new Sms()
							{
								To = user.PhoneNumber,
								Message = "Your Payment Is Succeeded, Please Wait For Customer Service Call To Confirm Shipping"
							});
					}
					break;

				case Events.PaymentIntentPaymentFailed:
					order = await _orderService.UpdateOrderStatus(PaymentIntent.Id, true);
					if (order is not null)
					{
						await _emailSender.SendEmailAsync(new Email()
						{
							To = order.BuyerEmail,
							Subject = "Payment Status",
							Body = "Your Payment Is Declined"
						});

						var user = await _userManager.FindByEmailAsync(order.BuyerEmail);
						if (user?.PhoneNumber is not null)
							await _smsSender.SendSmsAsync(new Sms()
							{
								To = user.PhoneNumber,
								Message = "Your Payment Is Declined"
							});
					}
					break;
			}
			return Ok();
		}
	}

}
