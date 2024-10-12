using AutoMapper;
using ECommerce.Api.Dtos.ProductDtos;
using ECommerce.Core.Entities.ProductModule;

namespace ECommerce.Api.Helpers
{
	public class ProductPictureUrlResolver : IValueResolver<Product, ProductToReturnDto, string>
	{
		private readonly IConfiguration _configuration;

		public ProductPictureUrlResolver(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		public string Resolve(Product source, ProductToReturnDto destination, string destMember, ResolutionContext context)
			=>
				!string.IsNullOrEmpty(source.PictureUrl) ?
				$"{_configuration["BaseUrl"]}/{source.PictureUrl}" :
				string.Empty;
	}
}
