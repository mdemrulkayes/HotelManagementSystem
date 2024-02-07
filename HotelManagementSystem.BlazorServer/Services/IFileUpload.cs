using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;

namespace HotelManagementSystem.BlazorServer.Services
{
    public interface IFileUpload
    {
        Task<string> UploadFile(IBrowserFile file);
        bool DeleteFile(string fileName);
    }
}
