using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using FizzWare.NBuilder;
using CrossChat.Domain.DBModel;
using Microsoft.AspNetCore.Http;

namespace CrossChat.DataAccess
{
    public partial class CrossChatContext : DbContext //, ICrossChatContext
    {
      
        public DbSet<User> Users { get; set; }
        public DbSet<LoginHistory> LoginHistory { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<MemberShip> MemberShip { get; set; }
        public DbSet<Message> Message { get; set; }
        public DbSet<MessageType> MessageType { get; set; }

        IHttpContextAccessor _httpContextAccessor;
        public CrossChatContext(DbContextOptions<CrossChatContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            Database.EnsureCreated();
        }
        public override int SaveChanges()
        {
            AddTimestamps();
            return base.SaveChanges();
        }
       
        //override of savechange to fill date created , date modified , ets.

        private void AddTimestamps()
        {
            var entities = ChangeTracker.Entries().Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));
            Guid currentUsername=new Guid();
            if (_httpContextAccessor.HttpContext!=null && _httpContextAccessor.HttpContext.User!=null && _httpContextAccessor.HttpContext.User.Claims.Count()>0 && _httpContextAccessor.HttpContext.User.Claims.First(t => t.Type == "UserId") != null)
                currentUsername = new Guid(_httpContextAccessor.HttpContext.User.Claims.First(t => t.Type == "UserId").Value);
            else if(entities.Count()>0)
                currentUsername = ((BaseEntity)entities.First().Entity).Id;

            foreach (var entity in entities)
            {
                if (entity.State == EntityState.Added)
                {
                    ((BaseEntity)entity.Entity).DateCreated = DateTime.Now;
                    ((BaseEntity)entity.Entity).UserCreated = currentUsername;
                    ((BaseEntity)entity.Entity).IsActive = true;
                    ((BaseEntity)entity.Entity).IsDeleted = false;
                }

                ((BaseEntity)entity.Entity).DateModified = DateTime.UtcNow;
                ((BaseEntity)entity.Entity).UserModified = currentUsername;
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //forginKey
            modelBuilder.Entity<User>().HasMany(a => a.SendedMessage).WithOne(a => a.SourceUser).HasForeignKey(a => a.SourceUserId).IsRequired();
            modelBuilder.Entity<User>().HasMany(a => a.ReciveMessage).WithOne(a => a.DestinationUser).HasForeignKey(a => a.DestinationUserId);

            //indexes
            modelBuilder.Entity<User>().HasIndex(b => b.Email).IsUnique();
            modelBuilder.Entity<User>().HasIndex(b => b.Name);
            modelBuilder.Entity<User>().HasIndex(b => b.Surname);
            modelBuilder.Entity<User>().HasIndex(b => b.DateCreated);
            modelBuilder.Entity<User>().HasIndex(b => b.DateModified);

            modelBuilder.Entity<Channel>().HasIndex(b => b.ChannelName).IsUnique();
            modelBuilder.Entity<Channel>().HasIndex(b => b.DateCreated);
            modelBuilder.Entity<Channel>().HasIndex(b => b.DateModified);

            modelBuilder.Entity<Message>().HasIndex(b => b.DateCreated);
            modelBuilder.Entity<Message>().HasIndex(b => b.DateModified);

            modelBuilder.Entity<MemberShip>().HasIndex(b => b.DateCreated);
            modelBuilder.Entity<MemberShip>().HasIndex(b => b.DateModified);
        }
    }

}





