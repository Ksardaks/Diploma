using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PastorNub.Models;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace PastorNub.Controllers
{
    public class CommonController : Controller
    {
        static string SearchText = "";
        static bool OnlyFavorite = false;

        public ActionResult Search(string Id = "", string Text = "")
        {
            ApplicationDbContext Context = new ApplicationDbContext();
            ApplicationUser CurrentUser = Context.Users.Find(User.Identity.GetUserId());

            List<Post> PostList = new List<Post>();

            if (OnlyFavorite)
            {
                foreach (var i in Context.Favorites.Where(i => i.User.Id == CurrentUser.Id).OrderByDescending(i => i.Post.Date).ToList())
                {
                    PostList.Add(i.Post);
                }
            }
            else
            {
                PostList = Context.Posts.Where(i => Id == "" ? true : i.Autor.Id == Id).OrderByDescending(i => i.Date).ToList();
            }

            SearchText = Text;
            if (SearchText != "")
            {
                PostList = PostList.Where(i => i.Title.Contains(SearchText) || i.Text.Contains(SearchText) || i.Autor.UserName.Contains(SearchText)).ToList();
            }
            if (CurrentUser != null)
            {
                PostList = PostList.Where(i => CurrentUser.Confession == null ? true : i.Confession.ConfessionName == CurrentUser.Confession.ConfessionName).ToList();
            }

            return PartialView("PostList", PostList);
        }

        public ActionResult PostList(string Id = "", string SortBy = "", bool ?Ascending = true, bool ?Favorite = null)
        {
            if(Favorite != null)
            {
                OnlyFavorite = Favorite.Value;
            }

            ApplicationDbContext Context = new ApplicationDbContext();
            ApplicationUser CurrentUser = Context.Users.Find(User.Identity.GetUserId());

            List<Post> PostList = new List<Post>();
          
            if(OnlyFavorite)
            {
                foreach (var i in Context.Favorites.Where(i => i.User.Id == CurrentUser.Id).OrderByDescending(i => i.Post.Date).ToList())
                {
                    PostList.Add(i.Post);
                }
            }
            else
            {
                PostList = Context.Posts.OrderByDescending(i => i.Date).ToList();
            }

            if (SearchText != "")
            {
                PostList = PostList.Where(i => i.Title.Contains(SearchText) || i.Text.Contains(SearchText) || i.Autor.UserName.Contains(SearchText)).ToList();
            }               
            PostList = PostList.Where(i => Id == "" ? true : (i.Autor.Id == Id)).ToList();
            if(CurrentUser != null)
            {
                PostList = PostList.Where(i => CurrentUser.Confession == null ? true : i.Confession.ConfessionName == CurrentUser.Confession.ConfessionName).ToList();
            }

            if (SortBy != String.Empty)
            {
                if(Ascending.Value)
                {
                    switch (SortBy)
                    {
                        case "Name":
                            PostList = PostList.OrderBy(i => i.Title).ToList();
                            return PartialView(PostList);
                        case "Date":
                            PostList = PostList.OrderBy(i => i.Date).ToList();
                            return PartialView(PostList);
                        case "Fav":
                            PostList = PostList.OrderBy(i => i.Favorites.Count).ToList();
                            return PartialView(PostList);
                    }       
                }
                else
                {
                    switch (SortBy)
                    {
                        case "Name":
                            PostList = PostList.OrderByDescending(i => i.Title).ToList();
                            return PartialView(PostList);
                        case "Date":
                            PostList = PostList.OrderByDescending(i => i.Date).ToList();
                            return PartialView(PostList);
                        case "Fav":
                            PostList = PostList.OrderByDescending(i => i.Favorites.Count).ToList();
                            return PartialView(PostList);
                    }
                }
            }

            return PartialView(PostList);
        }

        [HttpGet]
        public ActionResult EditComment(int Id)
        {
            ApplicationDbContext Context = new ApplicationDbContext();
            Comments CommentForEdit = Context.Comments.Find(Id);
            CommentForEdit.Text = CommentForEdit.Text.Replace("<br/>", "\r\n");
            return PartialView(CommentForEdit);
        }

        [HttpPost]
        public ActionResult EditComment(int Id, string Text)
        {
            ApplicationDbContext Context = new ApplicationDbContext();
            Comments CommentForEdit = Context.Comments.Find(Id);
            CommentForEdit.Text = Text.Replace("\r\n", "<br/>");
            Context.SaveChanges();
            return PartialView(CommentForEdit);
        }

        public ActionResult DeleteConfirm(int Id)
        {
            ApplicationDbContext Context = new ApplicationDbContext();
            Comments CommentForDelete = Context.Comments.Find(Id);
            CommentForDelete.Text = CommentForDelete.Text.Replace("<br/>", "\r\n");
            return PartialView(CommentForDelete);
        }

        public ActionResult DeleteComment(int Id)
        {
            ApplicationDbContext Context = new ApplicationDbContext();
            Comments CommentForDelete = Context.Comments.Find(Id);
            Context.Comments.Remove(CommentForDelete);
            Context.SaveChanges();
            return null;
        }

        public ActionResult ShowExtended(int Id)
        {
            ApplicationDbContext Context = new ApplicationDbContext();
            Post PostForShow = Context.Posts.Find(Id);
            return PartialView(PostForShow);
        }

        public ActionResult ShowComments(int Id)
        {
            ApplicationDbContext Context = new ApplicationDbContext();
            return PartialView(Context.Posts.Find(Id));
        }

        public ActionResult Like(int Id)
        {
            ApplicationDbContext Context = new ApplicationDbContext();
            ApplicationUser CurrentUser = Context.Users.Find(User.Identity.GetUserId());
            Post TargetPost = Context.Posts.Find(Id);

            //Checking if user doesn't have this post in favorites
            if (Context.Favorites.Where(i => i.User.Id == CurrentUser.Id && i.Post.Id == TargetPost.Id).Count() <= 0)
            {
                Favorite NewFavorite = new Favorite();
               
                NewFavorite.Post = TargetPost;
                NewFavorite.User = CurrentUser;

                CurrentUser.Favorites.Add(NewFavorite);
                TargetPost.Favorites.Add(NewFavorite);

                Context.Favorites.Add(NewFavorite);

                Context.SaveChanges();
            }
            return PartialView(TargetPost);
        }

        public ActionResult Dislike(int Id)
        {
            ApplicationDbContext Context = new ApplicationDbContext();
            ApplicationUser CurrentUser = Context.Users.Find(User.Identity.GetUserId());
            Post TargetPost = Context.Posts.Find(Id);

            //Checking if user have this post in favorites
            if (Context.Favorites.Where(i => i.User.Id == CurrentUser.Id && i.Post.Id == TargetPost.Id).Count() > 0)
            {
                Favorite DeletingFavorite = Context.Favorites.Where(i => i.User.Id == CurrentUser.Id && i.Post.Id == TargetPost.Id).FirstOrDefault();

                CurrentUser.Favorites.Remove(DeletingFavorite);
                TargetPost.Favorites.Remove(DeletingFavorite);

                Context.Favorites.Remove(DeletingFavorite);

                Context.SaveChanges();
            }
            return PartialView(TargetPost);
        }

        [HttpPost]
        public ActionResult Comment(int Id, string Text)
        {
            ApplicationDbContext Context = new ApplicationDbContext();
            ApplicationUser CurrentUser = Context.Users.Find(User.Identity.GetUserId());
            Post TargetPost = Context.Posts.Find(Id);

            Comments NewComment = new Comments();

            NewComment.Autor = CurrentUser;
            NewComment.Date = DateTime.Now;
            NewComment.Text = Text.Replace("\r\n", "<br/>");
            NewComment.Post = TargetPost;

            TargetPost.Comments.Add(NewComment);

            Context.Comments.Add(NewComment);

            Context.SaveChanges();

            return PartialView(TargetPost.Comments);
        }

        [HttpGet]
        public ActionResult Comment(int Id)
        {
            ApplicationDbContext Context = new ApplicationDbContext();
            return PartialView(Context.Comments.Where(i => i.Post.Id == Id).ToList());
        }

        public ActionResult Favorite()
        {
            return View();
        }

        public ActionResult Menu()
        {
            return PartialView();
        }

        public ActionResult NavMenu()
        {
            return PartialView();
        }
        public ActionResult MessagesMenu()
        {
            ApplicationDbContext Context = new ApplicationDbContext();
            ApplicationUser CurrentUser = Context.Users.Find(User.Identity.GetUserId());
            List<ChatRoom> RecentMessages = new List<ChatRoom>();
            if (CurrentUser != null)
            {
                ChatMessage LastMessage = new ChatMessage();


                foreach (var Item in Context.ChatRooms.Where(i => i.FirstUser.Id == CurrentUser.Id || i.SecondUser.Id == CurrentUser.Id).ToList())
                {
                    if (Item.ChatMessages.ToList().Count() != 0)
                    {
                        LastMessage = Item.ChatMessages.ToList().Last();
                        if (LastMessage.User != CurrentUser && Math.Abs(LastMessage.Date.Day - DateTime.Now.Day) <= 2 && LastMessage.Date.Month == DateTime.Now.Month && LastMessage.Date.Year == DateTime.Now.Year)
                        {
                            RecentMessages.Add(Item);
                        }
                    }
                  
                }
            }

            return PartialView(RecentMessages);
        }
        public ActionResult RecentMenu()
        {
            ApplicationDbContext Context = new ApplicationDbContext();
            string Id = User.Identity.GetUserId();
            List<Post> Posts = new List<Post>();
            if (Id != null)
            {
                foreach (var item in Context.Subscriptions.Where(i => i.User.Id == Id).ToList())
                {
                    Id = item.Pastor.User.Id;
                    Posts.AddRange(Context.Posts.Where(i => i.Autor.Id == Id
                    && Math.Abs(i.Date.Day - DateTime.Now.Day) <= 2
                    && (i.Date.Month == DateTime.Now.Month)
                    && (i.Date.Year == DateTime.Now.Year)).ToList());
                }
            }
            return PartialView(Posts);
        }

        public ActionResult SortMenu(string Id)
        {
            return PartialView(Id.ToCharArray());
        }

        public ActionResult Recent()
        {
            int a = 121;
            return PartialView(a);
        }
    }
}