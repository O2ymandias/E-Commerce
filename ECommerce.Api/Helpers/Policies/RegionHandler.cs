using ECommerce.Api.Extensions;
using ECommerce.Core.Entities.IdentityModule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace ECommerce.Api.Helpers.Policies
{
	public class RegionHandler : AuthorizationHandler<RegionRequirement>
	{
		private readonly IServiceProvider _serviceProvider;

		public RegionHandler(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;

		}
		protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, RegionRequirement requirement)
		{
			using var scope = _serviceProvider.CreateScope();
			var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

			var user = await userManager.FindUserWithAddressIncludedAsync(context.User);
			if (user?.Address is not null && user.Address.Country.Equals(requirement.Country, StringComparison.OrdinalIgnoreCase))
				context.Succeed(requirement);
		}
	}
}
