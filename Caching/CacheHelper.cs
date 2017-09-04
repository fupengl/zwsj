using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZhiDiExt.Caching
{
    public class CacheHelper
    {
        private static ICacheManager _cacheManager;

        public CacheHelper()
        {
        }

        public static ICacheManager GetCacheManager()
        {
            if (_cacheManager == null)
            {
                _cacheManager=new MemoryCacheManager();
            }

            return _cacheManager;
        }
    }
}