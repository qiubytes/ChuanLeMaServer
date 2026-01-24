using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository
{
    /// <summary>
    /// 设计时工厂  迁移命令使用
    /// </summary>
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // 方法1：硬编码连接字符串（简单但不推荐生产）
            //var connectionString = "Server=localhost;Port=3306;Database=MyDb;User=root;Password=123456;";
            // var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
            // 方法2：从配置文件读取（推荐）
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString),
                mysqlOptions => mysqlOptions.MigrationsAssembly("Repository")
            );

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
