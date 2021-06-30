using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace PastorNub.Models
{
    // В профиль пользователя можно добавить дополнительные данные, если указать больше свойств для класса ApplicationUser. Подробности см. на странице https://go.microsoft.com/fwlink/?LinkID=317594.
    public class ApplicationUser : IdentityUser
    {

        public string Name { get; set; }
        public string Surname { get; set; }
        public string MiddleName { get; set; }
        public virtual Confession Confession { get; set; }
        public virtual Pastor Pastor { get; set; }
        public string Born { get; set; }
        public string Avatar { get; set; }
        public virtual List<Subscriptions> Subscriptions { get; set; }
        public virtual List<UserConfession> UserConfessions { get; set; }
        public virtual List<Favorite> Favorites { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Обратите внимание, что authenticationType должен совпадать с типом, определенным в CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Здесь добавьте утверждения пользователя
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public DbSet<Pastor> Pastors { get; set; }
        public DbSet<Confession> Confessions { get; set; }
        public DbSet<UserConfession> UserConfessions { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<Subscriptions> Subscriptions { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Comments> Comments { get; set; }
        public DbSet<ChatRoom> ChatRooms { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }

    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public bool Published { get; set; }
        public bool Global { get; set; }
        public virtual ApplicationUser Autor { get; set; }
        public virtual Confession Confession { get; set; }
        public virtual List<File> Files { get; set; }
        public virtual List <Favorite> Favorites { get; set; }
        public virtual List<Comments> Comments { get; set; }

        public Post()
        {
            Files = new List<File>();
            Favorites = new List<Favorite>();
            Comments = new List<Comments>();
        }
    }

    public class Comments
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public virtual ApplicationUser Autor { get; set; }
        public virtual Post Post { get; set; }
    }

    public class File
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public virtual Post Post { get; set; }
    }

    public class Confession
    {
        public int Id { get; set; }
        public string ConfessionName { get; set; }
    }

    public class UserConfession
    {
        public int Id { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual Confession Confession { get; set; }
    }

    public class Pastor
    {
        public int Id { get; set; }
        public string Education { get; set; }

        public virtual List<Subscriptions> Subscribers { get; set; }

        [Required]
        public virtual ApplicationUser User { get; set; }

        public Pastor()
        {
            Subscribers = new List<Subscriptions>();
        }

    }

    public class Subscriptions
    {
        public int Id { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual Pastor Pastor { get; set; }
    }

    public class Favorite
    {
        public int Id { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual Post Post { get; set; }
    }

    public class ChatRoom
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public virtual ApplicationUser FirstUser { get; set; }
        public virtual ApplicationUser SecondUser { get; set; }
        public virtual List<ChatMessage> ChatMessages { get; set; }

        public ChatRoom()
        {
            ChatMessages = new List<ChatMessage>();
        }
    }

    public class ChatMessage
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public virtual ChatRoom Chat { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}