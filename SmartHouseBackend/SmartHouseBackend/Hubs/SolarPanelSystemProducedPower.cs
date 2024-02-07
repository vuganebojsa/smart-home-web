using Microsoft.AspNetCore.SignalR;
using SmartHouse.Hubs.interfaces;

namespace SmartHouse.Hubs
{
    public class SolarPanelSystemProducedPower : Hub<IDeviceStatus>
    {
        public async Task Send(string deviceId, string message)
        {

            await Clients.Group(deviceId).ReceiveMessage(message);

        }
        public override async Task OnConnectedAsync()
        {
            string deviceId = Context.GetHttpContext().Request.Query["deviceId"];
            await Groups.AddToGroupAsync(Context.ConnectionId, deviceId);


            await base.OnConnectedAsync();
        }
    }
}
