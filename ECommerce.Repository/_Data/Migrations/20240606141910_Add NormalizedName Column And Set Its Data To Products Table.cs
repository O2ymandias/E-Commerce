using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Repository._Data.Migrations
{
	/// <inheritdoc />
	public partial class AddNormalizedNameColumnAndSetItsDataToProductsTable : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<string>(
				name: "NormalizedName",
				table: "Products",
				type: "nvarchar(max)",
				nullable: false,
				defaultValue: "");

			migrationBuilder.Sql("UPDATE Products SET NormalizedName = UPPER(Name)");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "NormalizedName",
				table: "Products");
		}
	}
}
