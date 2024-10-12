using ECommerce.Core.Constants;
using ECommerce.Core.Entities.ProductModule;
using ECommerce.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminDashboard.Controllers.ApiControllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Policy = Permissions.Categories.Delete)]
	public class CategoriesController : ControllerBase
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogger<CategoriesController> _logger;

		public CategoriesController(IUnitOfWork unitOfWork, ILogger<CategoriesController> logger)
		{
			_unitOfWork = unitOfWork;
			_logger = logger;
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete([FromRoute] int id)
		{
			var categoryRepo = _unitOfWork.Repository<Category>();
			var category = await categoryRepo.GetAsync(id);
			if (category is null)
				return NotFound();

			categoryRepo.Delete(category);
			var numberOfRowAffected = await _unitOfWork.CompleteAsync();
			if (numberOfRowAffected == 0)
			{
				_logger.LogError("Error: Unexpected error while deleting the category with id '{Id}'.", id);
				return BadRequest();
			}
			_logger.LogInformation("Message: Category with id '{Id}' has been deleted successfully.", id);
			return Ok();
		}
	}
}
