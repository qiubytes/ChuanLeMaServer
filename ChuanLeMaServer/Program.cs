
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;

namespace ChuanLeMaServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
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

            // 3. 配置 Autofac 容器
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
