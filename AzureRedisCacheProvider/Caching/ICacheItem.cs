using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureRedisCacheProvider.Caching
{
        public interface ICacheItem
        {
            /// <summary>
            /// Gets or sets the return value.
            /// </summary>
            /// <value>
            /// The return value.
            /// </value>
            object ReturnValue { get; set; }
            /// <summary>
            /// Gets or sets the last modified.
            /// </summary>
            /// <value>
            /// The last modified.
            /// </value>
            DateTimeOffset LastModified { get; set; }
        }
}
