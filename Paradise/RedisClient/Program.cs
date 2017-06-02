using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedisClient.Caching;
using RedisClient.Models;
using StackExchange.Redis;

namespace RedisClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello Redis");


            GetAllKeys();

            //UseRedisProvider();

            Console.Read();
        }

        public static void UseRedisProvider()
        {
            var cacheProvider = new RedisCacheProvider();
            cacheProvider.Set("k-name", "Redis Client");
            Console.WriteLine(cacheProvider.Get<string>("k-name"));
        }
        public static void UseQuickProvider()
        {
            var redisProvider = new QuickRedisCacheProvider();

            redisProvider.GetAllKeys();

            redisProvider.Set("s-name", "Sundar");

            redisProvider.SetEmployees(new Employee(1, "Sundar"));
            var emp = redisProvider.GetEmployee(1);

            redisProvider.GetAllKeys();

            Console.WriteLine(redisProvider.Get("s-name"));

            redisProvider.Delete("s-name");
            Console.WriteLine(redisProvider.Get("s-name"));
        }

        public static void GetAllKeys()
        {
            var redisProvider = new QuickRedisCacheProvider();
            redisProvider.GetAllKeys();
        }

    }
}
