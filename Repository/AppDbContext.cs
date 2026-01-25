using Microsoft.EntityFrameworkCore;
using Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository
{
    public class AppDbContext : DbContext
    {
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<AppUserToken> AppUserTokens { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // 配置 AppUser 实体
            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(20);
                entity.Property(e => e.Password)
                .IsRequired()
                .HasMaxLength(32);//md5 长度32 个16进制字符
            });
            modelBuilder.Entity<AppUserToken>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId)
                    .IsRequired();
                entity.Property(e => e.Token)
                    .IsRequired();
                entity.Property(e => e.AddTime)
                    .HasColumnType("datetime")
                    .IsRequired();
            });
        }
    }
}
