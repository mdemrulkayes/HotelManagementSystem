using Microsoft.AspNetCore.Components;

namespace HotelManagementSystem.BlazorWasm.Shared
{
    public class SuccessBase: ComponentBase
    {
        [Parameter]
        public string SuccessMessage { get; set; }

    }
}
