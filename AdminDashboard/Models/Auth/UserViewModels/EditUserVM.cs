using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AdminDashboard.Models.Auth.UserViewModels
{
	public class EditUserVM
	{
		[Required, DisplayName("User Id")]
		public string UserId { get; set; }

		[Required, DisplayName("User Name")]
		public string UserName { get; set; }

		[Required, DisplayName("Display Name")]
		public string DisplayName { get; set; }

		[Required]
		public string Email { get; set; }

		[DisplayName("Phone Number")]
		public string? PhoneNumber { get; set; }

		[Required]
		public List<CheckBoxVM> Roles { get; set; } = [];
	}
}
