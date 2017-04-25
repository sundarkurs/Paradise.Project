using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RedisClient.Helpers;
using RedisClient.Models;
using StackExchange.Redis;

namespace RedisClient.Caching
{
    internal sealed class QuickRedisCacheProvider : IDisposable
    {
        private ConnectionMultiplexer _connectionMultiplexer;

        private bool _disposed;

        public QuickRedisCacheProvider()
        {
            var connectionString = "HydraQA.redis.cache.windows.net,ssl=true,password=HNGDwMtA02f8aeO52rdOcx8ZRYbvfM/nvAjS1V9V15s=";  //"localhost";
            _connectionMultiplexer = ConnectionMultiplexer.Connect(connectionString);
        }

        public string Set(string key, string value)
        {
            var database = _connectionMultiplexer.GetDatabase();
            database.StringSet(key, value);
            return value;
        }

        public string Get(string key)
        {
            var database = _connectionMultiplexer.GetDatabase();
            string value = database.StringGet(key);
            return value;
        }

        public void SetEmployees(Employee employee)
        {
            var database = _connectionMultiplexer.GetDatabase();
            database.StringSet("s-emp-" + employee.Id.ToString(), JsonConvert.SerializeObject(employee));
        }

        public Employee GetEmployee(int key)
        {
            var database = _connectionMultiplexer.GetDatabase();
            Employee employee = JsonConvert.DeserializeObject<Employee>(database.StringGet("s-emp-" + key));
            return employee;
        }

        public void GetAllKeys()
        {
            var server = _connectionMultiplexer.GetServer("HydraQA.redis.cache.windows.net", 6380);

            //var server = _connectionMultiplexer.GetServer(_connectionMultiplexer.Configuration);

            foreach (var key in server.Keys())
            {
                Console.WriteLine(key);
            }
        }

        public void Delete(string key)
        {
            var database = _connectionMultiplexer.GetDatabase();
            database.KeyDelete(key);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                if (_connectionMultiplexer != null)
                {
                    _connectionMultiplexer.Dispose();
                    _connectionMultiplexer = null;
                    _disposed = true;
                }
            }
        }

        ~QuickRedisCacheProvider()
        {
            Dispose(true);
        }
    }
}
