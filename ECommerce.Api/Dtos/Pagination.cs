namespace ECommerce.Api.Dtos
{
	public class Pagination<T>
	{
		public int PageIndex { get; set; }
		public int PageSize { get; set; }
		public int Count { get; set; }
		public IReadOnlyList<T> Data { get; set; }
	}
}
