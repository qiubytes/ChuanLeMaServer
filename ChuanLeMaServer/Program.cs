
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Repository;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Reflection;

namespace ChuanLeMaServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // 配置 Kestrel 服务器
            builder.WebHost.ConfigureKestrel(serverOptions =>
            {
                serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(20);
                serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(10);
                serverOptions.Limits.MaxRequestBodySize = 1024L * 1024L * 1024L * 10L; // 10GB
                serverOptions.Limits.MinRequestBodyDataRate = null; // 禁用最小数据速率限制
                serverOptions.Limits.MinResponseDataRate = null;    // 禁用响应数据速率限制  
                // 针对上传单独配置
                serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(60);

                // 增加并发连接数
                serverOptions.Limits.MaxConcurrentConnections = 1000;
                serverOptions.Limits.MaxConcurrentUpgradedConnections = 1000;
            });
            // 配置表单选项
            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 10L * 1024 * 1024 * 1024; // 10GB
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartHeadersLengthLimit = int.MaxValue;
                options.MultipartBoundaryLengthLimit = int.MaxValue;
                options.BufferBodyLengthLimit = 10L * 1024 * 1024 * 1024; // 10GB
                options.MemoryBufferThreshold = 1024 * 1024 * 10; // 10MB
                options.BufferBody = true;
            });

            builder.WebHost.UseUrls("http://*:5210");
            // 1、配置 Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("System", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} " +
                                   "{Properties:j}{NewLine}{Exception}")
                .WriteTo.File(
                    path: "logs/log-.txt",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            //builder.Host.UseSerilog();
            // 2. 使用 Autofac 作为服务容器
            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .UseSerilog();
            // 3. 添加 DbContext 到内置容器（会被Autofac接管）
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString)
                )
                // 启用敏感数据日志（包含参数值）
                .EnableSensitiveDataLogging()
                // 启用详细错误
                .EnableDetailedErrors()
                // 配置日志
                .LogTo(Console.WriteLine, LogLevel.Information) 
                );
            // 4. 配置 Autofac 容器
            builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
            {
                // 注册控制器（支持属性注入）
                //containerBuilder.RegisterType<Controllers.ProductsController>()
                //    .PropertiesAutowired();

                // 注册服务
                //containerBuilder.RegisterType<ProductService>()
                //    .As<IProductService>()
                //    .InstancePerLifetimeScope();

                // 注册 Serilog ILogger
                // 正确注册 Microsoft.Extensions.Logging.ILoggerFactory
                containerBuilder.RegisterInstance(new SerilogLoggerFactory(Log.Logger, true))
                    .As<ILoggerFactory>()
                    .SingleInstance();

                // 注册泛型 ILogger<T>
                containerBuilder.RegisterGeneric(typeof(Logger<>))
                    .As(typeof(ILogger<>))
                    .SingleInstance();

                // 注册泛型 ILogger<T>
                //containerBuilder.RegisterGeneric(typeof(Logger<>))
                //    .As(typeof(ILogger<>))
                //    .SingleInstance();

                // 注册其他服务
                //containerBuilder.RegisterModule(new ServiceModule());

                //注册仓储实现
                containerBuilder.RegisterAssemblyTypes(typeof(AppDbContext).Assembly)
                                .Where(t => t.Name.EndsWith("RepositoryImpl"))
                                .AsImplementedInterfaces()
                                .InstancePerLifetimeScope();
                //注册服务实现
                var assembly = Assembly.Load("Service");  // 指定程序集名称（不带 .dll）
                containerBuilder.RegisterAssemblyTypes(assembly)
                    .Where(t => t.Name.EndsWith("ServiceImpl"))
                    .AsImplementedInterfaces()
                    .InstancePerLifetimeScope();

            });
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/openapi/v1.json", "v1"); 
                });
            }

            app.UseAuthorization();


            app.MapControllers();

            //// 添加请求日志中间件
            //app.UseSerilogRequestLogging(options =>
            //{
            //    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
            //    options.GetLevel = (httpContext, elapsed, ex) =>
            //        ex != null ? LogEventLevel.Error :
            //        httpContext.Response.StatusCode > 499 ? LogEventLevel.Error : LogEventLevel.Information;
            //});

            //// 确保在应用关闭时刷新日志
            //app.Lifetime.ApplicationStopped.Register(Log.CloseAndFlush);

            app.Run();
        }
    }
}
