using Autofac;
using Microsoft.Extensions.Logging;
using Surging.Core.Caching;
using Surging.Core.Caching.Configurations;
using Surging.Core.CPlatform;
using Surging.Core.CPlatform.Configurations;
using Surging.Core.CPlatform.Utilities;
using Surging.Core.Nlog;
using Surging.Core.ProxyGenerator;
using Surging.Core.ServiceHosting;
using Surging.Core.ServiceHosting.Internal.Implementation;
using surgingDemo.Data.Models;
using System;
using System.Text;

namespace surgingDemo.Sevices.server
{
    class Program
    {
        static void Main(string[] args)
        {

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var host = new ServiceHostBuilder()
                .RegisterServices(builder =>
                {
                    builder.AddMicroService(option =>
                    {
                        option.AddClient()
                        .AddCache();
                        builder.Register(p => new CPlatformContainer(ServiceLocator.Current));
                    });
                })
                .Configure(build =>
                build.AddCacheFile("cacheSettings.json", optional: false, reloadOnChange: true))
                .Configure(build =>
                build.AddCPlatformFile("${surgingpath}|surgingSettings.json", optional: false, reloadOnChange: true))
                .UseNLog(LogLevel.Error)
                .UseClient()
                .UseConsoleLifetime()
                .UseStartup<Startup>()
                .Build();
            //在启动的时候吧连接字符串赋值
            testContext.ConnectionString = Surging.Core.CPlatform.AppConfig.GetSection("ConnectionStrings").GetSection("SqlServerStr").Value;
            using (host.Run())
            {
                Console.WriteLine($"服务端启动成功，{DateTime.Now}。");
            }
        }
    }
}
