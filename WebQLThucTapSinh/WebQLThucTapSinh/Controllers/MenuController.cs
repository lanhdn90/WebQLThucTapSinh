using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebQLThucTapSinh.Models;

namespace WebQLThucTapSinh.Controllers
{
    public class MenuController : Controller
    {
        // GET: Menu
        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult MainMenu()
        {
            var role = Convert.ToInt32(Session["Role"].ToString());
            WebDatabaseEntities database = new WebDatabaseEntities();
            var model = database.Menu.Where(x => x.RoleID == role).ToList();
            return PartialView(model);
        }
    }
}