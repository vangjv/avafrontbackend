namespace AvaFront.API.Services
{
    using StackExchange.Redis;
    using System;

    public class RedisService
    {
        private static Lazy<ConnectionMultiplexer> lazyConnection;
        TimeSpan defaultTimeSpan = new TimeSpan(7, 0, 0, 0);
        public RedisService(string connectionString)
        {
            lazyConnection = new Lazy<ConnectionMultiplexer>(() => {
                return ConnectionMultiplexer.Connect(connectionString);
            });
        }

        public IDatabase GetDatabase()
        {
            return lazyConnection.Value.GetDatabase();
        }

        public ISubscriber GetSubscriber()
        {
            return lazyConnection.Value.GetSubscriber();
        }

        public async Task<bool> SetCache(string key, string value)
        {
            return await lazyConnection.Value.GetDatabase().StringSetAsync(key, value, defaultTimeSpan);
        }   

        public async Task<string?> GetCache(string key)
        {
            return await lazyConnection.Value.GetDatabase().StringGetAsync(key);
        }
    }

}
