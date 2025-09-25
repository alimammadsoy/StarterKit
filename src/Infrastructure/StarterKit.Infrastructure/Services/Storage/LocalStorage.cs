using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using StarterKit.Application.Abstractions.Services.Storage;

namespace StarterKit.Infrastructure.Services.Storage
{
    public class LocalStorage : Storage, ILocalStorage
    {
        private readonly IWebHostEnvironment _webhostEnviroment;
        public LocalStorage(IWebHostEnvironment webHostEnvironment)
        {
            _webhostEnviroment = webHostEnvironment;
        }
        public async Task DeleteAsync(string path, string fileName)
         => File.Delete($"{path}\\{fileName}");

        public List<string> GetFiles(string path)
        {
            DirectoryInfo directory = new(path);
            return directory.GetFiles().Select(f => f.Name).ToList();
        }

        public bool HasFile(string path, string fileName)
            => File.Exists($"{path}\\{fileName}");

        private async Task<bool> CopyFileAsync(string path, IFormFile file)
        {
            try
            {
                await using FileStream fileStream = new(path, FileMode.Create, FileAccess.Write, FileShare.None, 1024 * 1024, useAsync: false);
                await file.CopyToAsync(fileStream);
                await fileStream.FlushAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<List<string>> UploadAsync(string path, IFormFileCollection files)
        {
            string uploadPath = Path.Combine("App_Data", path);
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            List<string> datas = new();
            List<bool> results = new();

            foreach (IFormFile file in files)
            {
                string fileNewName = await FileRenameAsync(path, file.FileName, HasFile);

                bool result = await CopyFileAsync($"{uploadPath}/{fileNewName}", file);
                datas.Add($"{path}/{fileNewName}");
                results.Add(result);
            }

            return datas;
        }

        public async Task<string> Upload(string path, IFormFile file)
        {
            string uploadPath = Path.Combine("App_Data", path);
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            string fileNewName = await FileRenameAsync(path, file.FileName, HasFile);

            string extension = Path.GetExtension(fileNewName);
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileNewName);
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string fileNameWithDate = $"{fileNameWithoutExt}_{timestamp}{extension}";

            await CopyFileAsync(Path.Combine(uploadPath, fileNameWithDate), file);
            string fileName = Path.Combine(path, fileNameWithDate).Replace("\\", "/");

            return fileName;
        }
    }
}
