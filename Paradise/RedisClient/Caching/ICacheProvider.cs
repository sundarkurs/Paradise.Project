using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisClient.Caching
{
    public interface ICacheProvider
    {
        /// <summary>
        ///     Sets the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="data">The data.</param>
        void Set<T>(string key, T data);

        /// <summary>
        ///     Sets the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="data">The data.</param>
        /// <param name="returnIfExists">if set to <c>true</c> [return if cache exists for the key].</param>
        /// <returns></returns>
        T Set<T>(string key, T data, bool returnIfExists);

        /// <summary>
        ///     Gets data based on key
        /// </summary>
        /// <typeparam name="T">Base type</typeparam>
        /// <param name="key">Key</param>
        /// <returns></returns>
        T Get<T>(string key);

        /// <summary>
        ///     Removes based on key
        /// </summary>
        /// <param name="key">Key</param>
        void Remove(string key);
    }
}
