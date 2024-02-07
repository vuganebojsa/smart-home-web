using Microsoft.AspNetCore.SignalR;
using SmartHouse.Hubs.interfaces;

namespace SmartHouse.Hubs
{
    public class DeviceStatus : Hub<IDeviceStatus>
    {
        public async Task Send(string propertyId, string message)
        {

            await Clients.Group(propertyId).ReceiveMessage(message);

        }
        public override async Task OnConnectedAsync()
        {
            string propertyId = Context.GetHttpContext().Request.Query["propertyId"];
            await Groups.AddToGroupAsync(Context.ConnectionId, propertyId);

            await base.OnConnectedAsync();
        }
    }
}
