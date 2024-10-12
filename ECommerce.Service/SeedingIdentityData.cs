using ECommerce.Core.Constants;
using ECommerce.Core.Entities.IdentityModule;
using ECommerce.Core.Interfaces.Services.Contract;
using ECommerce.Service.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace ECommerce.Service
{
	public class SeedingIdentityData : ISeedingIdentityData
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly ILogger<SeedingIdentityData> _logger;
		private readonly SuperAdmin _superAdmin;

		public SeedingIdentityData(UserManager<ApplicationUser> userManager,
			RoleManager<IdentityRole> roleManager,
			ILogger<SeedingIdentityData> logger,
			IOptions<SuperAdmin> options)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_logger = logger;
			_superAdmin = options.Value;
		}
		public async Task SeedRolesAsync()
		{
			if (!_roleManager.Roles.Any())
			{
				IdentityRole[] roles = [
					new IdentityRole() { Name = Roles.SuperAdmin },
					new IdentityRole() { Name = Roles.Admin },
					new IdentityRole() { Name = Roles.BasicUser }
					];

				try
				{
					foreach (var role in roles)
					{
						var result = await _roleManager.CreateAsync(role);
						LogIdentityResult(result, $"Role: '{role.Name}' has been created successfully.");
					}
					await AssignAllPermissionsToSuperAdminRoleAsync();
				}
				catch (Exception ex)
				{
					_logger.LogError("Error: {Message}", ex.Message);
					throw;
				}
			}
		}

		public async Task SeedUsersAsync()
		{
			if (!_userManager.Users.Any())
			{
				var superAdmin = new ApplicationUser()
				{
					DisplayName = _superAdmin.DisplayName,
					UserName = _superAdmin.UserName,
					Email = _superAdmin.Email,
					PhoneNumber = _superAdmin.PhoneNumber,
					EmailConfirmed = true
				};
				var creationalResult = await _userManager.CreateAsync(superAdmin, _superAdmin.Password);
				LogIdentityResult(creationalResult, $"User: '{superAdmin.UserName}' has been created successfully.");

				string[] allRoles = [Roles.SuperAdmin, Roles.Admin, Roles.BasicUser];
				var assigningResult = await _userManager.AddToRolesAsync(superAdmin, allRoles);
				LogIdentityResult(assigningResult, $"User: '{superAdmin.UserName}' has been assigned to all roles: '{string.Join(", ", allRoles)}' successfully.");
			}
		}

		private void LogIdentityResult(IdentityResult identityResult, string successMessage)
		{
			if (identityResult.Succeeded)
				_logger.LogInformation("Message: {SuccessMessage}", successMessage);

			else
				foreach (var error in identityResult.Errors)
					_logger.LogError("Error: {Description}", error.Description);
		}


		private async Task AssignAllPermissionsToSuperAdminRoleAsync()
		{
			var superAdminRole = await _roleManager.FindByNameAsync(Roles.SuperAdmin);
			if (superAdminRole is not null)
			{
				var superAdminRoleClaims = await _roleManager.GetClaimsAsync(superAdminRole);
				var allPermissions = Permissions.GenerateAllPermissions();
				foreach (var permission in allPermissions)
				{
					if (!superAdminRoleClaims.Any(c => c.Type == Permissions.Type && c.Value == permission))
					{
						var result = await _roleManager.AddClaimAsync(superAdminRole, new Claim(Permissions.Type, permission));

						LogIdentityResult(result, $"Claim: '{permission}' has been assigned to role: '{superAdminRole.Name}' successfully.");
					}
				}
			}
		}
	}
}
