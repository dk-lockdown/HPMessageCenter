#if NETSTANDARD1_3 || NETSTANDARD2_0
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageCenter.Framework.Cache
{
    public static class CacheManager
    {
        private const string CACHE_LOCKER_PREFIX = "C_L_";
        public static void RemoveFromCache(string cacheName, string cacheKey)
        {
            ICache instance = CacheFactory.GetInstance(cacheName);
            string text = "C_L_N:" + cacheName + cacheKey;
            lock (text)
            {
                instance.Remove(cacheKey);
            }
        }
        public static void RemoveFromLocalCache(string cacheKey)
        {
            string text = "C_L_" + cacheKey;
            lock (text)
            {
                CacheFactory.GetInstance().Remove(cacheKey);
            }
        }
        public static T GetWithCache<T>(string cacheName, string cacheKey, Func<T> getter, bool absoluteExpiration = true, int cacheExpirationMinutes = 30) where T : class
        {
            ICache instance = CacheFactory.GetInstance(cacheName);
            T t = instance.Get(cacheKey) as T;
            T result;
            if (t != null)
            {
                result = t;
            }
            else
            {
                string text = "C_L_N:" + cacheName + cacheKey;
                lock (text)
                {
                    t = (instance.Get(cacheKey) as T);
                    if (t != null)
                    {
                        result = t;
                    }
                    else
                    {
                        t = getter();
                        if (absoluteExpiration)
                        {
                            instance.Add(cacheKey, t, System.TimeSpan.FromMinutes((double)cacheExpirationMinutes));
                        }
                        else
                        {
                            instance.Add(cacheKey, t, System.TimeSpan.FromMinutes((double)cacheExpirationMinutes),true);
                        }
                        result = t;
                    }
                }
            }
            return result;
        }
        public static T GetWithLocalCache<T>(string cacheKey, Func<T> getter, bool absoluteExpiration = true, int cacheExpirationMinutes = 30) where T : class
        {
            T t = CacheFactory.GetInstance().Get(cacheKey) as T;
            T result;
            if (t != null)
            {
                result = t;
            }
            else
            {
                string text = "C_L_" + cacheKey;
                lock (text)
                {
                    t = (CacheFactory.GetInstance().Get(cacheKey) as T);
                    if (t != null)
                    {
                        result = t;
                    }
                    else
                    {
                        t = getter();
                        if (absoluteExpiration)
                        {
                            CacheFactory.GetInstance().Add(cacheKey, t, System.TimeSpan.FromMinutes((double)cacheExpirationMinutes));
                        }
                        else
                        {
                            CacheFactory.GetInstance().Add(cacheKey, t, System.TimeSpan.FromMinutes((double)cacheExpirationMinutes),true);

                        }
                        result = t;
                    }
                }
            }
            return result;
        }
    }
}
#endif