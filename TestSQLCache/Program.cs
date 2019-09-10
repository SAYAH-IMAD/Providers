using AppFactory.Runtime.Common.Foundation.AgentsRuntime;
using AppFactory.Runtime.Server.Providers.SQLServerCache;
using System;
using System.Threading.Tasks;

namespace TestSQLCache
{
    class Program
    {
        static void Main(string[] args)
        {
            SQLServerCacheProvider sqlCache = new SQLServerCacheProvider();
            sqlCache.InitializeAsync();

            #region sync sql cache

            sqlCache.Set("sql_cache_set", "saved context");

            var result = sqlCache.Get<string>("sql_cache_set");

            sqlCache.Remove("sql_cache_set");
            
            #endregion sync sql cache

            #region async sql cache

            sqlCache.SetAsync("sql_cache_set_async", "saved context async").Wait();

            var resultAsync = sqlCache.GetAsync<string>("sql_cache_set_async").Result;

            sqlCache.RemoveAsync("sql_cache_set").Wait();

            #endregion async sql cache

            #region sql cache server expiration time 

            sqlCache.Set("sql_cache_set_expire", "saved context", TimeSpan.FromSeconds(10));

            var resultBeforExpired = sqlCache.Get<string>("sql_cache_set_expire");
            Task.Delay(Convert.ToInt32((TimeSpan.FromSeconds(11).TotalMilliseconds))).Wait();
            var resultAfterExpired = sqlCache.Get<string>("sql_cache_set_expire");

            #endregion sql cache server expiration time

            Console.ReadKey();
        }
    }
}
