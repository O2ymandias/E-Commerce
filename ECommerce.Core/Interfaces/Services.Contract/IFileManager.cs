using Microsoft.AspNetCore.Http;

namespace ECommerce.Core.Interfaces.Services.Contract
{
	public interface IFileManager
	{
		Task<string> UploadFileAsync(IFormFile file, string folderName);
		void DeleteFile(string fileName);
	}
}
