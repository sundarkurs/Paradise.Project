using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisClient.Helpers
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
