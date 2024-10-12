using Microsoft.AspNetCore.Authorization;

namespace ECommerce.Api.Helpers.Policies
{
	public class RegionRequirement : IAuthorizationRequirement
	{
		public string Country { get; private set; }

		public RegionRequirement(string country)
		{
			Country = country;
		}
	}
}
