﻿namespace ECommerce.Core.Entities.IdentityModule
{
	public class RefreshToken
	{
		public int Id { get; set; }
		public string Token { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTime ExpiresOn { get; set; }
		public DateTime? RevokedOn { get; set; }

		public bool IsExpired => DateTime.UtcNow >= ExpiresOn;
		public bool IsActive => RevokedOn is null && !IsExpired;
	}
}
