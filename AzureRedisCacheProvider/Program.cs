using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureRedisCacheProvider
{
    class Program
    {
        static void Main(string[] args)
        {

            

            ConnectionMultiplexer connection = ConnectionMultiplexer.Connect("mediahubcache.redis.cache.windows.net,ssl=true,password=WBzyTvn5YUV8LIb6VIb6+sp9XBdD1/PUuSWZ8WKcSdk=");

            IDatabase cache = connection.GetDatabase();


            cache.StringSet("key1", "value");
            cache.StringSet("key2", 25);

            // Simple get of data types from the cache
            string key1 = cache.StringGet("key1");
            int key2 = (int)cache.StringGet("key2");

            Console.WriteLine(key1);



            Console.WriteLine("Done");
            Console.ReadLine();

        }
    }
}
