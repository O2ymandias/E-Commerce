using ECommerce.Core.Entities.IdentityModule;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ECommerce.Api.Extensions
{
	public static class UserManagerExtensions
	{
		public static async Task<ApplicationUser?> FindUserWithAddressIncludedAsync(this UserManager<ApplicationUser> userManager,
			ClaimsPrincipal User)
		{
			var email = User.FindFirstValue(ClaimTypes.Email)?.ToUpper();

			var appUser = await userManager.Users
				.Include(user => user.Address)
				.FirstOrDefaultAsync(user => user.NormalizedEmail == email);

			return appUser;
		}
	}
}
