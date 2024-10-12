using AdminDashboard.Models.Auth.AdminViewModels;
using ECommerce.Core.Constants;
using ECommerce.Core.Entities.IdentityModule;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AdminDashboard.Controllers
{
	public class AdminController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly IWebHostEnvironment _env;

		public AdminController(UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager,
			IWebHostEnvironment env)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_env = env;
		}

		[HttpGet]
		public IActionResult Login() => View();

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginVM input)
		{
			if (ModelState.IsValid)
			{
				var user = input.UserNameOrEmail.Contains('@')
					? await _userManager.FindByEmailAsync(input.UserNameOrEmail)
					: await _userManager.FindByNameAsync(input.UserNameOrEmail);

				if (user is not null && (await _userManager.IsInRoleAsync(user, Roles.SuperAdmin) || await _userManager.IsInRoleAsync(user, Roles.Admin)))
				{
					var isCorrectPassword = await _userManager.CheckPasswordAsync(user, input.Password);
					if (isCorrectPassword)
					{
						var result = await _signInManager.PasswordSignInAsync(user, input.Password, input.RememberMe, false);
						if (!result.Succeeded)
						{
							ModelState.AddModelError(string.Empty, "Something Went Wrong Trying Signing In!!");
							return View(input);
						}
						return RedirectToAction(nameof(HomeController.Index), "Home");
					}
				}

				ModelState.AddModelError(string.Empty, "Unauthorized, You Are -_-");
			}
			return View(input);
		}

		[HttpGet]
		public async Task<IActionResult> LogOut()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction(nameof(Login));
		}

		[HttpGet]
		public IActionResult AccessDenied() => View();
	}
}
