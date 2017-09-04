using System;

namespace ZhiDiExt.Caching
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class CacheExtensions
    {
        /// <summary>
        /// ��ȡ��������󲻴�������л��沢ָ��Ĭ�ϵĻ������ʱ��
        /// </summary>
        public static T Get<T>(this ICacheManager cacheManager, string key, Func<T> acquire)
        {
            return Get(cacheManager, key, 60, acquire);
        }
        /// <summary>
        /// ��ȡ��������󲻴�������л��沢ָ���������ʱ��
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
