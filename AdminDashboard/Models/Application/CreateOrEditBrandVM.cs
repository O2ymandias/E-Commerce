using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AdminDashboard.Models.Application
{
	public class CreateOrEditBrandVM
	{
		[Required, DisplayName("Brand Name")]
		public string BrandName { get; set; }
	}
}
