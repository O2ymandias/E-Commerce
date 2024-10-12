using ECommerce.Core.Entities.IdentityModule;

namespace ECommerce.Core.Interfaces.Services.Contract
{
	public interface ITokenService
	{
		Task<string> GenerateAccessTokenAsync(ApplicationUser appUser);
		RefreshToken GenerateRefreshToken(int expirationDays);
	}
}
