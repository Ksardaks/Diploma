using Microsoft.AspNet.Identity;
using PastorNub.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PastorNub.Controllers
{
    public class ProfileController : Controller
    {
        public ActionResult Index(string Id)
        {
            ApplicationDbContext Context = new ApplicationDbContext();
            ApplicationUser User = Context.Users.Find(Id);
            if (User != null)
            {
                if (User.Pastor != null)
                {
                    return View("PastorProfile", User);
                }
                else
                {
                    return View("UserProfile", User);
                }
            }
            return HttpNotFound();
        }

        [HttpPost]
        public ActionResult Search(string Text)
        {
            ApplicationDbContext Context = new ApplicationDbContext();
            ApplicationUser CurrentUser = Context.Users.Find(User.Identity.GetUserId());
            List<Post> PostList = Context.Posts.Where(i => i.Autor.Id == CurrentUser.Id && (i.Text.Contains(Text) || i.Title.Contains(Text))).ToList();

            return PartialView(PostList);
        }

        public ActionResult ConfessionError()
        {
            return View();
        }

        [Authorize(Roles = "pastor")]
        public ActionResult PostManager()
        {
            ApplicationDbContext Context = new ApplicationDbContext();
            ApplicationUser CurrentUser = Context.Users.Find(User.Identity.GetUserId());
            if (CurrentUser.Confession != null || User.IsInRole("administrator"))
            {
                return View(Context.Posts.Where(i => i.Autor.Id == CurrentUser.Id).ToList());
            }
            else
            {
                return RedirectToAction("ConfessionError");
            }
        }

        public ActionResult CreatePost()
        {
            return PartialView();
        }

        [HttpGet]
        public ActionResult EditPost(int Id)
        {
            ApplicationDbContext Contex = new ApplicationDbContext();
            Post Post = Contex.Posts.Find(Id);
            Post.Text = Post.Text.Replace("<br/>", "\r\n");
            return PartialView(Post);
        }

        public List<Models.File> AddFiles(IEnumerable<HttpPostedFileBase> FilesToAdd, Post PostForAdd)
        {
            List<Models.File> Files = new List<Models.File>();
            if (FilesToAdd != null)
            {
                ApplicationDbContext Context = new ApplicationDbContext();
                string FileName;
                int Counter = Context.Files.ToList().Last().Id;
                foreach (var FileToSave in FilesToAdd)
                {
                    if (FileToSave != null)
                    {
                        int PostId = Context.Posts.Count() == 0 ? 1 : Context.Posts.ToList().Last().Id;
                        FileName = "Post" + PostId + "User" + User.Identity.GetUserId() + "File" + Counter + Path.GetExtension(FileToSave.FileName);

                        FileToSave.SaveAs(Server.MapPath("~/Files/Post/" + FileName));

                        Files.Add(new Models.File { FileName = FileName, Post = PostForAdd });
                    }
                    Counter++;
                }
            }
            return Files;
        }

        [HttpPost]
        [Authorize(Roles = "pastor")]
        public ActionResult AddPost(string Title, string Text, IEnumerable<HttpPostedFileBase> Files, bool Published)
        {
            ApplicationDbContext Context = new ApplicationDbContext();
            if(string.IsNullOrEmpty(Title))
            {
                ModelState.AddModelError("Title", "Назва поста не введена");
            }

            if(ModelState.IsValid)
            {
                ApplicationUser CurrendUser = Context.Users.Find(User.Identity.GetUserId());
                Text = Text.Replace("\r\n", "<br/>");
                Post NewPost = new Post() { Title = Title, Text = Text, Autor = CurrendUser, Date = DateTime.Now, Published = Published };

                NewPost.Files.AddRange(AddFiles(Files, NewPost));

                if (User.IsInRole("administrator"))
                {
                    NewPost.Global = true;
                }
                else
                {
                    NewPost.Confession = CurrendUser.Confession;
                }

                Context.Posts.Add(NewPost);
                Context.SaveChangesAsync();

                return RedirectToAction("PostManager");
            }

           return RedirectToAction("PostManager");
        }

        [HttpPost]
        [Authorize(Roles = "pastor")]
        public ActionResult EditPost(int Id, string Title, string Text, IEnumerable<HttpPostedFileBase> Files)
        {
            if (string.IsNullOrEmpty(Title))
            {
                ModelState.AddModelError("Title", "Назва поста не введена");
            }

            if(ModelState.IsValid)
            {
                ApplicationDbContext Context = new ApplicationDbContext();
                Post PostForEdit = Context.Posts.Find(Id);
                PostForEdit.Title = Title;
                Text = Text.Replace("\r\n", "<br/>");
                PostForEdit.Text = Text;

                PostForEdit.Files.AddRange(AddFiles(Files, PostForEdit));

                Context.SaveChanges();

                return RedirectToAction("PostManager");
            }
            return RedirectToAction("PostManager");
        }

        public ActionResult DeleteFile(int FileId, int PostId)
        {
            ApplicationDbContext Contex = new ApplicationDbContext();

            System.IO.File.Delete(Server.MapPath("~/Files/Post/" + Contex.Files.Where(i => i.Id == FileId).FirstOrDefault().FileName));
            Contex.Files.Remove(Contex.Files.Where(i => i.Id == FileId).FirstOrDefault());
            Contex.SaveChanges();

            return PartialView(Contex.Posts.Find(PostId));
        }

        public ActionResult DeletePost(int Id)
        {
            ApplicationDbContext Contex = new ApplicationDbContext();

            Post PostToDelete = Contex.Posts.Where(i => i.Id == Id).FirstOrDefault();

            if (PostToDelete.Files != null)
            {
                foreach (var FileToDelete in PostToDelete.Files)
                {
                    System.IO.File.Delete(Server.MapPath("~/Files/Post/" + FileToDelete.FileName));
                }
            }

            Contex.Posts.Remove(PostToDelete);
            Contex.SaveChanges();

            return RedirectToAction("PostManager");
        }

        public ActionResult DePublished(int Id)
        {
            ApplicationDbContext Contex = new ApplicationDbContext();

            Post PostToDePublish = Contex.Posts.Where(i => i.Id == Id).FirstOrDefault();
            PostToDePublish.Published = false;
            Contex.SaveChanges();

            return RedirectToAction("PostManager");
        }

        public ActionResult Published(int Id)
        {
            ApplicationDbContext Contex = new ApplicationDbContext();

            Post PostToPublish = Contex.Posts.Where(i => i.Id == Id).FirstOrDefault();
            PostToPublish.Published = true;
            Contex.SaveChanges();

            return RedirectToAction("PostManager");
        }

        [Authorize]
        public ActionResult Follow(string Id)
        {
            ApplicationDbContext Contex = new ApplicationDbContext();

            ApplicationUser CurrentUser = Contex.Users.Find(User.Identity.GetUserId());
            ApplicationUser Pastor = Contex.Users.Find(Id);


            if (Contex.Subscriptions.Where(i => i.User.Id == CurrentUser.Id && i.Pastor.Id == Pastor.Pastor.Id).Count() <= 0)
            {
                Subscriptions NewSubscriptions = new Subscriptions() { User = CurrentUser, Pastor = Pastor.Pastor };

                CurrentUser.Subscriptions.Add(NewSubscriptions);
                Pastor.Pastor.Subscribers.Add(NewSubscriptions);

                Contex.SaveChanges();
            }

            return PartialView(Pastor);
        }

        public ActionResult UnFollow(string Id)
        {
            ApplicationDbContext Contex = new ApplicationDbContext();
            ApplicationUser CurrentUser = Contex.Users.Find(User.Identity.GetUserId());
            ApplicationUser Pastor = Contex.Users.Find(Id);

            if (Contex.Subscriptions.Where(i => i.User.Id == CurrentUser.Id && i.Pastor.Id == Pastor.Pastor.Id).Count() == 1)
            {
                Subscriptions SubscriptionToDelete = Contex.Subscriptions.Where(i => i.User.Id == CurrentUser.Id && i.Pastor.Id == Pastor.Pastor.Id).FirstOrDefault();
                CurrentUser.Subscriptions.Remove(SubscriptionToDelete);
                Pastor.Pastor.Subscribers.Remove(SubscriptionToDelete);
                Contex.Subscriptions.Remove(SubscriptionToDelete);

                Contex.SaveChanges();
            }
            return PartialView(Pastor);
        }
    }
}