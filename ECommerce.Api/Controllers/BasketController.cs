using AutoMapper;
using ECommerce.Api.Dtos.BasketDtos;
using ECommerce.Api.Errors;
using ECommerce.Core.Entities.BasketModule;
using ECommerce.Core.Interfaces.Repositories.Contract;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BasketController : ControllerBase
	{
		private readonly IBasketRepository _basketRepo;
		private readonly IMapper _mapper;

		public BasketController(IBasketRepository basketRepo,
			IMapper mapper)
		{
			_basketRepo = basketRepo;
			_mapper = mapper;
		}
		[ProducesResponseType(typeof(Basket), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
		[HttpPost]
		public async Task<ActionResult<Basket>> CreateOrUpdateBasket(BasketDto inputBasket)
		{
			var basket = _mapper.Map<Basket>(inputBasket);
			var createdOrUpdatedBasket = await _basketRepo.CreateOrUpdateBasketAsync(basket);
			return createdOrUpdatedBasket is not null
				? Ok(createdOrUpdatedBasket)
				: BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest, "An Error Occurred While Creating Or Updating Basket"));
		}

		[HttpGet]
		public async Task<ActionResult<Basket>> GetBasket(string id)
		{
			var basket = await _basketRepo.GetBasketAsync(id);
			if (basket is not null)
				return Ok(basket);

			else
			{
				var newBasket = new Basket(id);
				await _basketRepo.CreateOrUpdateBasketAsync(newBasket);
				return Ok(newBasket);
			}
		}

		[HttpDelete]
		public async Task<bool> DeleteBasket(string id)
			=> await _basketRepo.DeleteBasketAsync(id);
	}
}
