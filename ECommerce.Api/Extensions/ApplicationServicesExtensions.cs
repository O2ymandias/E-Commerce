using ECommerce.Api.CustomMiddlewares;
using ECommerce.Api.Errors;
using ECommerce.Api.Helpers;
using ECommerce.Api.Helpers.Policies;
using ECommerce.Core.Constants;
using ECommerce.Core.Entities.IdentityModule;
using ECommerce.Core.Interfaces;
using ECommerce.Core.Interfaces.Repositories.Contract;
using ECommerce.Core.Interfaces.Services.Contract;
using ECommerce.Repository;
using ECommerce.Repository._Data;
using ECommerce.Repository._Identity;
using ECommerce.Service;
using ECommerce.Service.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;

namespace ECommerce.Api.Extensions
{
	public static class ApplicationServicesExtensions
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection services,
			IConfiguration configuration)
		{
			services.AddDbContext<StoreDbContext>(options =>
			{
				options
				.UseLazyLoadingProxies()
				.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
			});
			services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
			services.AddScoped(typeof(IProductService), typeof(ProductService));
			services.AddAutoMapper(typeof(MappingProfiles));
			services.Configure<ApiBehaviorOptions>(configOptions =>
			{
				configOptions.InvalidModelStateResponseFactory = (actionContext) =>
				{
					var response = new ApiValidationErrorResponse();

					foreach (var modelStateEntry in actionContext.ModelState.Values)
						foreach (var modelError in modelStateEntry.Errors)
							response.Errors.Add(modelError.ErrorMessage);

					return new BadRequestObjectResult(response);
				};
			});
			services.AddScoped(typeof(ExceptionMiddleware));
			services.AddSingleton<IConnectionMultiplexer>(sp =>
			{
				var connectionString = configuration.GetConnectionString("RedisConnection") ??
				throw new Exception("Problem Has Occurred While Parsing 'RedisConnection' Key");

				return ConnectionMultiplexer.Connect(connectionString);
			});
			services.AddScoped(typeof(IBasketRepository), typeof(BasketRepository));
			services.AddSingleton(typeof(ICacheResponseService), typeof(CacheResponseService));
			services.AddScoped(typeof(ITokenService), typeof(TokenService));
			services.AddScoped(typeof(IOrderService), typeof(OrderService));
			services.AddScoped(typeof(IPaymentService), typeof(PaymentService));
			services.AddScoped(typeof(IAuthService), typeof(AuthService));
			services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
			services.AddTransient(typeof(IEmailSender), typeof(EmailSender));
			services.Configure<TwilioSettings>(configuration.GetSection("TwilioSettings"));
			services.AddTransient(typeof(ISmsSender), typeof(SmsSender));

			return services;
		}
		public static IServiceCollection AddIdentityServices(this IServiceCollection services,
			IConfiguration configuration)
		{
			services.AddDbContext<ApplicationIdentityDbContext>(options =>
			{
				options
				.UseLazyLoadingProxies()
				.UseSqlServer(configuration.GetConnectionString("IdentityConnection"));
			});
			services
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
			services
				.AddAuthentication(options =>
				{
					options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
					options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				})
				.AddJwtBearer(options =>
				{
					options.SaveToken = true;

					var jwtSettings = configuration.GetSection("JWT").Get<JwtSettings>()
					?? throw new Exception("Unable to bind the configurations from `JWT` section");
					options.TokenValidationParameters = new TokenValidationParameters()
					{
						ValidateIssuer = true,
						ValidIssuer = jwtSettings.Issuer,

						ValidateAudience = true,
						ValidAudience = jwtSettings.Audience,

						ValidateIssuerSigningKey = true,
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecurityKey)),

						ValidateLifetime = true,
						ClockSkew = TimeSpan.Zero
					};
				});

			services.Configure<JwtSettings>(configuration.GetSection("JWT"));
			services.Configure<SuperAdmin>(configuration.GetSection("SuperAdmin"));
			services.AddTransient<ISeedingIdentityData, SeedingIdentityData>();
			services
				.AddAuthorizationBuilder()
				.AddPolicy("Region", policyConfig =>
				{
					policyConfig.RequireRole(Roles.BasicUser);
					policyConfig.AddRequirements(new RegionRequirement("Egypt"));
				});
			services.AddSingleton(typeof(IAuthorizationHandler), typeof(RegionHandler));
			return services;
		}
	}
}
