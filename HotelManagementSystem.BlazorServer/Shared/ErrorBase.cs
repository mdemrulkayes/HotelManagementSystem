using Microsoft.AspNetCore.Components;

namespace HotelManagementSystem.BlazorServer.Shared
{
    public class ErrorBase: ComponentBase
    {
        [Parameter]
        public string ErrorMessage { get; set; }
    }
}
