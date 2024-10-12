using ECommerce.Core.Constants;
using ECommerce.Core.Entities.IdentityModule;
using ECommerce.Core.Entities.IdentityModule.Utilities;
using ECommerce.Core.Interfaces.Services.Contract;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ECommerce.Service
{
	public class AuthService : IAuthService
	{
		private const int _refreshTokenExpirationDays = 14;

		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ITokenService _tokenService;
		private readonly RoleManager<IdentityRole> _roleManager;

		public AuthService(UserManager<ApplicationUser> userManager,
			ITokenService tokenService,
			RoleManager<IdentityRole> roleManager)
		{
			_userManager = userManager;
			_tokenService = tokenService;
			_roleManager = roleManager;
		}

		public async Task<AuthModel> RegisterUserAsync(string displayName, string email, string phoneNumber, string password)
		{
			var checkedUser = await _userManager.FindByEmailAsync(email);
			if (checkedUser is not null)
				return new AuthModel() { Message = "Email Is Already Existed" };

			var refreshToken = _tokenService.GenerateRefreshToken(_refreshTokenExpirationDays);
			var appUser = new ApplicationUser()
			{
				UserName = email.Split('@')[0],
				DisplayName = displayName,
				Email = email,
				PhoneNumber = phoneNumber.StartsWith("+2") ? phoneNumber : $"+2{phoneNumber}",
				RefreshTokens = [refreshToken]
			};

			await _userManager.CreateAsync(appUser, password);
			await _userManager.AddToRoleAsync(appUser, Roles.BasicUser);

			return new AuthModel()
			{
				DisplayName = displayName,
				Email = email,
				Roles = new List<string>() { Roles.BasicUser },
				IsAuthenticated = true,
				Token = await _tokenService.GenerateAccessTokenAsync(appUser),
				RefreshToken = refreshToken.Token,
				RefreshTokenExpiration = refreshToken.ExpiresOn
			};
		}
		public async Task<AuthModel> LoginUserAsync(string email, string password)
		{
			var appUser = await _userManager.FindByEmailAsync(email);

			if (appUser is null || !await _userManager.CheckPasswordAsync(appUser, password))
				return new AuthModel() { Message = "Invalid Email Or Password!" };

			var jwtToken = await _tokenService.GenerateAccessTokenAsync(appUser);
			var roles = await _userManager.GetRolesAsync(appUser);

			var authModel = new AuthModel()
			{
				IsAuthenticated = true,
				DisplayName = appUser.DisplayName,
				Email = appUser.Email ?? string.Empty,
				Token = jwtToken,
				Roles = roles.ToList()
			};

			if (appUser.RefreshTokens.Any(t => t.IsActive))
			{
				var activeRefreshToken = appUser.RefreshTokens.Single(t => t.IsActive);

				authModel.RefreshToken = activeRefreshToken.Token;
				authModel.RefreshTokenExpiration = activeRefreshToken.ExpiresOn;
			}
			else
			{
				var newRefreshToken = _tokenService.GenerateRefreshToken(_refreshTokenExpirationDays);

				authModel.RefreshToken = newRefreshToken.Token;
				authModel.RefreshTokenExpiration = newRefreshToken.ExpiresOn;

				appUser.RefreshTokens.Add(newRefreshToken);
				await _userManager.UpdateAsync(appUser);
			}

			return authModel;
		}
		public async Task<AuthModel> RefreshTokenAsync(string refreshToken)
		{
			var user = await _userManager.Users.SingleOrDefaultAsync(user => user.RefreshTokens.Any(t => t.Token == refreshToken));

			if (user is null)
				return new AuthModel() { Message = "Invalid Token" };

			var userCurrentRefreshToken = user.RefreshTokens.Single(t => t.Token == refreshToken);

			if (!userCurrentRefreshToken.IsActive)
				return new AuthModel() { Message = "Inactive Token" };

			userCurrentRefreshToken.RevokedOn = DateTime.UtcNow;

			var newRefreshToken = _tokenService.GenerateRefreshToken(_refreshTokenExpirationDays);

			user.RefreshTokens.Add(newRefreshToken);

			await _userManager.UpdateAsync(user);

			var jwtToken = await _tokenService.GenerateAccessTokenAsync(user);
			var roles = await _userManager.GetRolesAsync(user);

			return new AuthModel()
			{
				DisplayName = user.DisplayName,
				Email = user.Email ?? string.Empty,
				Roles = roles.ToList(),
				IsAuthenticated = true,
				Token = jwtToken,
				RefreshToken = newRefreshToken.Token,
				RefreshTokenExpiration = newRefreshToken.ExpiresOn
			};
		}
		public async Task<bool> RevokeTokenAsync(string refreshToken)
		{
			var user = await _userManager.Users.SingleOrDefaultAsync(user => user.RefreshTokens.Any(t => t.Token == refreshToken));
			if (user is null)
				return false;

			var userCurrentRefreshToken = user.RefreshTokens.Single(t => t.Token == refreshToken);
			if (!userCurrentRefreshToken.IsActive)
				return false;

			userCurrentRefreshToken.RevokedOn = DateTime.UtcNow;

			var result = await _userManager.UpdateAsync(user);
			return result.Succeeded;
		}

		public async Task<AuthModel> GetCurrentUserAsync(ClaimsPrincipal user)
		{
			var email = user.FindFirstValue(ClaimTypes.Email)!;
			var appUser = (await _userManager.FindByEmailAsync(email))!;
			var roles = await _userManager.GetRolesAsync(appUser);
			var activeRefreshToken = appUser.RefreshTokens.Single(t => t.IsActive)!;
			var jwtToken = await _tokenService.GenerateAccessTokenAsync(appUser);

			return new AuthModel()
			{
				DisplayName = appUser.DisplayName,
				Email = appUser.Email ?? string.Empty,
				Roles = roles.ToList(),

				IsAuthenticated = true,
				Token = jwtToken,
				RefreshToken = activeRefreshToken.Token,
				RefreshTokenExpiration = activeRefreshToken.ExpiresOn,
			};
		}
	}
}
