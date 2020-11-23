using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebQLThucTapSinh.Common;
using WebQLThucTapSinh.Models;

namespace WebQLThucTapSinh.Controllers
{
    public class FacultyController : Controller
    {
        // GET: Faculty
        public ActionResult Index()
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            //var id = Session["Person"].ToString();
            var id = "ZXCVBUML";
            var list = database.Organization.Where(x=>x.PersonID == id).ToList();
            var companyID = database.Person.Find(id).CompanyID;
            list.Remove(list.SingleOrDefault(x=>x.ID == companyID));
            return View(list);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Organization organization)
        {
            if (ModelState.IsValid)
            {
                //var id = Session["Person"].ToString();
                var id = "ZXCVBUML";
                organization.PersonID = id;
                if (new Share().InsertOrganization(organization))
                {
                    ModelState.AddModelError("", "Thêm Khoa thành công");
                }
                else
                {
                    ModelState.AddModelError("", "Thêm Khoa thất bại");
                }
            }
            return View("Create");
        }

    }
}