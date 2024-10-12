using ECommerce.Api.CustomMiddlewares;
using ECommerce.Api.Extensions;
using ECommerce.Api.Filters;
using Newtonsoft.Json;

namespace ECommerce.Api
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			#region Add services to the container

			builder.Services
				.AddControllers(config =>
				{
					config.Filters.Add(typeof(CustomExceptionFilter));
				})
				.AddNewtonsoftJson(options =>
				{
					options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
				});
			builder.Services.
				AddEndpointsApiExplorer().
				AddSwaggerGen();
			builder.Services.AddCors(setupAction =>
			{
				setupAction.AddPolicy("MyPolicy", config =>
				{
					config
					.AllowAnyHeader()
					.AllowAnyMethod()
					.WithOrigins(builder.Configuration["ClientBaseUrl"]!);
				});
			});
			builder.Services
				.AddApplicationServices(builder.Configuration)
				.AddIdentityServices(builder.Configuration);

			#endregion

			var app = builder.Build();

			await DatabaseInitializerExtensions.InitializeDatabaseAsync(app);

			#region Configure the HTTP request pipeline

			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseMiddleware<ExceptionMiddleware>();

			app.UseStatusCodePagesWithReExecute("/Errors/{0}");

			app.UseStaticFiles();

			app.UseHttpsRedirection();

			app.UseCors("MyPolicy");

			app.UseAuthentication();

			app.UseAuthorization();

			app.MapControllers();

			app.Run();

			#endregion
		}
	}
}
