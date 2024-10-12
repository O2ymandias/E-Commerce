using AdminDashboard.Models.Auth;
using AdminDashboard.Models.Auth.RoleViewModels;
using AutoMapper;
using ECommerce.Core.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AdminDashboard.Controllers
{
	public class RolesController : Controller
	{
		#region Dependencies

		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IMapper _mapper;
		private readonly IWebHostEnvironment _env;
		private readonly ILogger<RolesController> _logger;

		public RolesController(RoleManager<IdentityRole> roleManager,
			IMapper mapper,
			IWebHostEnvironment env,
			ILogger<RolesController> logger)
		{
			_roleManager = roleManager;
			_mapper = mapper;
			_env = env;
			_logger = logger;
		}
		#endregion

		#region Index

		[Authorize(Policy = Permissions.Roles.View)]
		public async Task<IActionResult> Index(string? searchInput)
		{
			var roles = Enumerable.Empty<IdentityRole>();

			if (string.IsNullOrEmpty(searchInput))
				roles = await _roleManager.Roles.ToListAsync();

			else
			{
				var role = await _roleManager.FindByNameAsync(searchInput);
				if (role is not null)
					roles = new List<IdentityRole> { role };
			}

			return View(_mapper.Map<IEnumerable<RoleVM>>(roles));
		}

		#endregion

		#region Create 

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Policy = Permissions.Roles.Create)]
		public async Task<IActionResult> Create(CreateRoleVM input)
		{
			var allRoles = _mapper.Map<IEnumerable<RoleVM>>(await _roleManager.Roles.ToListAsync());

			if (!ModelState.IsValid)
				return View(nameof(Index), allRoles);

			var isRoleExists = await _roleManager.RoleExistsAsync(input.RoleName.Trim());
			if (isRoleExists)
			{
				ModelState.AddModelError(nameof(input.RoleName), "Role already exists");
				return View(nameof(Index), allRoles);
			}

			var role = new IdentityRole() { Name = input.RoleName.Trim() };
			var result = await _roleManager.CreateAsync(role);

			if (!result.Succeeded)
			{
				foreach (var error in result.Errors)
					_logger.LogError("Error: {Description}", error.Description);

				ModelState.AddModelError(string.Empty, $"Unexpected error, this role '{input.RoleName}' can't be created!");

				return View(nameof(Index), allRoles);
			}
			else
			{
				_logger.LogInformation("Message: Role '{RoleName} has been created successfully'", input.RoleName);
				return RedirectToAction(nameof(Index));
			}
		}

		#endregion

		#region Details

		[Authorize(Policy = Permissions.Roles.View)]
		public async Task<IActionResult> Details([FromRoute] string id)
		{
			if (string.IsNullOrEmpty(id))
				return BadRequest();

			var role = await _roleManager.FindByIdAsync(id);
			if (role is null)
				return NotFound();

			var permissions = (await _roleManager.GetClaimsAsync(role))
				.Where(c => c.Type == Permissions.Type)
				.Select(p => p.Value)
				.ToList();

			var model = new DetailsRoleVM()
			{
				Id = role.Id,
				Name = role.Name ?? string.Empty,
				Permissions = permissions
			};

			return View(model);
		}

		#endregion

		#region Edit 

		[HttpGet]
		[Authorize(Policy = Permissions.Roles.Edit)]
		public async Task<IActionResult> Edit([FromRoute] string id)
		{
			if (string.IsNullOrEmpty(id))
				return BadRequest();

			var role = await _roleManager.FindByIdAsync(id);
			if (role is null)
				return NotFound();

			var rolePermissions = (await _roleManager.GetClaimsAsync(role))
				.Where(c => c.Type == Permissions.Type)
				.Select(p => p.Value)
				.ToList();

			var allPermissions = Permissions
				.GenerateAllPermissions()
				.Select(permission => new CheckBoxVM()
				{
					DisplayValue = permission,
					IsSelected = rolePermissions.Contains(permission)
				})
				.ToList();

			return View(new EditRoleVM()
			{
				RoleId = role.Id,
				RoleName = role.Name ?? string.Empty,
				Permissions = allPermissions
			});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Policy = Permissions.Roles.Edit)]
		public async Task<IActionResult> Edit([FromRoute] string id, EditRoleVM input)
		{
			if (id != input.RoleId)
				return BadRequest();

			if (!ModelState.IsValid)
				return View(input);


			var role = (await _roleManager.FindByIdAsync(input.RoleId));
			if (role is null)
				return NotFound();

			role.Name = input.RoleName.Trim();

			var rolePermissions = (await _roleManager.GetClaimsAsync(role))
				.Where(roleClaim => roleClaim.Type == Permissions.Type);

			foreach (var permission in rolePermissions)
			{
				var removeClaimResult = await _roleManager.RemoveClaimAsync(role, permission);
				if (!removeClaimResult.Succeeded)
				{
					foreach (var error in removeClaimResult.Errors)
						_logger.LogError("Error: {Description}", error.Description);

					ModelState.AddModelError(string.Empty, "Unexpected error while updating permissions.");
					return View(input);
				}
			}

			var inputPermissions = input.Permissions
				.Where(permission => permission.IsSelected)
				.Select(permission => new Claim(Permissions.Type, permission.DisplayValue));

			foreach (var permission in inputPermissions)
			{
				var addClaimResult = await _roleManager.AddClaimAsync(role, permission);
				if (!addClaimResult.Succeeded)
				{
					foreach (var error in addClaimResult.Errors)
						_logger.LogError("Error: {Description}", error.Description);

					ModelState.AddModelError(string.Empty, "Unexpected error while updating permissions.");
					return View(input);
				}
			}

			var result = await _roleManager.UpdateAsync(role);
			if (!result.Succeeded)
			{
				foreach (var error in result.Errors)
					_logger.LogError("Error: {Description}", error.Description);

				ModelState.AddModelError(string.Empty, "Unexpected error while updating this role");
				return View(input);
			}

			return RedirectToAction(nameof(Index));
		}

		#endregion
	}
}
