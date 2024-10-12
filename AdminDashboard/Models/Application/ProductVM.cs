using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AdminDashboard.Models.Application
{
	public class ProductVM
	{
		public int Id { get; set; }
		public string Name { get; set; }

		[DisplayName("Picture")]
		public string PictureUrl { get; set; }
		public string Description { get; set; }

		[DataType(DataType.Currency)]
		public decimal Price { get; set; }
		public string Brand { get; set; }
		public string Category { get; set; }
	}
}
