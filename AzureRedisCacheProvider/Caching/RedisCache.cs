using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace AzureRedisCacheProvider.Caching
{
    /// <summary>
    /// Redis cache provider implementing the same interface as business hybrid cache
    /// </summary>
    public class RedisCache : IHybridCache
    {

        private IDatabase cache = null;
        private static ConnectionMultiplexer connection = null;
        private readonly Task<ConnectionMultiplexer> connectionTask;
        private object syncronizationObject = new Object();

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisCache"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public RedisCache(string configuration) : this(ConfigurationOptions.Parse(configuration, true))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisCache"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public RedisCache(ConfigurationOptions configuration)
        {
            if (RedisCache.connection == null)
            {
                try
                {
                    connectionTask = ConnectionMultiplexer.ConnectAsync(configuration);
                    connectionTask.ContinueWith(t =>
                        {
                            lock (syncronizationObject)
                            {
                                if (RedisCache.connection == null)
                                    RedisCache.connection = t.Result;
                            }
                            this.cache = RedisCache.connection.GetDatabase();
                            Trace.TraceInformation("Redis Cache Provider connection complete - Correlation Id = {0}", Trace.CorrelationManager.ActivityId);
                        });
                }
                catch (AggregateException age)
                {
                    age.Handle(e =>
                    {
                        Trace.TraceError("Redis Cache Provider error - Correlation Id = {0}\n {1}\n {2}", Trace.CorrelationManager.ActivityId, e.Message, e.StackTrace);
                        return true;
                    });
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Redis Cache Provider exception - Correlation Id = {0}\n {1}\n {2}", Trace.CorrelationManager.ActivityId, ex.Message, ex.StackTrace);
                }
            }
        }

        private bool IsCacheInitilized()
        {
            bool isCacheConnected = RedisCache.connection != null && cache != null;

            if (!isCacheConnected)
                Trace.TraceWarning("Redis Cache Provider still connecting - Correlation Id = {0}, current task status is {1}", 
                    Trace.CorrelationManager.ActivityId, this.connectionTask.Status.ToString());

            return isCacheConnected;
        }
        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="redisAbsoluteExpiration">The redis absolute expiration.</param>
        /// <param name="memoryAbsoluteExpiration">The memory absolute expiration.</param>
        /// <returns></returns>
        public bool Add(string key, ICacheItem value, DateTimeOffset redisAbsoluteExpiration, DateTimeOffset memoryAbsoluteExpiration)
        {
            //Ignore the memory expiration since we don't use memory cache in azure
            return Add(key, value, redisAbsoluteExpiration);
        }

        /// <summary>
        /// Gets the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="redisCacheDuration">Duration of the redis cache.</param>
        /// <param name="memoryCacheDuration">Duration of the memory cache.</param>
        /// <returns></returns>
        public ICacheItem Get(string key, int redisCacheDuration, int memoryCacheDuration)
        {
            //Ignore the syncronization duration flags, we don't use memory cache in azure
            return Get(key);
        }

        /// <summary>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="absoluteExpiration"></param>
        /// <returns>
        ///   <c>true</c> if insertion succeeded, or <c>false</c> if there is an already an entry in the cache that has the same key as key.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool Add(string key, ICacheItem value, DateTimeOffset absoluteExpiration)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            try
            {
                if (IsCacheInitilized())
                {
                    TimeSpan expiration = absoluteExpiration.Subtract(DateTimeOffset.UtcNow);
                    this.cache.SetAsync(key, value, expiration);
                    double expirationInSeconds = Math.Ceiling(expiration.TotalSeconds);
                    
                    Trace.TraceInformation("Redis Cache Provider: Requested to add asyn key {0} for {1} secounds. Correlation Id = {2}", 
                        key, expirationInSeconds, Trace.CorrelationManager.ActivityId);

                    return true;
                }
                else
                {
                    Trace.TraceWarning("Redis Cache Provider: Cache still not initilized when trying to add the key {0}. Correlation Id = {1}", 
                        key, Trace.CorrelationManager.ActivityId);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Redis Cache Provider: Error inserting cache item. Key={0}, Correlation Id = {2}.\n Exception Message {3}\n{4}", 
                    key, Trace.CorrelationManager.ActivityId, ex.Message, ex.StackTrace);
            }
            return false;
        }

        /// <summary>
        /// Gets the specified cache entry from the cache as a <see cref="Ptc.MediaHub.Cache.MethodCacheItem" /> instance.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry to get.</param>
        /// <returns>
        /// A reference to the cache entry that is identified by key, if the entry exists; otherwise, null.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public ICacheItem Get(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            try
            {
                if (IsCacheInitilized())
                {
                    ICacheItem cacheItem = this.cache.Get<ICacheItem>(key);

                    if (cacheItem != null)
                    {
                        Trace.TraceInformation("Redis Cache Provider: Cache item retrieved (cache hit) for key {0}. Correlation Id = {1}",
                      key, Trace.CorrelationManager.ActivityId);
                        return cacheItem;
                    }
                    else
                    {
                        Trace.TraceInformation("Redis Cache Provider: Cache item NULL (cache miss) for key {0}. Correlation Id = {1}",
                      key, Trace.CorrelationManager.ActivityId);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Redis Cache Provider: Error retrieving key {0}. Correlation Id = {1}\n Exception Message = {2} \n {3}", 
                    key, Trace.CorrelationManager.ActivityId, ex.Message, ex.StackTrace);
            }

            return null;
        }


    }
}
