using ECommerce.Core.Entities.IdentityModule;
using ECommerce.Core.Interfaces.Services.Contract;
using ECommerce.Service.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ECommerce.Service
{
	public class TokenService : ITokenService
	{
		private readonly JwtSettings _jwtSettings;
		private readonly UserManager<ApplicationUser> _userManager;

		public TokenService(IOptions<JwtSettings> jwtSettings,
			UserManager<ApplicationUser> userManager)
		{
			_jwtSettings = jwtSettings.Value;
			_userManager = userManager;
		}

		public RefreshToken GenerateRefreshToken(int expirationDays)
		{
			var randomNumber = new byte[32];
			RandomNumberGenerator.Fill(randomNumber);
			var token = Convert.ToBase64String(randomNumber);

			return new RefreshToken()
			{
				Token = token,
				CreatedOn = DateTime.UtcNow,
				ExpiresOn = DateTime.UtcNow.AddDays(expirationDays),
			};
		}

		public async Task<string> GenerateAccessTokenAsync(ApplicationUser appUser)
		{
			var privateClaims = new List<Claim>()
			{
				new (ClaimTypes.NameIdentifier, appUser.Id),
				new (ClaimTypes.Name, appUser.UserName!),
				new (ClaimTypes.Email, appUser.Email!)
			};

			var userRoles = await _userManager.GetRolesAsync(appUser);
			foreach (var role in userRoles)
				privateClaims.Add(new Claim(ClaimTypes.Role, role));

			string securityKey = _jwtSettings.SecurityKey;

			var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));

			var token = new JwtSecurityToken(
				issuer: _jwtSettings.Issuer,
				audience: _jwtSettings.Audience,
				expires: DateTime.UtcNow.AddMinutes(_jwtSettings.MinutesBeforeExpiry),
				claims: privateClaims,
				signingCredentials: new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256));

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
