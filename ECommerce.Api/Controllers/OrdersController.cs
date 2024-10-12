using AutoMapper;
using ECommerce.Api.Dtos.OrderDtos;
using ECommerce.Api.Errors;
using ECommerce.Core.Constants;
using ECommerce.Core.Entities.OrderModule;
using ECommerce.Core.Entities.OrderModule.Utilities;
using ECommerce.Core.Interfaces.Services.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Roles = Roles.BasicUser, Policy = "Region")]
	public class OrdersController : ControllerBase
	{
		private readonly IOrderService _orderService;
		private readonly IMapper _mapper;

		public OrdersController(IOrderService orderService,
			IMapper mapper)
		{
			_orderService = orderService;
			_mapper = mapper;
		}

		[ProducesResponseType(typeof(OrderToReturnDto), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
		[HttpPost]
		public async Task<ActionResult<OrderToReturnDto>> CreateOrder(OrderToCreateDto inputOrder)
		{
			var buyerEmail = User.FindFirstValue(ClaimTypes.Email);

			var buyerAddress = _mapper.Map<Address>(inputOrder.ShippingAddress);

			var order = await _orderService.CreateOrderAsync(buyerEmail!, inputOrder.BasketId, inputOrder.DeliveryMethodId, buyerAddress);

			return order is not null ?
				Ok(_mapper.Map<OrderToReturnDto>(order)) :
				BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest,
					"Unable To Create The Order, Make Sure All Requirements Are Satisfied"));
		}

		[HttpGet]
		public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrdersForCurrentUser()
		{
			var userEmail = User.FindFirstValue(ClaimTypes.Email);
			var orders = await _orderService.GetOrdersForUserAsync(userEmail!);
			return Ok(_mapper.Map<IReadOnlyList<OrderToReturnDto>>(orders));
		}

		[ProducesResponseType(typeof(OrderToReturnDto), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
		[HttpGet("{orderId}")]
		public async Task<ActionResult<OrderToReturnDto>> GetOrderForCurrentUser(int orderId)
		{
			var userEmail = User.FindFirstValue(ClaimTypes.Email);
			var order = await _orderService.GetOrderByIdForUserAsync(userEmail!, orderId);
			return order is not null ?
				Ok(_mapper.Map<OrderToReturnDto>(order)) :
				NotFound(new ApiErrorResponse(StatusCodes.Status404NotFound,
					$"There Is No Such Order With Id: {orderId} For User: {userEmail}"));
		}

		[HttpGet("DeliveryMethods")]
		public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
			=> Ok(await _orderService.GetDeliveryMethodsAsync());
	}
}
