using InfluxDB.Client;

namespace SmartHouse.Infrastructure.Interfaces.Services
{
    public interface IInfluxDbService
    {
        void Write(Action<WriteApi> action);


        Task<T> QueryAsync<T>(Func<QueryApi, Task<T>> action);

    }
}
