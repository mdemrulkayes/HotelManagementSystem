using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace HotelManagementSystem.BlazorWasm.Shared
{
    public class SuccessBase: ComponentBase
    {
        [Parameter]
        public string SuccessMessage { get; set; }

    }
}
