using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AdminDashboard.Models.Auth.RoleViewModels
{
	public class RoleVM
	{
		[DisplayName("Role Id")]
		public string Id { get; set; }


		[Required(), DisplayName("Role Name"), StringLength(50, MinimumLength = 3)]
		public string Name { get; set; }

	}
}
