using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureRedisCacheProvider.Caching
{
    //[Serializable]
    public class CacheItem : ICacheItem
    {
        public object ReturnValue
        {
            get;
            set;
        }

        public DateTimeOffset LastModified
        {
            get;
            set;
        }
    }
}
