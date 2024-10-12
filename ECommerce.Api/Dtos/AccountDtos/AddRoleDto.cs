using System.ComponentModel.DataAnnotations;

namespace ECommerce.Api.Dtos.AccountDtos
{
	public class AddRoleDto
	{
		[Required]
		public string RoleName { get; set; }

		[Required]
		public string UserId { get; set; }
	}
}
