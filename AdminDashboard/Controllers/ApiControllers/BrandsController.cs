using ECommerce.Core.Constants;
using ECommerce.Core.Entities.ProductModule;
using ECommerce.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminDashboard.Controllers.ApiControllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Policy = Permissions.Brands.Delete)]
	public class BrandsController : ControllerBase
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogger<BrandsController> _logger;

		public BrandsController(IUnitOfWork unitOfWork, ILogger<BrandsController> logger)
		{
			_unitOfWork = unitOfWork;
			_logger = logger;
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete([FromRoute] int id)
		{
			var brandRepo = _unitOfWork.Repository<Brand>();
			var brand = await brandRepo.GetAsync(id);
			if (brand is null)
				return NotFound();

			brandRepo.Delete(brand);
			var numberOfRowAffected = await _unitOfWork.CompleteAsync();
			if (numberOfRowAffected == 0)
			{
				_logger.LogError("Error: Unexpected error while deleting the brand with id '{Id}'.", id);
				return BadRequest();
			}
			_logger.LogInformation("Message: Brand with id '{Id}' has been deleted successfully.", id);
			return Ok();
		}
	}
}
