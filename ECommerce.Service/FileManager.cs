using ECommerce.Core.Interfaces.Services.Contract;
using Microsoft.AspNetCore.Http;

namespace ECommerce.Service
{
	public class FileManager : IFileManager
	{

		public async Task<string> UploadFileAsync(IFormFile file, string folderName)
		{
			if (file is null)
				return string.Empty;

			var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folderName);

			if (!Directory.Exists(folderPath))
				Directory.CreateDirectory(folderPath);

			var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

			var filePath = Path.Combine(folderPath, fileName);

			using var stream = new FileStream(filePath, FileMode.Create);
			await file.CopyToAsync(stream);

			return $"{folderName}/{fileName}";
		}
		public void DeleteFile(string fileName)
		{
			var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", fileName);
			if (File.Exists(filePath))
				File.Delete(filePath);
		}
	}
}
