using System.Text.Json;

namespace ECommerce.Service.Helpers
{
	public sealed class CustomJsonSerializerOptions
	{
		private CustomJsonSerializerOptions()
		{
		}

		public static JsonSerializerOptions Options { get; }

		static CustomJsonSerializerOptions()
		{
			Options = new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase
			};
		}
	}
}
