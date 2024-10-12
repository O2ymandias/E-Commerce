using ECommerce.Core.Interfaces.Services.Contract;
using ECommerce.Repository._Data;
using ECommerce.Repository._Identity;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Extensions
{
	public class DatabaseInitializerExtensions
	{
		public static async Task InitializeDatabaseAsync(WebApplication app)
		{
			using var scope = app.Services.CreateScope();
			var scopedServices = scope.ServiceProvider;

			var dbContext = scopedServices.GetRequiredService<StoreDbContext>();
			var identityDbContext = scopedServices.GetRequiredService<ApplicationIdentityDbContext>();
			var seedingIdentityData = scopedServices.GetRequiredService<ISeedingIdentityData>();


			try
			{
				await dbContext.Database.MigrateAsync();
				await StoreDbContextSeed.SeedDataAsync(dbContext);

				await identityDbContext.Database.MigrateAsync();
				await seedingIdentityData.SeedRolesAsync();
				await seedingIdentityData.SeedUsersAsync();

			}
			catch (Exception ex)
			{
				var loggerFactory = scopedServices.GetRequiredService<ILoggerFactory>();
				var logger = loggerFactory.CreateLogger<DatabaseInitializerExtensions>();
				logger.LogError(ex, "An Error Has Occurred While Applying Migrations And Seeding Data");
			}
		}
	}
}
