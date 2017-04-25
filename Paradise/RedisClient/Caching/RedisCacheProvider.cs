using System;
using Microsoft.Practices.EnterpriseLibrary.WindowsAzure.TransientFaultHandling.AzureStorage;
using Microsoft.Practices.TransientFaultHandling;
using RedisClient.Helpers;
using StackExchange.Redis;

namespace RedisClient.Caching
{
    internal sealed class RedisCacheProvider : ICacheProvider, IDisposable
    {
        /// <summary>
        ///     The _connection multiplexer
        /// </summary>
        private ConnectionMultiplexer _connectionMultiplexer;

        /// <summary>
        ///     The _disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RedisCacheProvider" /> class.
        /// </summary>
        public RedisCacheProvider()
        {
            var connectionString =
                $"{ConfigurationHelper.RedisHost},ssl={ConfigurationHelper.RedisIsSecured},password={ConfigurationHelper.RedisAccountKey}";

            // Get & initalize the Redis Connection String.
            _connectionMultiplexer = ConnectionMultiplexer.Connect(connectionString);
        }

        /// <summary>
        ///     Sets the specified key and data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="data">The data.</param>
        public void Set<T>(string key, T data)
        {
            // Define your retry strategy: retry 3 times, 1 second apart.  
            var retryStrategy = new Incremental(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2));

            // Define your retry policy using the retry strategy and the Windows Azure storage  
            // transient fault detection strategy.  
            var retryPolicy = new RetryPolicy<StorageTransientErrorDetectionStrategy>(retryStrategy);

            // Do some work that may result in a transient fault.  
            try
            {
                // Call a method that uses Windows Azure storage and which may  
                // throw a transient exception.  
                retryPolicy.ExecuteAction(
                    () =>
                    {
                        var database = _connectionMultiplexer.GetDatabase();
                        database.StringSet(key, JsonSerializer.Serialize(data), ConfigurationHelper.SessionPoolTimeOut);
                    });
            }
            catch (Exception ex)
            {
                //Logger.Error(ex, $"{GetType().FullName} / {MethodBase.GetCurrentMethod().Name}");
            }
        }

        /// <summary>
        ///     Sets the specified key and data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="data">The data.</param>
        /// <param name="returnIfExists">if set to <c>true</c> [return if exists].</param>
        public T Set<T>(string key, T data, bool returnIfExists)
        {
            // Define your retry strategy: retry 3 times, 1 second apart.  
            var retryStrategy = new Incremental(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2));

            // Define your retry policy using the retry strategy and the Windows Azure storage  
            // transient fault detection strategy.  
            var retryPolicy = new RetryPolicy<StorageTransientErrorDetectionStrategy>(retryStrategy);

            // Do some work that may result in a transient fault.  
            try
            {
                // Call a method that uses Windows Azure storage and which may  
                // throw a transient exception.  
                retryPolicy.ExecuteAction(
                    () =>
                    {
                        var database = _connectionMultiplexer.GetDatabase();
                        database.StringSet(key, JsonSerializer.Serialize(data), ConfigurationHelper.SessionPoolTimeOut);
                    });
            }
            catch (Exception ex)
            {
                //Logger.Error(ex, $"{GetType().FullName} / {MethodBase.GetCurrentMethod().Name}");
            }

            return returnIfExists ? Get<T>(key) : data;
        }

        /// <summary>
        ///     Gets the specified key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            var cachedData = default(T);
            var retryStrategy = new Incremental(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2));
            var retryPolicy = new RetryPolicy<StorageTransientErrorDetectionStrategy>(retryStrategy);

            // Do some work that may result in a transient fault.  
            try
            {
                retryPolicy.ExecuteAction(() =>
                {
                    var database = _connectionMultiplexer.GetDatabase();
                    var data = database.StringGet(key);
                    if (data.HasValue)
                    {
                        cachedData = JsonSerializer.Deserialize<T>(database.StringGet(key));
                    }
                });
            }
            catch (Exception ex)
            {
                //Logger.Error(ex, $"{GetType().FullName} / {MethodBase.GetCurrentMethod().Name}");
            }
            return cachedData;
        }

        /// <summary>
        ///     Removes cached data based on key.
        /// </summary>
        /// <param name="key">Key</param>
        public void Remove(string key)
        {
            var retryStrategy = new Incremental(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2));
            var retryPolicy = new RetryPolicy<StorageTransientErrorDetectionStrategy>(retryStrategy);

            // Do some work that may result in a transient fault.  
            try
            {
                retryPolicy.ExecuteAction(() =>
                {
                    var database = _connectionMultiplexer.GetDatabase();
                    database.KeyDelete(key);
                });
            }
            catch (Exception ex)
            {
                //Logger.Error(ex, $"{GetType().FullName} / {MethodBase.GetCurrentMethod().Name}");
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
                if (_connectionMultiplexer != null)
                {
                    _connectionMultiplexer.Dispose();
                    _connectionMultiplexer = null;
                    _disposed = true;
                }
            }
        }

        /// <summary>
        ///     We want the RedisCacheProvider object to be disposed only once the static object instance loses scope.
        /// </summary>
        ~RedisCacheProvider()
        {
            Dispose(true);
        }
    }
}
