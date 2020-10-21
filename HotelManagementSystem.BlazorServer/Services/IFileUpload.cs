using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorInputFile;

namespace HotelManagementSystem.BlazorServer.Services
{
    public interface IFileUpload
    {
        Task<string> UploadFile(IFileListEntry file);
    }
}
