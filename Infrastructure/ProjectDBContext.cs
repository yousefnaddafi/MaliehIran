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
        public DbSet<UserType> UserTypes { get; set; }
        public DbSet<Media> Medias { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageRecipient> MessageRecipients { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }


        public ProjectDBContext(DbContextOptions<ProjectDBContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            modelbuilder.Entity<User>().ToTable("Users", "dbo").HasKey(z => z.UserId);
            modelbuilder.Entity<UserType>().ToTable("UserTypes", "dbo").HasKey(z => z.UserTypeId);
            modelbuilder.Entity<Media>().ToTable("Medias", "dbo").HasKey(z => z.MediaId);
            modelbuilder.Entity<Message>().ToTable("Medias", "dbo").HasKey(z => z.MessageId);
            modelbuilder.Entity<MessageRecipient>().ToTable("Medias", "dbo").HasKey(z => z.MessageRecipientId);
            modelbuilder.Entity<Group>().ToTable("Groups", "dbo").HasKey(z => z.GroupId);
            modelbuilder.Entity<UserGroup>().ToTable("UserGroups", "dbo").HasKey(z => z.UserGroupId);

            modelbuilder.Entity<User>().Property(b => b.CreateDate).HasDefaultValueSql("getdate()");
        }
    }
}
