using System.ComponentModel.DataAnnotations;

namespace AdminDashboard.Models.Auth.RoleViewModels
{
	public class EditRoleVM
	{
		[Required, Display(Name = "Role Id")]
		public string RoleId { get; set; }

		[Required, Display(Name = "Role Name"), StringLength(maximumLength: 50, MinimumLength = 3)]
		public string RoleName { get; set; }

		public List<CheckBoxVM> Permissions { get; set; }
	}
}
