using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace RedisClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello Redis");

            Set("s-name", "Sundar");
            Console.WriteLine(Get("s-name"));

            Delete("s-name");
            Console.WriteLine(Get("s-name"));

            Console.Read();
        }

        public static string Set(string key, string value)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            IDatabase db = redis.GetDatabase();
            db.StringSet(key, value);
            return value;
        }

        public static string Get(string key)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            IDatabase db = redis.GetDatabase();
            string value = db.StringGet(key);
            return value;
        }

        public static void Delete(string key)
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
            IDatabase db = redis.GetDatabase();
            db.KeyDelete(key);
        }
    }
}
