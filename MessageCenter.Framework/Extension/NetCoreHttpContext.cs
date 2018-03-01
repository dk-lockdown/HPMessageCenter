#if NETSTANDARD1_3 || NETSTANDARD2_0
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageCenter.Framework.Extension
{
    public class NetCoreHttpContext
    {
        private static IHttpContextAccessor _accessor;

        private static HttpContext _current;

        public static HttpContext Current
        {
            get
            {
                if (_current == null)
                {
                    return _accessor.HttpContext;
                }
                return _current;
            }
            set { _current = value; }
        }

        internal static void Configure(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }
    }

    public static class StaticHttpContextExtensions
    {
        public static void AddHttpContextAccessor(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        public static IApplicationBuilder UseStaticHttpContext(this IApplicationBuilder app)
        {
            var httpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
            NetCoreHttpContext.Configure(httpContextAccessor);
            return app;
        }
    }
}
#endif
