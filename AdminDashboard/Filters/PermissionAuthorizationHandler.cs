using ECommerce.Core.Constants;
using Microsoft.AspNetCore.Authorization;

namespace AdminDashboard.Filters
{
	public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
	{
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
		{
			if (context.User is not null && context.User.Claims.Any(c => c.Type == Permissions.Type && c.Value.Equals(requirement.Permission, StringComparison.OrdinalIgnoreCase)))
				context.Succeed(requirement);

			return Task.CompletedTask;
		}
	}
}
