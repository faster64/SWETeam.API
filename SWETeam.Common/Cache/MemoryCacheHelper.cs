using Microsoft.Extensions.Caching.Memory;
using System;

namespace SWETeam.Common.Caching
{
    /// <summary>
    /// Memory Cache Helper
    /// </summary>
    public static class MemoryCacheHelper
    {
        #region Declares
        /// <summary>
        /// Memory Cache
        /// </summary>
        private static MemoryCache _memoryCache;
        public static MemoryCache memoryCache
        {
            get
            {
                if (_memoryCache == null)
                {
                    _memoryCache = new MemoryCache(new MemoryCacheOptions());
                }
                return _memoryCache;
            }
        }

        /// <summary>
        /// Mặc định cache trong 1d
        /// </summary>
        public static double defaultCacheTime = 60 * 24;
        #endregion

        #region Methods
        /// <summary>
        /// Get cache
        /// </summary>
        public static object Get(string key)
        {
            return memoryCache.Get(key);
        }

        /// <summary>
        /// Set cache
        /// </summary>
        /// <param name="key">Key cache</param>
        /// <param name="value">Giá trị cache</param>
        /// <param name="cacheTime">Thời gian cache (minutes)</param>
        public static void Set(string key, object value, double? cacheTime = default)
        {
            if (cacheTime == null)
            {
                memoryCache.Set(key, value, TimeSpan.FromMinutes(defaultCacheTime));
            }
            else
            {
                memoryCache.Set(key, value, TimeSpan.FromMinutes(cacheTime.Value));
            }

        }

        /// <summary>
        /// Try get value
        /// </summary>
        public static bool TryGetValue(string key, out object result)
        {
            return memoryCache.TryGetValue(key, out result);
        }

        /// <summary>
        /// Remove cache
        /// </summary>
        public static void Remove(string key)
        {
            memoryCache.Remove(key);
        }

        /// <summary>
        /// Get count
        /// </summary>
        public static int GetCount()
        {
            return memoryCache.Count;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public static void Dispose()
        {
            memoryCache.Dispose();
        }
        #endregion
    }
}
