using AutoMapper;
using ECommerce.Api.Dtos;
using ECommerce.Api.Dtos.AccountDtos;
using ECommerce.Api.Errors;
using ECommerce.Api.Extensions;
using ECommerce.Core.Entities.IdentityModule;
using ECommerce.Core.Interfaces.Services.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AccountController : ControllerBase
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IMapper _mapper;
		private readonly IAuthService _authService;

		public AccountController(UserManager<ApplicationUser> userManager,
			IMapper mapper,
			IAuthService authService)
		{
			_userManager = userManager;
			_mapper = mapper;
			_authService = authService;
		}


		[ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
		[HttpPost("Register")]
		public async Task<ActionResult<UserDto>> Register(RegisterDto registerInput)
		{
			var authModel = await _authService.RegisterUserAsync(registerInput.DisplayName, registerInput.Email,
				registerInput.PhoneNumber, registerInput.Password);

			if (!authModel.IsAuthenticated)
				return BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest, authModel.Message));

			var result = _mapper.Map<UserDto>(authModel);
			SetRefreshTokenToCookieStorage(authModel.RefreshToken, authModel.RefreshTokenExpiration);
			return Ok(result);
		}

		[ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
		[HttpPost("Login")]
		public async Task<ActionResult<UserDto>> Login(LoginDto loginInput)
		{
			var authModel = await _authService.LoginUserAsync(loginInput.Email, loginInput.Password);
			if (!authModel.IsAuthenticated)
				return Unauthorized(new ApiErrorResponse(StatusCodes.Status401Unauthorized, authModel.Message));

			var result = _mapper.Map<UserDto>(authModel);
			SetRefreshTokenToCookieStorage(authModel.RefreshToken, authModel.RefreshTokenExpiration);
			return Ok(result);
		}


		[ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
		[HttpGet("RefreshToken")]
		public async Task<ActionResult<UserDto>> RefreshToken()
		{
			if (!Request.Cookies.ContainsKey("refreshToken"))
				return BadRequest("refreshToken Is Required");

			var refreshToken = Request.Cookies["refreshToken"]!;

			var authModel = await _authService.RefreshTokenAsync(refreshToken);

			if (!authModel.IsAuthenticated)
				return BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest, authModel.Message));

			var result = _mapper.Map<UserDto>(authModel);
			SetRefreshTokenToCookieStorage(authModel.RefreshToken, authModel.RefreshTokenExpiration);
			return Ok(result);
		}


		[ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
		[HttpGet("RevokeToken")]
		public async Task<ActionResult<bool>> RevokeToken(string? refreshToken)
		{
			var token = refreshToken ?? Request.Cookies["refreshToken"];
			if (string.IsNullOrEmpty(token))
				return BadRequest("refreshToken Is Required");

			var isRevokedSuccessfully = await _authService.RevokeTokenAsync(token);
			return isRevokedSuccessfully
				? Ok("refreshToken Is Revoked Successfully")
				: BadRequest(new ApiErrorResponse(StatusCodes.Status400BadRequest, "Invalid/Inactive refreshToken"));
		}


		[HttpGet, Authorize]
		public async Task<ActionResult<UserDto>> GetCurrentUser()
		{
			var authModel = await _authService.GetCurrentUserAsync(User);
			return Ok(_mapper.Map<UserDto>(authModel));
		}

		[ProducesResponseType(typeof(AddressDto), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
		[Authorize]
		[HttpGet("Address")]
		public async Task<ActionResult<AddressDto>> GetUserAddress()
		{
			var appUser = await _userManager.FindUserWithAddressIncludedAsync(User);

			if (appUser?.Address is not null)
				return Ok(_mapper.Map<AddressDto>(appUser.Address));

			return NotFound(new ApiErrorResponse(StatusCodes.Status404NotFound, $"There Is No Address Provided For This User"));
		}

		[ProducesResponseType(typeof(AddressDto), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiValidationErrorResponse), StatusCodes.Status400BadRequest)]
		[Authorize]
		[HttpPut("Address")]
		public async Task<ActionResult<AddressDto>> UpdateUserAddress(AddressDto inputAddress)
		{
			var appUser = (await _userManager.FindUserWithAddressIncludedAsync(User))!;

			if (appUser.Address is null)
				appUser.Address = _mapper.Map<Address>(inputAddress);

			else
			{
				appUser.Address = _mapper.Map<Address>(inputAddress);
				appUser.Address.ApplicationUserId = appUser.Id;
			}

			var result = await _userManager.UpdateAsync(appUser);

			if (result.Succeeded)
				return Ok(_mapper.Map<AddressDto>(appUser.Address));

			return BadRequest(new ApiValidationErrorResponse()
			{
				Errors = result.Errors.Select(e => e.Description).ToList()
			});
		}
		private void SetRefreshTokenToCookieStorage(string token, DateTime expiry)
		{
			var cookieOptions = new CookieOptions()
			{
				HttpOnly = true,
				Expires = expiry
			};
			Response.Cookies.Append("refreshToken", token, cookieOptions);
		}
	}
}
