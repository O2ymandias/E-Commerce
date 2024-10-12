using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AdminDashboard.Models.Auth.UserViewModels
{
	public class UserVM
	{
		public string Id { get; set; }

		[DisplayName("Display Name")]
		public string DisplayName { get; set; }

		[DisplayName("User Name")]
		public string UserName { get; set; }

		[DisplayName("Email Address"), EmailAddress]
		public string Email { get; set; }

		[DisplayName("Phone Number")]
		public string PhoneNumber { get; set; }

		public IList<string> Roles { get; set; }
	}
}
