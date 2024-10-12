using AdminDashboard.Models.Application;
using AutoMapper;
using ECommerce.Core.Constants;
using ECommerce.Core.Entities.ProductModule;
using ECommerce.Core.Interfaces;
using ECommerce.Core.Interfaces.Services.Contract;
using ECommerce.Core.Specifications.ProductSpecifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminDashboard.Controllers
{
	public class ProductsController : Controller
	{
		#region Dependencies

		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly IFileManager _fileManager;
		private readonly ILogger<ProductsController> _logger;

		public ProductsController(IUnitOfWork unitOfWork,
			IMapper mapper,
			IFileManager fileManager,
			ILogger<ProductsController> logger)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_fileManager = fileManager;
			_logger = logger;
		}

		#endregion

		#region Index 

		[Authorize(Policy = Permissions.Products.View)]
		[HttpGet]
		public async Task<IActionResult> Index(string? searchInput)
		{
			var specs = new ProductsSpecifications(searchInput);
			var products = await _unitOfWork.Repository<Product>().GetAllWithSpecsAsync(specs);
			var model = _mapper.Map<IReadOnlyList<ProductVM>>(products);
			return View(model);
		}

		#endregion

		#region Create 

		[Authorize(Policy = Permissions.Products.Create)]
		[HttpGet]
		public IActionResult Create() => View();

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Policy = Permissions.Products.Create)]
		public async Task<IActionResult> Create(CreateOrEditProductVM input)
		{
			if (!ModelState.IsValid)
				return View(input);

			if (input.Picture is null)
			{
				ModelState.AddModelError(nameof(CreateOrEditProductVM.Picture), "The picture field is required.");
				return View(input);
			}

			input.PictureUrl = await _fileManager.UploadFileAsync(input.Picture, "Images/Products");

			var product = _mapper.Map<Product>(input);

			_unitOfWork.Repository<Product>().Add(product);
			var numberOfRowsAffected = await _unitOfWork.CompleteAsync();
			if (numberOfRowsAffected == 0)
			{
				_logger.LogError("Error: Unexpected error trying to add this new product '{Name}'.", product.Name);
				ModelState.AddModelError(string.Empty, "Unexpected error trying to add this new product.");
				return View(input);
			}
			_logger.LogInformation("Message: Product with id '{Id}' has been created successfully.", product.Id);
			return RedirectToAction(nameof(Index));
		}

		#endregion

		#region Edit

		[Authorize(Policy = Permissions.Products.Edit)]
		[HttpGet]
		public async Task<IActionResult> Edit(int id)
		{
			var specs = new ProductsSpecifications(id);
			var product = await _unitOfWork.Repository<Product>().GetWithSpecsAsync(specs);

			if (product is null)
				return BadRequest();

			return View(_mapper.Map<CreateOrEditProductVM>(product));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Policy = Permissions.Products.Edit)]
		public async Task<IActionResult> Edit([FromRoute] int id, CreateOrEditProductVM input)
		{
			if (!ModelState.IsValid)
				return View(input);

			var productRepo = _unitOfWork.Repository<Product>();

			var product = await productRepo.GetAsync(id);
			if (product is null)
				return BadRequest();

			product.Name = input.Name;
			product.Description = input.Description;
			product.Price = input.Price;
			product.BrandId = input.BrandId;
			product.CategoryId = input.CategoryId;
			product.NormalizedName = input.NormalizedName;

			if (input.Picture is not null)
			{
				_fileManager.DeleteFile(product.PictureUrl);
				product.PictureUrl = await _fileManager.UploadFileAsync(input.Picture, "Images/Products");
			}

			productRepo.Update(product);
			var numberOfRowsAffected = await _unitOfWork.CompleteAsync();
			if (numberOfRowsAffected == 0)
			{
				_logger.LogError("Error: Unexpected error trying to update this product '{Id}'.", product.Id);
				ModelState.AddModelError(string.Empty, "Unexpected error trying to update this product.");
				return View(input);
			}
			_logger.LogInformation("Message: Product with id '{Id}' has been updated successfully.", product.Id);
			return RedirectToAction(nameof(Index));
		}

		#endregion
	}
}
