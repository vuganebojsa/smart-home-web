namespace SmartHouse.Hubs.interfaces
{
    public interface IDeviceStatus
    {
        Task ReceiveMessage(string message);

        Task OnOffChanged(string message);
    }
}
