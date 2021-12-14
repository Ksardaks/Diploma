using PastorNub.Models;
using System.Linq;
using System.Web.Mvc;

namespace PastorNub.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ApplicationDbContext Contex = new ApplicationDbContext();
            return View(Contex.Posts.ToList());
        }


        public ActionResult PostList()
        {
            ApplicationDbContext Context = new ApplicationDbContext();
            return PartialView(Context.Posts.ToList());
        }

    }
}