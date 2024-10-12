using System.ComponentModel.DataAnnotations;

namespace AdminDashboard.Models.Auth.RoleViewModels
{
	public class DetailsRoleVM
	{
		[Display(Name = "Role Id")]
		public string Id { get; set; }

		[Display(Name = "Role Name")]
		public string Name { get; set; }

		[Display(Name = "Role Permissions")]
		public List<string> Permissions { get; set; }
	}
}
