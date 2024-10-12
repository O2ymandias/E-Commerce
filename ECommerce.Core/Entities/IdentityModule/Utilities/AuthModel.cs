namespace ECommerce.Core.Entities.IdentityModule.Utilities
{
	public class AuthModel
	{
		public string DisplayName { get; set; }
		public string Email { get; set; }
		public List<string> Roles { get; set; }
		public bool IsAuthenticated { get; set; }
		public string? Message { get; set; }
		public string Token { get; set; }
		public string RefreshToken { get; set; }
		public DateTime RefreshTokenExpiration { get; set; }
	}
}
