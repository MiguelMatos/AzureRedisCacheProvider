using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureRedisCacheProvider.Caching
{
    public interface ICache
    {
        /// <summary>
        /// Inserts a cache entry into the cache without overwriting any existing cache entry.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="value">The cache entry to insert.</param>
        /// <param name="absoluteExpiration">The fixed date and time at which the cache entry will expire.</param>

        /// <returns>
        ///   <c>true</c> if insertion succeeded, or <c>false</c> if there is an already an entry in the cache that has the same key as key.
        /// </returns>
        bool Add(string key, ICacheItem value, DateTimeOffset absoluteExpiration);

        /// <summary>
        /// Gets the specified cache entry from the cache as a <see cref="Ptc.MediaHub.Cache.MethodCacheItem" /> instance.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry to get.</param>
        /// <returns>
        /// A reference to the cache entry that is identified by key, if the entry exists; otherwise, null.
        /// </returns>
        ICacheItem Get(string key);
    }
}
