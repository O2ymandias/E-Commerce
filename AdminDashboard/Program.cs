using AdminDashboard.Filters;
using AdminDashboard.Helpers;
using ECommerce.Core.Entities.IdentityModule;
using ECommerce.Core.Interfaces;
using ECommerce.Core.Interfaces.Services.Contract;
using ECommerce.Repository;
using ECommerce.Repository._Data;
using ECommerce.Repository._Identity;
using ECommerce.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AdminDashboard
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			#region Add services to the container.

			builder.Services.AddControllersWithViews();
			builder.Services.AddDbContext<StoreDbContext>(options =>
			{
				options
				.UseLazyLoadingProxies()
				.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
			});

			builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
			{
				options
				.UseLazyLoadingProxies()
				.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
			});

			builder.Services
				.AddIdentity<ApplicationUser, IdentityRole>(setup =>
				{
					setup.Password.RequiredLength = 6;
					setup.Password.RequireNonAlphanumeric = true;
					setup.Password.RequireUppercase = true;
					setup.Password.RequireLowercase = true;
					setup.Password.RequireDigit = true;
					setup.Password.RequiredUniqueChars = 1;

					setup.User.RequireUniqueEmail = true;
				})
				.AddEntityFrameworkStores<ApplicationIdentityDbContext>();

			builder.Services.ConfigureApplicationCookie(options =>
			{
				options.LoginPath = new PathString("/Admin/Login");
				options.LogoutPath = new PathString("/Admin/Login");
				options.AccessDeniedPath = new PathString("/Admin/AccessDenied");
			});

			builder.Services.AddAutoMapper(typeof(MappingProfiles));

			builder.Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
			builder.Services.AddScoped(typeof(IFileManager), typeof(FileManager));

			builder.Services.AddSingleton(typeof(IAuthorizationPolicyProvider), typeof(PermissionPolicyProvider));
			builder.Services.AddScoped(typeof(IAuthorizationHandler), typeof(PermissionAuthorizationHandler));

			builder.Services.Configure<SecurityStampValidatorOptions>(options =>
			{
				options.ValidationInterval = TimeSpan.Zero;
			});

			#endregion

			var app = builder.Build();

			#region Configure the HTTP request pipeline.

			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.Run();

			#endregion
		}
	}
}
