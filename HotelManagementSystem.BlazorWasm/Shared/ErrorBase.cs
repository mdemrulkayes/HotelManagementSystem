using Microsoft.AspNetCore.Components;

namespace HotelManagementSystem.BlazorWasm.Shared
{
    public class ErrorBase: ComponentBase
    {
        [Parameter]
        public string ErrorMessage { get; set; }
    }
}
