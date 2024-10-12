using System.ComponentModel.DataAnnotations;

namespace AdminDashboard.Models.Auth.UserViewModels
{
	public class CreateUserVM
	{
		[Required, Display(Name = "Display Name")]
		public string DisplayName { get; set; }

		[Required, Display(Name = "User Name")]
		public string UserName { get; set; }

		[Required, EmailAddress, Display(Name = "Email")]
		public string Email { get; set; }

		[Required, StringLength(100, MinimumLength = 6), DataType(DataType.Password), Display(Name = "Password")]
		public string Password { get; set; }

		[Required, DataType(DataType.Password), Compare(nameof(Password)), Display(Name = "Confirm password")]
		public string ConfirmPassword { get; set; }

		public List<CheckBoxVM> Roles { get; set; }

		[Display(Name = "Phone Number")]
		public string? PhoneNumber { get; set; }
	}
}
