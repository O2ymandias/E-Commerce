using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AdminDashboard.Models.Application
{
	public class CreateOrEditCategoryVM
	{
		[Required, DisplayName("Category Name")]
		public string CategoryName { get; set; }
	}
}
