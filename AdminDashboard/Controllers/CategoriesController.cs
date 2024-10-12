using AdminDashboard.Models.Application;
using ECommerce.Core.Constants;
using ECommerce.Core.Entities.ProductModule;
using ECommerce.Core.Interfaces;
using ECommerce.Core.Specifications.CategorySpecifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminDashboard.Controllers
{
	public class CategoriesController : Controller
	{
		#region Dependencies

		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogger<CategoriesController> _logger;

		public CategoriesController(IUnitOfWork unitOfWork, ILogger<CategoriesController> logger)
		{
			_unitOfWork = unitOfWork;
			_logger = logger;
		}

		#endregion

		#region Index

		[HttpGet]
		[Authorize(Policy = Permissions.Categories.View)]
		public async Task<IActionResult> Index(string? searchInput)
		{
			var specs = new CategoriesSpecifications(searchInput?.Trim());
			var categories = await _unitOfWork.Repository<Category>().GetAllWithSpecsAsync(specs);
			return View(categories);
		}

		#endregion

		#region Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Policy = Permissions.Categories.Create)]
		public async Task<IActionResult> Create(CreateOrEditCategoryVM input)
		{
			var categoryRepo = _unitOfWork.Repository<Category>();

			if (!ModelState.IsValid)
				return View(nameof(Index), await categoryRepo.GetAllAsync());


			var isCategoryExisted = (await categoryRepo.GetAllAsync())
				.Any(c => c.Name.Equals(input.CategoryName, StringComparison.OrdinalIgnoreCase));

			if (isCategoryExisted)
			{
				ModelState.AddModelError(nameof(CreateOrEditCategoryVM.CategoryName), "Category already exists.");
				return View(nameof(Index), await categoryRepo.GetAllAsync());
			}


			var category = new Category() { Name = input.CategoryName };
			categoryRepo.Add(category);
			var numberOfRowsAffected = await _unitOfWork.CompleteAsync();
			if (numberOfRowsAffected == 0)
			{
				_logger.LogError("Error: Unexpected error trying to add this new category '{Name}'.", category.Name);
				ModelState.AddModelError(string.Empty, "Unexpected error trying to add this new category");
				return View(nameof(Index), await categoryRepo.GetAllAsync());

			}

			_logger.LogInformation("Message: Category with id '{Id}' has been created successfully.", category.Id);
			return RedirectToAction(nameof(Index));
		}

		#endregion

		#region Edit

		[HttpGet]
		[Authorize(Policy = Permissions.Categories.Edit)]
		public async Task<IActionResult> Edit(int id)
		{
			var category = await _unitOfWork.Repository<Category>().GetAsync(id);
			if (category is null)
				return BadRequest();

			return View(new CreateOrEditCategoryVM() { CategoryName = category.Name });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Policy = Permissions.Categories.Edit)]
		public async Task<IActionResult> Edit([FromRoute] int id, CreateOrEditCategoryVM input)
		{
			if (!ModelState.IsValid)
				return View(input);
			var categoryRepo = _unitOfWork.Repository<Category>();

			var category = await categoryRepo.GetAsync(id);
			if (category is null)
				return BadRequest();

			var isCategoryExisted = (await categoryRepo.GetAllAsync())
				.Any(c => c.Name.Equals(input.CategoryName, StringComparison.OrdinalIgnoreCase));

			if (isCategoryExisted)
			{
				ModelState.AddModelError(nameof(CreateOrEditCategoryVM.CategoryName), "Category already exists.");
				return View(input);
			}

			category.Name = input.CategoryName;
			categoryRepo.Update(category);
			var numberOfRowAffected = await _unitOfWork.CompleteAsync();

			if (numberOfRowAffected == 0)
			{
				_logger.LogError("Error: Unexpected error trying to update this category '{Id}'.", category.Id);
				ModelState.AddModelError(string.Empty, "Unexpected error trying to update this category.");
				return View(input);
			}

			_logger.LogInformation("Message: Category with id '{Id}' has been updated successfully.", category.Id);
			return RedirectToAction(nameof(Index));
		}

		#endregion
	}
}
