using AzureRedisCacheProvider.Caching;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzureRedisCacheProvider
{
    class Program
    {
        static void Main(string[] args)
        {

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();


            Console.WriteLine(string.Format("{0} : Cache initialization starting...", sw.ElapsedMilliseconds));
            IHybridCache cache = new RedisCache("mediahubcache.redis.cache.windows.net,ssl=true,password=");
            Console.WriteLine(string.Format("{0} : Cache initialization done...", sw.ElapsedMilliseconds));

            ICacheItem ci = new CacheItem();
            ci.ReturnValue = "item a";
            ci.LastModified = DateTimeOffset.UtcNow;

            Console.WriteLine(string.Format("{0} : Sleeping for 500ms to wait for the connection to be made", sw.ElapsedMilliseconds));
            //Thread.Sleep(500);

            Console.WriteLine(string.Format("{0} : Lets add cache item", sw.ElapsedMilliseconds));
            bool addResult = cache.Add("a", ci, DateTimeOffset.UtcNow.AddSeconds(10));

            while (!addResult)
            {
                Console.WriteLine(string.Format("{0} : Item add failed, retrying..", sw.ElapsedMilliseconds));
                addResult = cache.Add("a", ci, DateTimeOffset.UtcNow.AddSeconds(10));
            }

            Console.WriteLine(string.Format("{0} : Done.", sw.ElapsedMilliseconds));

            Console.WriteLine(string.Format("{0} : Retriving the item back", sw.ElapsedMilliseconds));
            var item = cache.Get("a");

            if (item != null)
            {
                Console.WriteLine(string.Format("{0} : Done.", sw.ElapsedMilliseconds));
            }
            else
            {
                Console.WriteLine(string.Format("{0} : Error.", sw.ElapsedMilliseconds));

            }

            Console.ReadLine();

            //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            //sw.Start();
            //ConnectionMultiplexer connection = ConnectionMultiplexer.Connect("mediahubcache.redis.cache.windows.net,ssl=true,password=9SPYdYgu4p23gm7ic1zNoXvnWkKPcq3W0siQaqyAUH0=");

            //IDatabase cache = connection.GetDatabase();


            //cache.StringSet("key1", "value");


            //// Simple get of data types from the cache
            //string key1 = cache.StringGet("key1");
     

            //Console.WriteLine(key1);

            ////RedisKey key;
            ////RedisValue value;

            //Console.WriteLine("Done");
            //Console.WriteLine(sw.ElapsedMilliseconds);
            //Console.ReadLine();


        }
    }
}
