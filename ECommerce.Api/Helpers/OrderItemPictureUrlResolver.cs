using AutoMapper;
using ECommerce.Api.Dtos.OrderDtos;
using ECommerce.Core.Entities.OrderModule;

namespace ECommerce.Api.Helpers
{
	public class OrderItemPictureUrlResolver : IValueResolver<OrderItem, OrderItemDto, string>
	{
		private readonly IConfiguration _configuration;

		public OrderItemPictureUrlResolver(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public string Resolve(OrderItem source, OrderItemDto destination, string destMember, ResolutionContext context)
			=>
				!string.IsNullOrEmpty(source.Product.PictureUrl) ?
				$"{_configuration["BaseUrl"]}/{source.Product.PictureUrl}" :
				string.Empty;
	}
}
