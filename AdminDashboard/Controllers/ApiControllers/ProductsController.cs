using ECommerce.Core.Constants;
using ECommerce.Core.Entities.ProductModule;
using ECommerce.Core.Interfaces;
using ECommerce.Core.Interfaces.Services.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminDashboard.Controllers.ApiControllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Policy = Permissions.Products.Delete)]
	public class ProductsController : ControllerBase
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IFileManager _fileManager;
		private readonly ILogger<ProductsController> _logger;

		public ProductsController(IUnitOfWork unitOfWork, IFileManager fileManager, ILogger<ProductsController> logger)
		{
			_unitOfWork = unitOfWork;
			_fileManager = fileManager;
			_logger = logger;
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete([FromRoute] int id)
		{

			var product = await _unitOfWork.Repository<Product>().GetAsync(id);
			if (product is null)
				return BadRequest();

			_unitOfWork.Repository<Product>().Delete(product);
			var numberOfRowsAffected = await _unitOfWork.CompleteAsync();

			if (numberOfRowsAffected == 0)
			{
				_logger.LogError("Error: Unexpected error while deleting the product with id '{Id}'.", id);
				return BadRequest();
			}

			_fileManager.DeleteFile(product.PictureUrl);
			_logger.LogInformation("Message: Product with id '{Id}' has been deleted successfully.", id);
			return Ok();
		}
	}
}
