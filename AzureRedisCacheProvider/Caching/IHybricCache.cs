using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureRedisCacheProvider.Caching
{
    public interface IHybridCache : ICache
    {
        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="appfabricAbsExpiration">The appfabric abs expiration.</param>
        /// <param name="memoryAbsExpiration">The memory abs expiration.</param>
        /// <returns></returns>
        bool Add(string key, ICacheItem value, DateTimeOffset appfabricAbsExpiration, DateTimeOffset memoryAbsExpiration);

        /// <summary>
        /// Gets the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="appFabricCacheDuration">Duration of the application fabric cache.</param>
        /// <param name="memoryCacheDuration">Duration of the memory cache.</param>
        /// <returns></returns>
        ICacheItem Get(string key, int appFabricCacheDuration, int memoryCacheDuration);
    }
}
