using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebQLThucTapSinh.Common;
using WebQLThucTapSinh.Models;

namespace WebQLThucTapSinh.Controllers
{
    public class InformationController : Controller
    {
        // GET: Information
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TTcanhan()
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            var userName = Session["TenTk"].ToString();
            var findU = database.Users.Find(userName);
            var model = database.Person.Find(findU.PersonID);
            SetViewBagG();
            return View(model);
        }

        [HttpPost]
        public ActionResult TTcanhan1(string Password, string Newpassword)
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            var userName = Session["TenTk"].ToString();
            var model = database.Users.Find(userName);
            if (model.PassWord != MaHoaMd5.MD5Hash(Password))
            {
                ModelState.AddModelError("", "Mật khẩu không đúng.");

            }
            else
            {
                HomeController home = new HomeController();
                home.UpdateUser(model.PersonID, Newpassword);
            }
            SetViewBagG();
            var findP = database.Person.Find(model.PersonID);
            return RedirectToAction("TTcanhan");
        }

        [HttpPost]
        public ActionResult UpdatePerson(Person person)
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            var userName = Session["TenTk"].ToString();
            var model = database.Users.Find(userName);
            new Share().UpdatePerson(person);
            SetViewBagG();
            return RedirectToAction("TTcanhan");
        }

        public void SetViewBagG(string selectedID = null)
        {
            SelectList GenGender = new SelectList(new[] {
                new {Text = "Nam", Value = true},
                new {Text = "Nữ", Value = false},
            }, "Value", "Text");
            ViewBag.GenGender = GenGender;
        }

        public void SetViewBag(string selectedID = null)
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            var list = database.Person.Where(x => x.RoleID == 2).ToList();
            List<Organization> Organ = new List<Organization>();
            foreach (var item in list)
            {
                var model = database.Organization.Find(item.CompanyID);
                Organ.Add(model);
            }
            SelectList OrganList = new SelectList(Organ, "ID", "Name");
            ViewBag.OrganList = OrganList;
        }
    }
}