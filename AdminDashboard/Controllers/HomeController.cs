using ECommerce.Core.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminDashboard.Controllers
{
	[Authorize(Roles = $"{Roles.SuperAdmin},{Roles.Admin}")]
	public class HomeController : Controller
	{
		public IActionResult Index() =>
			View();
	}
}
