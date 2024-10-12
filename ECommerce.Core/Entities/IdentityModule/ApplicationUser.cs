using Microsoft.AspNetCore.Identity;

namespace ECommerce.Core.Entities.IdentityModule
{
	public class ApplicationUser : IdentityUser
	{
		public string DisplayName { get; set; }
		public virtual Address? Address { get; set; }
		public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
	}
}
