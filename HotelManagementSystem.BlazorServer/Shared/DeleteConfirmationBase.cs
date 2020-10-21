using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace HotelManagementSystem.BlazorServer.Shared
{
    public class DeleteConfirmationBase: ComponentBase
    {
        public bool IsProcessStart { get; set; } = false;

        [Parameter]
        public string ConfirmationTitle { get; set; } = "Confirm Delete";

        [Parameter]
        public string ConfirmationMessage { get; set; } = "Are you sure you want to delete";

        [Parameter]
        public EventCallback<bool> ConfirmationChanged { get; set; }

        protected async Task OnConfirmationChange(bool value)
        {
            if (value)
            {
                IsProcessStart = true;
            }
            await ConfirmationChanged.InvokeAsync(value);
        }
    }
}
