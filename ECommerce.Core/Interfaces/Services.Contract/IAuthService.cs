using ECommerce.Core.Entities.IdentityModule.Utilities;
using System.Security.Claims;

namespace ECommerce.Core.Interfaces.Services.Contract
{
	public interface IAuthService
	{
		Task<AuthModel> RegisterUserAsync(string displayName, string email, string phoneNumber, string password);
		Task<AuthModel> LoginUserAsync(string email, string password);
		Task<AuthModel> RefreshTokenAsync(string refreshToken);
		Task<bool> RevokeTokenAsync(string refreshToken);
		Task<AuthModel> GetCurrentUserAsync(ClaimsPrincipal user);
	}
}
