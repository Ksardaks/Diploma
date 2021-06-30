using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using PastorNub.Models;

namespace PastorNub.Controllers
{
    public class UserController : Controller
    {
        public ActionResult Subscriptions()
        {
           
            ApplicationDbContext Context = new ApplicationDbContext();
            string Id = User.Identity.GetUserId();
            return View(Context.Subscriptions.Where(i => i.User.Id == Id).ToList());
        }
    }
}