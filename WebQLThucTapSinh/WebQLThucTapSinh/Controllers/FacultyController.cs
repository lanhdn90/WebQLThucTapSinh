using Newtonsoft.Json;
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
            var id = Session["Person"].ToString();
            var list = database.Organization.Where(x => x.PersonID == id).ToList();
            var companyID = database.Person.Find(id).CompanyID;
            list.Remove(list.SingleOrDefault(x => x.ID == companyID));
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
                WebDatabaseEntities database = new WebDatabaseEntities();
                var id = Session["Person"].ToString();
                organization.PersonID = id;
                var company = Session["CompanyID"].ToString();
                var model = database.Organization.SingleOrDefault(x => x.ID == company);
                organization.StartDay = model.StartDay;
                organization.ExpiryDate = model.ExpiryDate;
                organization.Status = true;
                organization.SendEmail = false;
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

        [HttpPost]
        public JsonResult ChangeStatus(string id)
        {
            var res = new Company().ChangeStatusUser(id, 3);
            return Json(new
            {
                status = res
            });
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            var model = database.Organization.Find(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(Organization faculty)
        {
            if (ModelState.IsValid)
            {
                var model1 = new Company().UpdateOrganization(faculty, 1);
                if (model1)
                {
                    ModelState.AddModelError("", "Cập nhật thành công");
                }
                else
                {
                    ModelState.AddModelError("", "Cập nhật thất bại");
                }
            }
            return View("Edit");
        }

        public int DeleteFaculty(string id)
        {
            var company = Session["CompanyID"].ToString();
            var re = new Share().UpdateIntern(id, company);
            if (re == true)
            {
                try
                {
                        WebDatabaseEntities database = new WebDatabaseEntities();
                    var list = database.Person.Where(x => x.SchoolID == id && x.RoleID == 3).ToList();
                    if (list != null)
                    {
                        foreach (var item in list)
                        {
                            new Share().DeletePerson(item.PersonID);
                        }
                    }
                    
                    return 1;
                }
                catch
                {
                    return 2;
                }
            }
            else
            {
                return 3;
            }
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            JsonSerializerSettings json = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            var result = "";
            switch (DeleteFaculty(id))
            {
                case 1:
                    WebDatabaseEntities database = new WebDatabaseEntities();
                    var model = database.Organization.Find(id);
                    database.Organization.Remove(model);
                    database.SaveChanges();
                    result = JsonConvert.SerializeObject("Xóa thành công", Formatting.Indented, json);
                    break;
                case 2:
                    result = JsonConvert.SerializeObject("Không thể xóa Giáo vụ", Formatting.Indented, json);
                    break;
                case 3:
                    result = JsonConvert.SerializeObject("Không thể câp nhật thực tập sinh", Formatting.Indented, json);
                    break;
            }
            return this.Json(result, JsonRequestBehavior.AllowGet);
        }

        public bool deleteFaculty(string id)
        {
            var re = new Share().UpdateIntern(id, null);
            if (re == true)
            {
                WebDatabaseEntities database = new WebDatabaseEntities();
                var list = database.Person.Where(x => x.SchoolID == id && x.RoleID == 3).ToList();
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        new Share().DeletePerson(item.PersonID);
                    }
                }
                return (new Company().deleteOrganzation(id));
            }
            else
            {
                return false;
            }
        }

        public bool DeleteListFaculty(string id)
        {
            try
            {
                WebDatabaseEntities database = new WebDatabaseEntities();
                var school = database.Person.SingleOrDefault(c => c.CompanyID == id && c.RoleID == 6);
                var list = database.Organization.Where(x => x.PersonID == school.PersonID).ToList();
                list.Remove(list.SingleOrDefault(x => x.ID == id));
                foreach (var item in list)
                {
                    deleteFaculty(item.ID);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}