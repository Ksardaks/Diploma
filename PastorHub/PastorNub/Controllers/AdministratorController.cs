using PastorNub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PastorNub.Controllers
{
    public class AdministratorController : Controller
    {
        public ActionResult ConfessionsManager()
        {
            ApplicationDbContext Context = new ApplicationDbContext();
            return View(Context.Confessions.ToList());
        }

        [HttpGet]
        public ActionResult CreateConfession()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult CreateConfession(string Title)
        {
            ApplicationDbContext Context = new ApplicationDbContext();
            Confession NewConfession = new Confession() { ConfessionName = Title};
            Context.Confessions.Add(NewConfession);
            Context.SaveChanges();

            return RedirectToAction("ConfessionsManager");
        }

        [HttpGet]
        public ActionResult EditConfession(int Id)
        {
            ApplicationDbContext Contex = new ApplicationDbContext();
            Confession ConfessionForEdit = Contex.Confessions.Find(Id);
            return PartialView(ConfessionForEdit);
        }

        [HttpPost]
        public ActionResult EditConfession(int Id, string Name)
        {
            ApplicationDbContext Contex = new ApplicationDbContext();
            Confession ConfessionForEdit = Contex.Confessions.Find(Id);
            ConfessionForEdit.ConfessionName = Name;
            Contex.SaveChanges();

            return RedirectToAction("ConfessionsManager");
        }

        public ActionResult DeleteConfession(int Id)
        {
            ApplicationDbContext Contex = new ApplicationDbContext();

            Confession ConfessionToDelete = Contex.Confessions.Where(i => i.Id == Id).FirstOrDefault();
            Contex.Confessions.Remove(ConfessionToDelete);
            Contex.SaveChanges();

            return RedirectToAction("ConfessionsManager");
        }
    }
}