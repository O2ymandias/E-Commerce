namespace ECommerce.Service.Helpers
{
	public class JwtSettings
	{
		public string Issuer { get; set; }
		public string Audience { get; set; }
		public double MinutesBeforeExpiry { get; set; }
		public string SecurityKey { get; set; }
	}
}
