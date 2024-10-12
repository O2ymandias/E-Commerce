using AdminDashboard.Models.Auth;
using AdminDashboard.Models.Auth.UserViewModels;
using AutoMapper;
using ECommerce.Core.Constants;
using ECommerce.Core.Entities.IdentityModule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminDashboard.Controllers
{
	public class UsersController : Controller
	{
		#region Dependencies

		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IMapper _mapper;
		private readonly IWebHostEnvironment _env;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly ILogger<UsersController> _logger;

		public UsersController(UserManager<ApplicationUser> userManager,
			RoleManager<IdentityRole> roleManager,
			IMapper mapper,
			IWebHostEnvironment env,
			SignInManager<ApplicationUser> signInManager,
			ILogger<UsersController> logger)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_mapper = mapper;
			_env = env;
			_signInManager = signInManager;
			_logger = logger;
		}

		#endregion

		#region Index

		[Authorize(Policy = Permissions.Users.View)]
		public async Task<IActionResult> Index(string? searchInput)
		{
			var model = new List<UserVM>();

			if (string.IsNullOrEmpty(searchInput))
			{
				var users = await _userManager.Users.ToListAsync();
				foreach (var user in users)
				{
					var userVM = _mapper.Map<UserVM>(user);
					userVM.Roles = await _userManager.GetRolesAsync(user);
					model.Add(userVM);
				}
			}
			else
			{
				var user = await _userManager.FindByNameAsync(searchInput);
				if (user is not null)
				{
					var userVM = _mapper.Map<UserVM>(user);
					userVM.Roles = await _userManager.GetRolesAsync(user);
					model.Add(userVM);
				}
			}
			return View(model);
		}

		#endregion

		#region Create 

		[HttpGet]
		[Authorize(Policy = Permissions.Users.Create)]
		public async Task<IActionResult> Create()
		{
			var roles = await _roleManager.Roles
				.Select(role => new CheckBoxVM() { DisplayValue = role.Name ?? string.Empty })
				.ToListAsync();

			return View(new CreateUserVM() { Roles = roles });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Policy = Permissions.Users.Create)]
		public async Task<IActionResult> Create(CreateUserVM model)
		{
			if (!ModelState.IsValid)
				return View(model);


			var uniqueEmail = await _userManager.FindByEmailAsync(model.Email);
			if (uniqueEmail is not null)
			{
				ModelState.AddModelError(nameof(CreateUserVM.Email), "Email already taken.");
				return View(model);
			}

			var uniqueUserName = await _userManager.FindByNameAsync(model.UserName);
			if (uniqueUserName is not null)
			{
				ModelState.AddModelError(nameof(CreateUserVM.Email), "UserName already taken.");
				return View(model);
			}

			var appUser = new ApplicationUser()
			{
				UserName = model.UserName,
				DisplayName = model.DisplayName,
				Email = model.Email,
				PhoneNumber = model.PhoneNumber
			};

			var creationalResult = await _userManager.CreateAsync(appUser, model.Password);
			if (!creationalResult.Succeeded)
			{
				ModelState.AddModelError(string.Empty, "Error occurred while creating this user");
				foreach (var error in creationalResult.Errors)
					_logger.LogError("Error: {Description}", error.Description);

				return View(model);
			}
			_logger.LogInformation("Message: User '{UserName}' has been created successfully.", appUser.UserName);


			var selectedRoles = model.Roles.Where(role => role.IsSelected).Select(role => role.DisplayValue);
			var addToRolesResult = await _userManager.AddToRolesAsync(appUser, selectedRoles);
			if (!addToRolesResult.Succeeded)
			{
				ModelState.AddModelError(string.Empty, $"User '{appUser.UserName}' is created but failed to assign him to roles {string.Join(", ", selectedRoles)}");

				foreach (var error in creationalResult.Errors)
					_logger.LogError("Error: {Description}", error.Description);

				return View(model);
			}

			return RedirectToAction(nameof(Index));
		}

		#endregion

		#region Edit
		[HttpGet]
		[Authorize(Policy = Permissions.Users.Edit)]
		public async Task<IActionResult> Edit(string? id)
		{
			if (string.IsNullOrEmpty(id))
				return BadRequest();

			var user = await _userManager.FindByIdAsync(id);
			if (user is null)
				return BadRequest();

			var allRoles = await _roleManager.Roles.ToListAsync();

			var model = new EditUserVM()
			{
				UserId = user.Id,
				UserName = user.UserName ?? string.Empty,
				Email = user.Email ?? string.Empty,
				DisplayName = user.DisplayName,
				PhoneNumber = user.PhoneNumber
			};

			foreach (var role in allRoles)
			{
				model.Roles.Add(new CheckBoxVM()
				{
					DisplayValue = role.Name ?? string.Empty,
					IsSelected = await _userManager.IsInRoleAsync(user, role.Name!)
				});
			}
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Policy = Permissions.Users.Edit)]
		public async Task<IActionResult> Edit([FromRoute] string? id, EditUserVM model)
		{
			if (id != model.UserId)
				return BadRequest();

			if (!ModelState.IsValid)
				return View(model);

			var appUser = await _userManager.FindByIdAsync(model.UserId);
			if (appUser is null)
				return NotFound();

			#region Update User Basic Info (Email, UserName, DisplayName, PhoneNumber)

			if (!model.Email.Equals(appUser.Email, StringComparison.OrdinalIgnoreCase))
			{
				var isTakenEmail = await _userManager.FindByEmailAsync(model.Email);
				if (isTakenEmail is not null)
				{
					ModelState.AddModelError(nameof(EditUserVM.Email), "Email already taken.");
					return View(model);
				}
				appUser.Email = model.Email;
			}

			if (!model.UserName.Equals(appUser.UserName, StringComparison.OrdinalIgnoreCase))
			{
				var isTakenUserName = await _userManager.FindByNameAsync(model.UserName);
				if (isTakenUserName is not null)
				{
					ModelState.AddModelError(nameof(EditUserVM.UserName), "UserName already taken.");
					return View(model);
				}
				appUser.UserName = model.UserName;
			}

			appUser.DisplayName = model.DisplayName;
			appUser.PhoneNumber = model.PhoneNumber;

			var updateResult = await _userManager.UpdateAsync(appUser);
			if (!updateResult.Succeeded)
			{
				foreach (var error in updateResult.Errors)
					_logger.LogError("Error: {Description}", error.Description);

				ModelState.AddModelError(string.Empty, $"Unable to update basic info for user '{appUser.UserName}'.");
				return View(model);
			}

			_logger.LogInformation("Message: Basic info for user '{UserName}' have been updated successfully.", appUser.UserName);

			#endregion

			#region Update User Roles

			var userRoles = await _userManager.GetRolesAsync(appUser);
			var removeRolesResult = await _userManager.RemoveFromRolesAsync(appUser, userRoles);
			if (!removeRolesResult.Succeeded)
			{
				foreach (var error in removeRolesResult.Errors)
					_logger.LogError("Message: {Description}", error.Description);

				ModelState.AddModelError(nameof(EditUserVM.Roles), "Unexcpected error while updating roles.");
				return View(model);
			}

			var selectedRoles = model.Roles
				.Where(role => role.IsSelected)
				.Select(role => role.DisplayValue);
			foreach (var role in selectedRoles)
			{
				if (await _roleManager.RoleExistsAsync(role))
				{
					var addToRoleResult = await _userManager.AddToRoleAsync(appUser, role);

					if (!addToRoleResult.Succeeded)
					{
						foreach (var error in addToRoleResult.Errors)
							_logger.LogError("Message: {Description}", error.Description);

						ModelState.AddModelError(nameof(EditUserVM.Roles), "Unexcpected error while updating roles");
						return View(model);
					}
				}
			}
			_logger.LogInformation("Message: Roles have been updated successfully.");

			#endregion

			return RedirectToAction(nameof(Index));
		}
		#endregion
	}
}
