using AutoMapper;
using ECommerce.Api.Dtos;
using ECommerce.Api.Dtos.AccountDtos;
using ECommerce.Api.Dtos.BasketDtos;
using ECommerce.Api.Dtos.OrderDtos;
using ECommerce.Api.Dtos.ProductDtos;
using ECommerce.Core.Entities.BasketModule;
using ECommerce.Core.Entities.IdentityModule.Utilities;
using ECommerce.Core.Entities.OrderModule;
using ECommerce.Core.Entities.ProductModule;

namespace ECommerce.Api.Helpers
{
	public class MappingProfiles : Profile
	{
		public MappingProfiles()
		{
			#region Product

			CreateMap<Product, ProductToReturnDto>()
				.ForMember(dest => dest.Brand, opts => opts.MapFrom(src => src.Brand.Name))
				.ForMember(dest => dest.Category, opts => opts.MapFrom(src => src.Category.Name))
				.ForMember(dest => dest.PictureUrl, opts => opts.MapFrom<ProductPictureUrlResolver>());

			#endregion

			#region Basket

			CreateMap<BasketDto, Basket>().ReverseMap();
			CreateMap<BasketItemDto, BasketItem>().ReverseMap();

			#endregion

			#region Account

			CreateMap<Core.Entities.IdentityModule.Address, AddressDto>()
				.ReverseMap();

			CreateMap<AuthModel, UserDto>();

			#endregion

			#region Order

			CreateMap<AddressDto, Core.Entities.OrderModule.Utilities.Address>();

			CreateMap<Order, OrderToReturnDto>()
				.ForMember(dest => dest.DeliveryMethod, opts => opts.MapFrom(src => src.DeliveryMethod.ShortName))
				.ForMember(dest => dest.DeliveryMethodCost, opts => opts.MapFrom(src => src.DeliveryMethod.Cost));

			CreateMap<OrderItem, OrderItemDto>()
				.ForMember(dest => dest.ProductId, opts => opts.MapFrom(src => src.Product.Id))
				.ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.Product.Name))
				.ForMember(dest => dest.PictureUrl, opts => opts.MapFrom<OrderItemPictureUrlResolver>());

			#endregion
		}
	}
}
