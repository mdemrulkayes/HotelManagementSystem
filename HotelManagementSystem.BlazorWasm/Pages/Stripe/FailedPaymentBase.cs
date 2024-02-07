using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;

namespace HotelManagementSystem.BlazorWasm.Pages.Stripe
{
    public class FailedPaymentBase: ComponentBase
    {
        [Inject]
        public ILocalStorageService LocalStorageService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LocalStorageService.RemoveItemAsync("OrderDetails");
            await LocalStorageService.RemoveItemAsync("RoomId");
            await LocalStorageService.RemoveItemAsync("InitialRoomBookingInfo");
        }
    }
}
