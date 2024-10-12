using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AdminDashboard.Models.Application
{
	public class CreateOrEditProductVM
	{
		[Required, StringLength(50, MinimumLength = 3)]
		public string Name { get; set; }

		[Required, Range(0.1, double.MaxValue, ErrorMessage = "The Amount Is Invalid, Minimum Amount Is $0.1")]
		public decimal Price { get; set; }

		[Required, MinLength(15)]
		public string Description { get; set; }


		[Required(ErrorMessage = "Brand Is Required"), DisplayName("Brand")]
		public int BrandId { get; set; }

		[Required(ErrorMessage = "Category Is Required"), DisplayName("Category")]
		public int CategoryId { get; set; }

		public IFormFile? Picture { get; set; }

		[ValidateNever]
		public string PictureUrl { get; set; }
		public string NormalizedName => Name.ToUpper();
	}
}
