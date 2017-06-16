using System;
using System.Runtime.Caching;
using RedisClient.Helpers;

namespace RedisClient.Caching
{
    public sealed class InMemoryCacheProvider : ICacheProvider, IDisposable
    {
        /// <summary>
        ///     Default CacheExpiration TimeinMinutes
        /// </summary>
        private const int DefaultCacheExpirationTimeinMinutes = 30;

        /// <summary>
        ///     Cache Item Policy
        /// </summary>
        private readonly CacheItemPolicy _defaultCacheItemPolicy;

        private MemoryCache _cache;
        private bool _disposed;

        /// <summary>
        ///     Prevents a default instance of the <see cref="InMemoryCacheProvider" /> class from being created.
        /// </summary>
        private InMemoryCacheProvider()
        {
            _defaultCacheItemPolicy = new CacheItemPolicy
            {
                SlidingExpiration = TimeSpan.FromMinutes(DefaultCacheExpirationTimeinMinutes),

                // when item got removed from the cache, call back event will dispose the item immediately.
                RemovedCallback = args =>
                {
                    // Check if the items is disposable.
                    var value = args.CacheItem.Value as IDisposable;
                    // dispose the item.
                    value?.Dispose();
                }
            };

            _cache = MemoryCache.Default;
        }

        /// <summary>
        ///     Gets the instance.
        /// </summary>
        /// <value>
        ///     The instance.
        /// </value>
        public static InMemoryCacheProvider Instance { get; } = new InMemoryCacheProvider();

        /// <summary>
        ///     Sets data
        /// </summary>
        /// <typeparam name="T">Base type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="data">Data</param>
        public void Set<T>(string key, T data)
        {
            data = GetFormattedData(key, data);
            _cache.Set(new CacheItem(key, data), _defaultCacheItemPolicy);
        }

        /// <summary>
        ///     Sets data
        /// </summary>
        /// <typeparam name="T">Base type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="data">Data</param>
        /// <param name="returnIfExists">if set to <c>true</c> [return if exists].</param>
        /// <returns></returns>
        public T Set<T>(string key, T data, bool returnIfExists)
        {
            if (!_cache.Contains(key))
            {
                data = GetFormattedData(key, data);
                _cache.Set(new CacheItem(key, data), _defaultCacheItemPolicy);
            }

            return returnIfExists ? Get<T>(key) : data;
        }

        /// <summary>
        /// Gets the formatted data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        private T GetFormattedData<T>(string key, T data)
        {
            // TODO: this function is a temporary measure to verify the data is serializable or not, change the configuration to true, if this required.
            // Check if the data added to cache is serializable, otherwise if not serializable, it may lead to issues when using redis cache.
            //if(!ConfigurationHelper.ValidateCacheDataSerializable)
            //{
            //    return data;
            //}

            try
            {
                var serializeData = JsonSerializer.Serialize(data);
                var deserializeData = JsonSerializer.Deserialize<T>(serializeData);
                return deserializeData;
            }
            catch (Exception ex)
            {
                //Logger.Error(ex, $"{GetType().FullName} / {MethodBase.GetCurrentMethod().Name}");
            }

            return data;
        }

        /// <summary>
        ///     Gets data based on key
        /// </summary>
        /// <typeparam name="T">Base type</typeparam>
        /// <param name="key">Key</param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            return (T)_cache.Get(key);
        }

        /// <summary>
        ///     Removes based on key
        /// </summary>
        /// <param name="key">Key</param>
        public void Remove(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                _cache.Remove(key);
            }
        }

        /// <summary>
        ///     Dispose method
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Setting dispose
        /// </summary>
        /// <param name="disposing">Disposing value</param>
        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                if (_cache != null)
                {
                    _cache.Dispose();
                    _cache = null;
                    _disposed = true;
                }
            }
        }

        /// <summary>
        ///     We want the remove object to be disposed only once the static object instance loses scope.
        /// </summary>
        ~InMemoryCacheProvider()
        {
            Dispose(true);
        }
    }
}
