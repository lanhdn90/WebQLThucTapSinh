using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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