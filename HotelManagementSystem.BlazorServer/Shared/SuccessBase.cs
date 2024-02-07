using Microsoft.AspNetCore.Components;

namespace HotelManagementSystem.BlazorServer.Shared
{
    public class SuccessBase: ComponentBase
    {
        [Parameter]
        public string SuccessMessage { get; set; }

    }
}
