namespace ECommerce.Core.Constants
{
	public static class Permissions
	{
		public const string Type = nameof(Permissions);

		public static List<string> GenerateAllPermissions()
		{
			List<string> allPermissions = [];
			foreach (var module in Enum.GetNames(typeof(Modules)))
				allPermissions.AddRange(GeneratePermissionsForModule(module));
			return allPermissions;
		}

		private static List<string> GeneratePermissionsForModule(string module)
		{
			return
				[
					$"{Type}.{module}.View",
					$"{Type}.{module}.Create",
					$"{Type}.{module}.Edit",
					$"{Type}.{module}.Delete",
				];
		}

		public static class Products
		{
			public const string View = $"{Type}.{nameof(Products)}.View";
			public const string Create = $"{Type}.{nameof(Products)}.Create";
			public const string Edit = $"{Type}.{nameof(Products)}.Edit";
			public const string Delete = $"{Type}.{nameof(Products)}.Delete";
		}

		public static class Brands
		{
			public const string View = $"{Type}.{nameof(Brands)}.View";
			public const string Create = $"{Type}.{nameof(Brands)}.Create";
			public const string Edit = $"{Type}.{nameof(Brands)}.Edit";
			public const string Delete = $"{Type}.{nameof(Brands)}.Delete";
		}

		public static class Categories
		{
			public const string View = $"{Type}.{nameof(Categories)}.View";
			public const string Create = $"{Type}.{nameof(Categories)}.Create";
			public const string Edit = $"{Type}.{nameof(Categories)}.Edit";
			public const string Delete = $"{Type}.{nameof(Categories)}.Delete";
		}

		public static class Users
		{
			public const string View = $"{Type}.{nameof(Users)}.View";
			public const string Create = $"{Type}.{nameof(Users)}.Create";
			public const string Edit = $"{Type}.{nameof(Users)}.Edit";
			public const string Delete = $"{Type}.{nameof(Users)}.Delete";
		}
		public static class Roles
		{
			public const string View = $"{Type}.{nameof(Roles)}.View";
			public const string Create = $"{Type}.{nameof(Roles)}.Create";
			public const string Edit = $"{Type}.{nameof(Roles)}.Edit";
			public const string Delete = $"{Type}.{nameof(Roles)}.Delete";
		}

	}
}
