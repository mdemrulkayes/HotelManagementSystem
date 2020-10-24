using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BlazorInputFile;
using HotelManagementSystem.BlazorServer.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace HotelManagementSystem.BlazorServer.Data
{
    public class FileUpload: IFileUpload
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;

        public FileUpload(IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
        }

        public async Task<string> UploadFile(IFileListEntry file)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(file.Name);
                var fileName = Guid.NewGuid().ToString() + fileInfo.Extension;
                var folderDirectory = $"{_webHostEnvironment.WebRootPath}\\RoomImages";
                var path = Path.Combine(_webHostEnvironment.WebRootPath, "RoomImages", fileName);
                var memoryStream = new MemoryStream();
                await file.Data.CopyToAsync(memoryStream);

                if (!Directory.Exists(folderDirectory))
                {
                    Directory.CreateDirectory(folderDirectory);
                }

                await using (var fs = new FileStream(path,FileMode.Create,FileAccess.Write))
                {
                    memoryStream.WriteTo(fs);
                }

                var fullPath = $"{_configuration["ImageUrl"]}/RoomImages/{fileName}";
                return fullPath;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public bool DeleteFile(string fileName)
        {
            bool status = false;
            try
            {
                var path = $"{_webHostEnvironment.WebRootPath}\\RoomImages\\{fileName}";
                if (File.Exists(path))
                {
                    File.Delete(path);
                    status = true;
                }
                return status;
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
