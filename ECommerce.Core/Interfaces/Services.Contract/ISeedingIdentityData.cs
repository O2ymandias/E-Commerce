namespace ECommerce.Core.Interfaces.Services.Contract
{
	public interface ISeedingIdentityData
	{
		Task SeedRolesAsync();
		Task SeedUsersAsync();
	}
}
