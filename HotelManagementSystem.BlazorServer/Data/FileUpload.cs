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
                var fileName = Guid.NewGuid() +"_"+ file.Name;
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
    }
}
