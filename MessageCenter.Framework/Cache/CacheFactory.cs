#if NETSTANDARD1_3 || NETSTANDARD2_0
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageCenter.Framework.Cache
{
    public static class CacheFactory
    {
        private static Dictionary<string, ICache> s_CacheProviders = new Dictionary<string, ICache>();
        public static void Init(IMemoryCache cache)
        {
            s_CacheProviders.Add("MemoryCache", new MemoryCacheWapper(cache));
        }

        public static ICache GetInstance()
        {
            return CacheFactory.GetInstance(null);
        }
        public static ICache GetInstance(string name)
        {
            name = name ?? "MemoryCache";
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new KeyNotFoundException("The default cache name is not configured in config file.");
            }
            if (CacheFactory.s_CacheProviders.ContainsKey(name))
            {
               return CacheFactory.s_CacheProviders[name];
            }
            throw new NotSupportedException($"The cache named {name} is not configured in config file.");
        }
    }
}
#endif
