using System.ComponentModel.DataAnnotations;

namespace ECommerce.Api.Dtos.AccountDtos
{
	public class RegisterDto
	{
		[Required]
		public string DisplayName { get; set; }

		[Required]
		[EmailAddress]
		public string Email { get; set; }

		[Required]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
			ErrorMessage = "Password must be at least 6 characters long and contains at least:\n" +
			"one uppercase letter,\n" +
			"one lowercase letter,\n" +
			"one digit,\n" +
			"and one special character.")]
		public string Password { get; set; }

		[Required]
		public string PhoneNumber { get; set; }
	}
}
