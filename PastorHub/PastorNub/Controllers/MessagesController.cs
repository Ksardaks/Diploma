using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using PastorNub.Models;

namespace PastorNub.Controllers
{
    public class MessagesController : Controller
    {
        // GET: Chat
        [Authorize]
        public ActionResult Index()
        {
            ApplicationDbContext Context = new ApplicationDbContext();

            ApplicationUser CurrentUser = Context.Users.Find(User.Identity.GetUserId());

            return View(Context.ChatRooms.Where(i => i.FirstUser.Id == CurrentUser.Id || i.SecondUser.Id == CurrentUser.Id).ToList());
        }

        [Authorize]
        public ActionResult Chat(string Id)
        {
            ApplicationDbContext Context = new ApplicationDbContext();

            ApplicationUser CurrentUser = Context.Users.Find(User.Identity.GetUserId());
            ApplicationUser TargetUser = Context.Users.Find(Id);
            if(CurrentUser == null || TargetUser == null)
            {
                return HttpNotFound();
            }
            if(CurrentUser.Pastor == null && TargetUser.Pastor == null)
            {
                return HttpNotFound();
            }
            ChatRoom TargetChatRoom;

            var md5 = MD5.Create();
            string FirstKey = CurrentUser.Id + TargetUser.Id;
            string SecondKey = TargetUser.Id + CurrentUser.Id;
            var FirstHash = md5.ComputeHash(Encoding.UTF8.GetBytes(FirstKey));
            var SecondHash = md5.ComputeHash(Encoding.UTF8.GetBytes(SecondKey));

            FirstKey = Convert.ToBase64String(FirstHash);
            SecondKey = Convert.ToBase64String(SecondHash);

            if (Context.ChatRooms.Where(i => i.Key == FirstKey).Count() > 0)
            {
                TargetChatRoom = Context.ChatRooms.Where(i => i.Key == FirstKey).FirstOrDefault();
            }
            else if (Context.ChatRooms.Where(i => i.Key == SecondKey).Count() > 0)
            {
                TargetChatRoom = Context.ChatRooms.Where(i => i.Key == SecondKey).FirstOrDefault();
            }
            else
            {
                ChatRoom NewChatRoom = new ChatRoom();

                NewChatRoom.Key = FirstKey;
                NewChatRoom.FirstUser = CurrentUser;
                NewChatRoom.SecondUser = TargetUser;

                Context.ChatRooms.Add(NewChatRoom);

                Context.SaveChanges();

                TargetChatRoom = NewChatRoom;
            }

            return View(TargetChatRoom);
        }

        [Authorize]
        public ActionResult WriteMessage(int Id, string Text)
        {
            ApplicationDbContext Context = new ApplicationDbContext();

            ApplicationUser CurrentUser = Context.Users.Find(User.Identity.GetUserId());
            ChatRoom CurrentChat = Context.ChatRooms.Find(Id);

            ChatMessage NewChatMessage = new ChatMessage();
            NewChatMessage.User = CurrentUser;
            NewChatMessage.Chat = CurrentChat;
            NewChatMessage.Date = DateTime.Now;
            NewChatMessage.Message = Text.Replace("\r\n", "<br/>");

            CurrentChat.ChatMessages.Add(NewChatMessage);

            Context.SaveChanges();

            return PartialView(CurrentChat);
        }
        
    }
}