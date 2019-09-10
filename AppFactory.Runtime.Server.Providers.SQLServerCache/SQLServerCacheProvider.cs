using AppFactory.Runtime.Common.Foundation.AgentsRuntime;
using AppFactory.Runtime.Common.Foundation.Constants;
using AppFactory.Runtime.Common.Foundation.Design;
using AppFactory.Runtime.Common.Foundation.Helpers;
using AppFactory.Runtime.Common.Foundation.MEF;
using AppFactory.Runtime.Common.Foundation.MEF.Design.Providers;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.SqlServer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace AppFactory.Runtime.Server.Providers.SQLServerCache
{
    public class SQLServerCacheProvider : ProviderBase, ISQLServerCacheProvider
    {
        #region default 
        public override string LoadMessage => "SQL Server Cache";

        public override int Priority { get { return ProviderPriority.Storage; } } 
        #endregion default 

        private IDistributedCache _cache = null;
        public const string ConnectionString = @"Data Source=DESKTOP-T8R781C\LOCALDB;Initial Catalog=DistributedCacheStore;Integrated Security=True";
        protected override async Task InternalInitializeAsync()
        {
            LoadCacheSettings();
        }

        // substitute 
        public async Task InitalizeProvider()
        {
            await InternalInitializeAsync();
        }

        private void LoadCacheSettings()
        {
           
            _cache = new SqlServerCache(new SqlServerCacheOptions()
            {
                ConnectionString = ConnectionString,
                SchemaName = "dbo",
                TableName = "CacheTable"
            });
            
        }

        #region async method
        public async Task<T> GetAsync<T>(string key)
        {
            if (await HasKeyAsync(key))
            {
                var result = await _cache.GetAsync(key);
                return Compressor.Decompressed<T>(result);
            }
            else
                return default(T);
        }

        public async Task SetAsync<T>(string key, T item, TimeSpan? duration)
        {
            var result = Compressor.Compressed(item);
            await _cache.SetAsync(key, result, new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = duration });
        }

        public async Task SetAsync<T>(string key, Func<T> function, TimeSpan? duration)
        {
            var functionResult = function.Invoke();
            var result = Compressor.Compressed(functionResult);
            await _cache.SetAsync(key, result, new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = duration });
        }

        public async Task SetAsync<T>(string key, T item, DateTimeOffset? duration)
        {
            var result = Compressor.Compressed(item);
            await _cache.SetAsync(key, result, new DistributedCacheEntryOptions() { AbsoluteExpiration = duration });
        }

        public async Task SetAsync<T>(string key, Func<T> function, DateTimeOffset? duration)
        {
            var functionResult = function.Invoke();
            var result = Compressor.Compressed(functionResult);
            await _cache.SetAsync(key, result, new DistributedCacheEntryOptions() { AbsoluteExpiration = duration });
        }

        public async Task SetAsync<T>(string key, T item)
        {
            var result = Compressor.Compressed(item);
            await _cache.SetAsync(key, result);
        }

        public async Task SetAsync<T>(string key, Func<T> function)
        {
            var functionResult = function.Invoke();
            var result = Compressor.Compressed(functionResult);
            await _cache.SetAsync(key, result);
        }

        public async Task<bool> HasKeyAsync(string key)
        {
            var result = await _cache.GetAsync(key);
            if (result == null)
                return false;
            return true;
        }

        public async Task RemoveAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }

        public async Task RefreshAsync(string key)
        {
            await _cache.RefreshAsync(key);
        }
        #endregion async method

        #region sync method
        public T Get<T>(string key)
        {
            if (!HasKey(key)) return default(T);
            var result = _cache.Get(key);
            return Compressor.Decompressed<T>(result);
        }

        public void Set<T>(string key, T item, TimeSpan? duration)
        {
            var result = Compressor.Compressed(item);
            _cache.Set(key, result, new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = duration });
        }

        public void Set<T>(string key, Func<T> function, TimeSpan? duration)
        {
            var functionResult = function.Invoke();
            var result = Compressor.Compressed(functionResult);
            _cache.Set(key, result, new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = duration, SlidingExpiration = duration });
        }

        public void Set<T>(string key, T item)
        {
            var result = Compressor.Compressed(item);
            _cache.Set(key, result);
        }

        public void Set<T>(string key, Func<T> function)
        {
            var functionResult = function.Invoke();
            var result = Compressor.Compressed(functionResult);
             _cache.Set(key, result);
        }

        public bool HasKey(string key)
        {
            var result = _cache.Get(key);
            return result != null;
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public void Refresh(string key)
        {
             _cache.Refresh(key);
        }
       
        #endregion sync method

    }
}
