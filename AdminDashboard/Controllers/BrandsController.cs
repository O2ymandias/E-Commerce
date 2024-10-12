using AdminDashboard.Models.Application;
using ECommerce.Core.Constants;
using ECommerce.Core.Entities.ProductModule;
using ECommerce.Core.Interfaces;
using ECommerce.Core.Specifications.BrandSpecifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminDashboard.Controllers
{
	public class BrandsController : Controller
	{
		#region Dependencies

		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogger<BrandsController> _logger;

		public BrandsController(IUnitOfWork unitOfWork, ILogger<BrandsController> logger)
		{
			_unitOfWork = unitOfWork;
			_logger = logger;
		}

		#endregion

		#region Index

		[HttpGet]
		[Authorize(Policy = Permissions.Brands.View)]
		public async Task<IActionResult> Index(string? searchInput)
		{
			var specs = new BrandsSpecifications(searchInput?.Trim());
			var categories = await _unitOfWork.Repository<Brand>().GetAllWithSpecsAsync(specs);
			return View(categories);
		}

		#endregion

		#region Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Policy = Permissions.Brands.Create)]
		public async Task<IActionResult> Create(CreateOrEditBrandVM input)
		{
			var brandRepo = _unitOfWork.Repository<Brand>();

			if (!ModelState.IsValid)
				return View(nameof(Index), await brandRepo.GetAllAsync());


			var isBrandExisted = (await brandRepo.GetAllAsync())
				.Any(c => c.Name.Equals(input.BrandName, StringComparison.OrdinalIgnoreCase));

			if (isBrandExisted)
			{
				ModelState.AddModelError(nameof(CreateOrEditBrandVM.BrandName), "Brand already exists.");
				return View(nameof(Index), await brandRepo.GetAllAsync());
			}


			var brand = new Brand() { Name = input.BrandName };
			brandRepo.Add(brand);
			var numberOfRowsAffected = await _unitOfWork.CompleteAsync();
			if (numberOfRowsAffected == 0)
			{
				_logger.LogError("Error: Unexpected error trying to add this new brand '{Name}'.", brand.Name);
				ModelState.AddModelError(string.Empty, "Unexpected error trying to add this new brand");
				return View(nameof(Index), await brandRepo.GetAllAsync());
			}

			_logger.LogInformation("Message: Brand with id '{Id}' has been created successfully.", brand.Id);
			return RedirectToAction(nameof(Index));
		}

		#endregion

		#region Edit

		[HttpGet]
		[Authorize(Policy = Permissions.Brands.Create)]
		public async Task<IActionResult> Edit(int id)
		{
			var brand = await _unitOfWork.Repository<Brand>().GetAsync(id);
			if (brand is null)
				return BadRequest();

			return View(new CreateOrEditBrandVM() { BrandName = brand.Name });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Policy = Permissions.Brands.Create)]
		public async Task<IActionResult> Edit([FromRoute] int id, CreateOrEditBrandVM input)
		{
			if (!ModelState.IsValid)
				return View(input);
			var brandRepo = _unitOfWork.Repository<Brand>();

			var brand = await brandRepo.GetAsync(id);
			if (brand is null)
				return BadRequest();

			var isBrandExisted = (await brandRepo.GetAllAsync())
				.Any(c => c.Name.Equals(input.BrandName, StringComparison.OrdinalIgnoreCase));

			if (isBrandExisted)
			{
				ModelState.AddModelError(nameof(CreateOrEditBrandVM.BrandName), "Brand already exists.");
				return View(input);
			}

			brand.Name = input.BrandName;
			brandRepo.Update(brand);
			var numberOfRowAffected = await _unitOfWork.CompleteAsync();

			if (numberOfRowAffected == 0)
			{
				_logger.LogError("Error: Unexpected error trying to update this brand '{Id}'.", brand.Id);
				ModelState.AddModelError(string.Empty, "Unexpected error trying to update this brand.");
				return View(input);
			}

			_logger.LogInformation("Message: Brand with id '{Id}' has been updated successfully.", brand.Id);
			return RedirectToAction(nameof(Index));
		}

		#endregion
	}
}
