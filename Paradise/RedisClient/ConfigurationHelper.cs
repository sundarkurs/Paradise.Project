using System;
using System.Configuration;

namespace RedisClient
{
    public static class ConfigurationHelper
    {
        /// <summary>
        ///     Redis Is Secured
        /// </summary>
        public static string RedisIsSecured => ConfigurationManager.AppSettings["RedisIsSecured"];

        /// <summary>
        ///     Redis Account Key
        /// </summary>
        public static string RedisAccountKey => ConfigurationManager.AppSettings["RedisAccountKey"];

        /// <summary>
        ///     Redis Host
        /// </summary>
        public static string RedisHost => ConfigurationManager.AppSettings["RedisHost"];

        public static TimeSpan SessionPoolTimeOut
        {
            get
            {
                int val;
                return
                    TimeSpan.FromMinutes(int.TryParse(ConfigurationManager.AppSettings["SessionPoolTimeOut"], out val)
                        ? val
                        : 30);
            }
        }
    }
}
