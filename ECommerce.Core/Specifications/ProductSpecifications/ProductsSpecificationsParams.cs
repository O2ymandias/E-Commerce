namespace ECommerce.Core.Specifications.ProductSpecifications
{
	public class ProductsSpecificationsParams
	{
		#region Constants For Pagination

		private const int _maxPageSize = 10;
		private const int _defaultPageSize = 5;
		private const int _defaultPageIndex = 1;

		#endregion

		#region Private Fields For Full Properties

		private string? _sort;
		private int _pageSize = _defaultPageSize;
		private string? _search;

		#endregion

		#region Main Properties

		public int? BrandId { get; set; }
		public int? CategoryId { get; set; }
		public string? Sort
		{
			get => _sort;
			set => _sort = value?.ToUpper();
		}
		public string? Search
		{
			get => _search;
			set => _search = value?.ToUpper();
		}
		public int PageSize
		{
			get => _pageSize;
			set => _pageSize = value switch
			{
				>= _maxPageSize => _maxPageSize,
				< _maxPageSize and > 0 => value,
				_ => _defaultPageSize
			};
		}
		public int PageIndex { get; set; } = _defaultPageIndex;

		#endregion
	}
}
