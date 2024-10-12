using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AdminDashboard.Models.Auth.RoleViewModels
{
	public class CreateRoleVM
	{
		[Required, DisplayName("Role Name"), StringLength(50, MinimumLength = 3)]
		public string RoleName { get; set; }
	}
}
