using System;

namespace ZhiDiExt.Caching
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class CacheExtensions
    {
        /// <summary>
        /// 获取对象，如对象不存在则进行缓存并指定默认的缓存过期时间
        /// </summary>
        public static T Get<T>(this ICacheManager cacheManager, string key, Func<T> acquire)
        {
            return Get(cacheManager, key, 60, acquire);
        }
        /// <summary>
        /// 获取对象，如对象不存在则进行缓存并指定缓存过期时间
        /// </summary>
        public static T Get<T>(this ICacheManager cacheManager, string key, int cacheTime, Func<T> acquire) 
        {
            if (cacheManager.IsSet(key))
            {
                return cacheManager.Get<T>(key);
            }
            else
            {
                var result = acquire();
                //if (result != null)
                    cacheManager.Set(key, result, cacheTime);
                return result;
            }
        }
    }
}
