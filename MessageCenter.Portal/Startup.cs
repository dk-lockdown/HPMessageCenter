using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Newtonsoft.Json.Serialization;
using MessageTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyModel.Resolution;
using Microsoft.Extensions.DependencyModel;
using Microsoft.DotNet.PlatformAbstractions;
using System.IO;
using System.Reflection;
using MessageCenter.BLL;
using MessageCenter.Framework.Extension;
using MessageCenter.Framework.Cache;
using MessageTransit.Monitor;
using MessageTransit.Message;

namespace MessageCenter.Portal
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();

            services.AddHttpContextAccessor();

            services.AddAuthentication(AuthenticationConfig.AuthenticationKey)
                    .AddCookie(AuthenticationConfig.AuthenticationKey, options =>
                    {
                        options.AccessDeniedPath = new PathString("/Account/Login");
                        options.LoginPath = new PathString("/Account/Login");
                        options.Events = new CookieAuthenticationEvents
                        {
                            OnSignedIn = context =>
                            {
                                Console.WriteLine("{0} - {1}: {2}", DateTime.Now,
                                  "OnSignedIn", context.Principal.Identity.Name);
                                return Task.CompletedTask;
                            },
                            OnSigningOut = context =>
                            {
                                Console.WriteLine("{0} - {1}: {2}", DateTime.Now,
                                  "OnSigningOut", context.HttpContext.User.Identity.Name);
                                return Task.CompletedTask;
                            },
                            OnValidatePrincipal = context =>
                            {
                                Console.WriteLine("{0} - {1}: {2}", DateTime.Now,
                                  "OnValidatePrincipal", context.Principal.Identity.Name);
                                return Task.CompletedTask;
                            }
                        };
                    });

            //分布式部署
            services.AddDataProtection()
                .SetApplicationName("MessageCenter")
                .AddKeyManagementOptions(options =>
                {
                    options.XmlRepository = new XmlRepository();
                });
            var mvcBuilder = services.AddMvc(options =>
            {
                options.Filters.Add(typeof(WebHandleExceptionAttribute)); // an instance
            }).AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());//解决返回json数据首字母小写的问题
            services.AddOptions().Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            new MvcConfiguration().ConfigureMvc(mvcBuilder);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IMemoryCache cache)
        {
            CacheFactory.Init(cache);
            TopicConfiguratorGeter.Init(new SqlTopicConfigurator());
            MessageCenter.Startup.Init(new SqlMonitor());
            MessageCenterManager.Init();

            loggerFactory.AddConsole();
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStatusCodePages();

            app.UseAuthentication();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }

    public class MvcConfiguration : IDesignTimeMvcBuilderConfiguration
    {
        private class DirectReferenceAssemblyResolver : ICompilationAssemblyResolver
        {
            public bool TryResolveAssemblyPaths(CompilationLibrary library, List<string> assemblies)
            {
                if (!string.Equals(library.Type, "reference", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                var paths = new List<string>();

                foreach (var assembly in library.Assemblies)
                {
                    var path = Path.Combine(ApplicationEnvironment.ApplicationBasePath, assembly);

                    if (!File.Exists(path))
                    {
                        return false;
                    }

                    paths.Add(path);
                }

                assemblies.AddRange(paths);

                return true;
            }
        }

        public void ConfigureMvc(IMvcBuilder builder)
        {
            // .NET Core SDK v1 does not pick up reference assemblies so
            // they have to be added for Razor manually. Resolved for
            // SDK v2 by https://github.com/dotnet/sdk/pull/876 OR SO WE THOUGHT
            /*builder.AddRazorOptions(razor =>
            {
            razor.AdditionalCompilationReferences.Add(
            MetadataReference.CreateFromFile(
            typeof(PdfHttpHandler).Assembly.Location));
            });*/

            // .NET Core SDK v2 does not resolve reference assemblies‘ paths
            // at all, so we have to hack around with reflection
            typeof(CompilationLibrary)
            .GetTypeInfo()
            .GetDeclaredField("<DefaultResolver>k__BackingField")
            .SetValue(null, new CompositeCompilationAssemblyResolver(new ICompilationAssemblyResolver[]
            {
                new DirectReferenceAssemblyResolver(),
                new AppBaseCompilationAssemblyResolver(),
                new ReferenceAssemblyPathResolver(),
                new PackageCompilationAssemblyResolver(),
            }));
        }
    }

    public class SqlMonitor : IMonitor
    {
        public void onEvent(MessageEventArgs e)
        {
            if(e is MessageSuccessEventArgs)
            {
                MessageSuccessEventArgs args = e as MessageSuccessEventArgs;
                MessageSvc.UpdateMessageStatusToSuccess(Guid.Parse(args.message.Headers[BuiltinKeys.TraceId]), args.message.Headers[BuiltinKeys.Topic], args.milliseconds);
            }
            else if(e is MessageExceptionEventArgs)
            {
                MessageExceptionEventArgs args = e as MessageExceptionEventArgs;
                MessageSvc.UpdateMessageStatusToFail(new ProcessFailRecord()
                {
                    MessageId = Guid.Parse(args.message.Headers[BuiltinKeys.TraceId]),
                    Topic = args.message.Headers[BuiltinKeys.Topic],
                    FailRecord = args.ex.Message,
                    TimePeriod = args.milliseconds
                });

                #region 邮件通知


                #endregion
            }
        }
    }

    public class SqlTopicConfigurator : ITopicConfigurator
    {
        public string GetExchange(string topic)
        {
            return TopicSvc.LoadTopicByTopicName(topic).ExchangeName;
        }

        public string GetProcessorConfig(string topic)
        {
            return TopicSvc.LoadTopicByTopicName(topic).ProcessorConfig;
        }
    }
}
