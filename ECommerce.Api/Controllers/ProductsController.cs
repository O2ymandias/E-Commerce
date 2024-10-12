using AutoMapper;
using ECommerce.Api.Dtos;
using ECommerce.Api.Dtos.ProductDtos;
using ECommerce.Api.Errors;
using ECommerce.Api.Filters;
using ECommerce.Core.Constants;
using ECommerce.Core.Entities.ProductModule;
using ECommerce.Core.Interfaces.Services.Contract;
using ECommerce.Core.Specifications.ProductSpecifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Roles = Roles.BasicUser)]
	public class ProductsController : ControllerBase
	{
		private readonly IProductService _productService;
		private readonly IMapper _mapper;

		public ProductsController(IProductService productService,
			IMapper mapper)
		{
			_productService = productService;
			_mapper = mapper;
		}

		[HttpGet]
		[CacheResponse(5)]
		public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetAll([FromQuery] ProductsSpecificationsParams specParams)
		{
			var products = await _productService.GetAllProductsAsync(specParams);
			var count = await _productService.GetCountOfSpecificProductsAsync(specParams);
			var result = new Pagination<ProductToReturnDto>()
			{
				PageIndex = specParams.PageIndex,
				PageSize = specParams.PageSize,
				Count = count,
				Data = _mapper.Map<IReadOnlyList<ProductToReturnDto>>(products)
			};
			return Ok(result);
		}

		[ProducesResponseType(typeof(ProductToReturnDto), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
		[HttpGet("{id}")]
		public async Task<ActionResult<ProductToReturnDto>> Get(int id)
		{
			var product = await _productService.GetProductByIdAsync(id);
			return product is not null ?
				Ok(_mapper.Map<ProductToReturnDto>(product)) :
				NotFound(new ApiErrorResponse(StatusCodes.Status404NotFound, $"Product With Id: {id} Is Not Found"));
		}

		[HttpGet("Brands")]
		public async Task<ActionResult<IReadOnlyList<Brand>>> GetBrands()
			=> Ok(await _productService.GetAllBrandsAsync());

		[HttpGet("Categories")]
		public async Task<ActionResult<IReadOnlyList<Category>>> GetCategories()
			=> Ok(await _productService.GetAllCategoriesAsync());
	}
}
