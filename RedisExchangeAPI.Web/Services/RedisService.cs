using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisExchangeAPI.Web.Services
{
    public class RedisService
    {
        private readonly string _redisHost;
        private readonly string _rediPort;
        private ConnectionMultiplexer _redis;
        public IDatabase database { get; set; }


        public RedisService(IConfiguration configuration)
        {

            _redisHost = configuration["Redis:Host"];
            _rediPort = configuration["Redis:Port"];
        }

        public void Connect()
        {
            var configString = $"{_redisHost}:{_rediPort}";
            _redis = ConnectionMultiplexer.Connect(configString);


        }

        public IDatabase GetDb(int db)
        {
            return _redis.GetDatabase(db);
        }
    }
}
