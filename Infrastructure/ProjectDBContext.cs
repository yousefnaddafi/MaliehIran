using MaliehIran.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaliehIran.Infrastructure
{
    public class ProjectDBContext : DbContext
    {
        public ProjectDBContext()
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Market> Markets { get; set; }
        public DbSet<UserType> UserTypes { get; set; }
        public DbSet<Media> Medias { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Cron> Crons { get; set; }
        public DbSet<CryptoAccount> CryptoAccounts { get; set; }


        public ProjectDBContext(DbContextOptions<ProjectDBContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            modelbuilder.Entity<User>().ToTable("Users", "dbo").HasKey(z => z.UserId);
            modelbuilder.Entity<Market>().ToTable("Markets", "dbo").HasKey(z => z.MarketId);
            modelbuilder.Entity<UserType>().ToTable("UserTypes", "dbo").HasKey(z => z.UserTypeId);
            modelbuilder.Entity<Media>().ToTable("Medias", "dbo").HasKey(z => z.MediaId);
            modelbuilder.Entity<Order>().ToTable("Orders", "dbo").HasKey(z => z.OrderId);
            modelbuilder.Entity<Cron>().ToTable("Crons", "dbo").HasKey(z => z.CronId);
            modelbuilder.Entity<CryptoAccount>().ToTable("CryptoAccounts", "dbo").HasKey(z=>z.CryptoAccountId);

            modelbuilder.Entity<User>().Property(b => b.CreateDate).HasDefaultValueSql("getdate()");
            modelbuilder.Entity<Market>().Property(b => b.CreateDate).HasDefaultValueSql("getdate()");
            modelbuilder.Entity<Cron>().Property(b => b.CreateDate).HasDefaultValueSql("getdate()");
            modelbuilder.Entity<Order>().Property(b => b.CreateDate).HasDefaultValueSql("getdate()");
            modelbuilder.Entity<CryptoAccount>().Property(b => b.CreateDate).HasDefaultValueSql("getdate()");
            modelbuilder.Entity<Order>().Property(b => b.MainOrderPrice).HasColumnType("decimal(11, 8)");
            modelbuilder.Entity<Order>().Property(b=>b.MainOrderPrice).HasPrecision(11,8);
            modelbuilder.Entity<Order>().Property(b => b.RiskAversion).HasColumnType("decimal(11, 8)");
            modelbuilder.Entity<Order>().Property(b => b.RiskAversion).HasPrecision(11, 8);
            modelbuilder.Entity<Order>().Property(b => b.Percent).HasColumnType("decimal(11, 8)");
            modelbuilder.Entity<Order>().Property(b => b.Percent).HasPrecision(11, 8);
        }
    }
}
