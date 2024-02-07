using InfluxDB.Client;
using SmartHouse.Infrastructure.Interfaces.Services;

namespace SmartHouse.Services
{
    public class InfluxDBService : IInfluxDbService
    {
        private readonly string _token;
        private readonly string _org;
        private readonly string _url;
        private readonly string _bucket;
        private readonly string _username;
        private readonly string _password;

        public InfluxDBService(IConfiguration configuration)
        {
            _token = configuration["InfluxDbSettings:Token"];
            _org = configuration["InfluxDbSettings:Org"];
            _url = configuration["InfluxDbSettings:Url"];
            _bucket = configuration["InfluxDbSettings:Bucket"];

            _username = configuration["InfluxDbSettings:Username"];
            _password = configuration["InfluxDbSettings:Password"];

        }

        public void Write(Action<WriteApi> action)
        {
            using var client = new InfluxDBClient("http://192.168.105.29:8086", _token);

            using var write = client.GetWriteApi();
            action(write);
        }

        public async Task<T> QueryAsync<T>(Func<QueryApi, Task<T>> action)
        {
            using var client = new InfluxDBClient("http://192.168.105.29:8086", _token);

            var query = client.GetQueryApi();
            return await action(query);
        }
    }
}
