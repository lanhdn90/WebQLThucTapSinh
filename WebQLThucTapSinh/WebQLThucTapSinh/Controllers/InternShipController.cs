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
    public class InternShipController : Controller
    {
        // GET: InternShip
        public ActionResult Index(int id = 0)
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            var model = listIShip();
            if (id == 0)
            {
                id = model.KhoaHocCoQuanly[0].InternshipID;
                Session["InternshipID"] = id;
            }
            else
            {
                Session["InternshipID"] = id;
            }
            var l = database.IntershipWithTask.Where(x => x.InternshipID == id).OrderBy(x => x.Sort).ToList();
            SelectList chose = new SelectList(l, "Sort", "Sort");
            ViewBag.listID = chose;
            ViewBag.listT = model;
            return View(ListTask(id));
        }

        public InternShipViewModel listIShip()
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            //Sau khi làm sprint Login thì mở lại
            var personID = Session["Person"].ToString();

            var role = Convert.ToInt32(Session["Role"]);
            var companyId = database.Person.Find(personID).CompanyID;
            var model = new InternShipViewModel();
            if (role == 4)
            {
                model.KhoaHocCoQuanly = database.InternShip.Where(x => x.PersonID == personID).OrderByDescending(x => x.StartDay).ToList();
                model.KhoaHocChuaCoQuanly = database.InternShip.Where(c => c.CompanyID == companyId && c.PersonID == null).OrderByDescending(x => x.StartDay).ToList();
            }
            else
            {
                model.KhoaHocCoQuanly = database.InternShip.Where(x => x.CompanyID == companyId && x.PersonID != null).OrderByDescending(x => x.StartDay).ToList();
                model.KhoaHocChuaCoQuanly = database.InternShip.Where(c => c.CompanyID == companyId && c.PersonID == null).OrderByDescending(x => x.StartDay).ToList();
            }
            return model;
        }

        public List<TaskDatabase> ListTask(int id)
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            var l = (from a in database.Task
                     join b in database.IntershipWithTask on a.TaskID equals b.TaskID
                     where b.InternshipID == id
                     select new TaskDatabase
                     {
                         ID = b.ID,
                         taskid = a.TaskID,
                         taskname = a.TaskName,
                         sort = b.Sort,
                         InternshipID = b.InternshipID
                     }).OrderBy(x => x.sort).ToList();
            return l;
        }

        public List<Task> Tasks(int id)
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            List<Task> tt = new List<Task>();
            var listinandt = database.IntershipWithTask.Where(x => x.InternshipID == id).ToList();
            // danh sách task cú cùng intership
            foreach (var item in listinandt)
            {
                var tas = database.Task.Find(item.TaskID);
                tt.Add(tas);
                // lấy ds task trong bảng task
                // đối tượng task (Find)
            }
            return tt;
        }

        public ActionResult Create()
        {
            SetViewBagL();
            SetViewBagM();
            return View();
        }

        public bool CreateInternShip(InternShip ish)
        {
            try
            {
                WebDatabaseEntities database = new WebDatabaseEntities();
                InternShip i = new InternShip();
                i.InternshipID = database.InternShip.Count() + 1;
                i.CourseName = ish.CourseName;
                i.Note = ish.Note;
                i.StartDay = ish.StartDay;
                i.ExpiryDate = ish.ExpiryDate;
                i.Status = false;
                i.CompanyID = Session["CompanyID"].ToString();
                var ro = Convert.ToInt32(Session["Role"]);
                if (ish.PersonID != null)
                {
                    i.PersonID = ish.PersonID;
                }
                else if (ro == 4)
                {
                    var pid = Session["Person"].ToString();
                    i.PersonID = pid;
                }
                database.InternShip.Add(i);
                database.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        [HttpPost]
        public ActionResult Create(InternShip ish)
        {
            if (ModelState.IsValid)
            {
                if (CreateInternShip(ish))
                {
                    ModelState.AddModelError("", "Thêm Khóa học thành công");
                }
                else
                {
                    ModelState.AddModelError("", "Thêm Khóa học thất bại");
                }
            }
            SetViewBagL();
            SetViewBagM();
            return View("Create");
        }

        public void SetViewBagM(string selectedID = null)
        {
            SelectList Month = new SelectList(new[] {
                new {Text = "Một Tháng", Value = 1},
                new {Text = "Hai Tháng", Value = 2},
                new {Text = "Ba Tháng", Value = 3},
                new {Text = "Bốn Tháng", Value = 4},
                new {Text = "Năm Tháng", Value = 5},
                new {Text = "Sáu Tháng", Value = 6},
            }, "Value", "Text");
            ViewBag.Month = Month;
        }

        public void SetViewBagL(string selectedID = null)
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            var model = Session["CompanyID"].ToString();
            var list = database.Person.Where(x => x.CompanyID == model && x.RoleID == 4).ToList();
            var listLeader = new List<LeaderClass>();
            
            foreach (var item in list)
            {
                LeaderClass leader = new LeaderClass();
                leader.FullName = item.LastName + " " + item.FirstName;
                leader.PersonID = item.PersonID;
                listLeader.Add(leader);
            }
            SelectList Listleader = new SelectList(listLeader, "PersonID", "FullName");
            ViewBag.ListLeader = Listleader;
        }

        public ActionResult Edit(int id)
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            var model = database.InternShip.Find(id);
            SetViewBagL();
            SetViewBagM();
            Session["InternshipID"] = id;
            return View(model);
        }

        public bool EditInterShip(InternShip ish)
        {
            try
            {
                WebDatabaseEntities database = new WebDatabaseEntities();
                var i = database.InternShip.Find(ish.InternshipID);
                i.CourseName = ish.CourseName;
                i.Note = ish.Note;
                i.StartDay = ish.StartDay;
                i.ExpiryDate = ish.ExpiryDate;
                if (ish.PersonID != null)
                {
                    i.PersonID = ish.PersonID;
                }
                database.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        [HttpPost]
        public ActionResult Edit(InternShip ish)
        {
            if (ModelState.IsValid)
            {
                if (EditInterShip(ish))
                {
                    ModelState.AddModelError("", "Cập nhật Khóa học thành công");
                }
                else
                {
                    ModelState.AddModelError("", "Cập nhật Khóa học thất bại");
                }
            }
            SetViewBagL();
            SetViewBagM();
            Session["InternshipID"] = ish.InternshipID;
            return View("Edit");
        }

        public ActionResult Accuracy(int id)
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            var pid = Session["Person"].ToString();
            var model = database.InternShip.Find(id);
            model.PersonID = pid;
            database.SaveChanges();
            return RedirectToAction("Index", new { id = 0 });
        }

        //Xóa bài học ra khỏi danh sách bài học của khóa học
        [HttpPost]
        public ActionResult DeleteInternShipWithTask(int id)
        {
            JsonSerializerSettings json = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            var result = "";
            if (DeleteTask(id))
            {
                result = JsonConvert.SerializeObject("Xóa thành công Bài học", Formatting.Indented, json);
            }
            else
            {
                result = JsonConvert.SerializeObject("Xóa Bài học thất bại", Formatting.Indented, json);
            }
            return this.Json(result, JsonRequestBehavior.AllowGet);
        }

        public bool DeleteTask(int id)
        {
            try
            {
                new Company().DeleteIntershipWithTask(id);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void deleteInternShip(int id)
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            var count = database.InternShip.ToList().Count();
            var modelend = database.InternShip.SingleOrDefault(x => x.InternshipID == count);
            var model = database.InternShip.SingleOrDefault(c => c.InternshipID == id);
            model.CourseName = modelend.CourseName;
            model.Note = modelend.Note;
            model.PersonID = modelend.PersonID;
            model.CompanyID = modelend.CompanyID;
            model.StartDay = modelend.StartDay;
            model.ExpiryDate = modelend.ExpiryDate;
            model.Status = modelend.Status;
            var list = database.IntershipWithTask.Where(a => a.InternshipID == count).ToList();
            if(list != null)
            {
                foreach(var item in list)
                {
                   database.IntershipWithTask.Find(item.ID).InternshipID = id;
                }
                database.SaveChanges();
            }
            database.InternShip.Remove(modelend);
            database.SaveChanges();
        }

        public int DeleteInternShip(int id)
        {
            Company company = new Company();
            if (company.DeleteListIntershipWithTask(0, id) && company.UpdateIntern(id))
            {
                deleteInternShip(id);
                return 1;
            }
            else
            {
                if (company.DeleteListIntershipWithTask(0, id) == false)
                {
                    return 2;
                }
                else
                {
                    if (company.UpdateIntern(id) == false)
                    {
                        return 3;
                    }
                    else
                    {
                        return 4;
                    }
                }
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            JsonSerializerSettings json = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            var result = "";
            switch (DeleteInternShip(id))
            {
                case 1:
                    result = JsonConvert.SerializeObject("Xóa thành công Khóa học", Formatting.Indented, json);
                    break;
                case 2:
                    result = JsonConvert.SerializeObject("Xóa danh sách Bài học thất bại", Formatting.Indented, json);
                    break;
                case 3:
                    result = JsonConvert.SerializeObject("Cập nhật thực tập sinh thất bại", Formatting.Indented, json);
                    break;
                case 4:
                    result = JsonConvert.SerializeObject("Xóa Khóa học thất bại", Formatting.Indented, json);
                    break;
            }
            return this.Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateSort(int id, int sort)
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            var find1 = database.IntershipWithTask.Find(id);
            int sort1 = find1.Sort; //1
            find1.Sort = sort;
            var find2 = database.IntershipWithTask.SingleOrDefault(x => x.InternshipID == find1.InternshipID && x.Sort == sort);
            find2.Sort = sort1;
            database.SaveChanges();
            JsonSerializerSettings json = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            var result = JsonConvert.SerializeObject("Cập nhật danh sách bài học thành công", Formatting.Indented, json);
            return this.Json(result, JsonRequestBehavior.AllowGet);
        }

        public bool ChangeStatusInternS(int id)
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            var com = database.InternShip.Find(id);
            if (com.Status == true)
            {
                com.Status = false;
            }
            else
            {
                var date = DateTime.Now;
                if (com.StartDay > date)
                {
                    com.StartDay = date;
                    com.Status = true;
                }
                else if (com.StartDay.AddMonths(com.ExpiryDate) > date)
                {
                    com.Status = true;
                }
            }
            database.SaveChanges();
            return com.Status;

        }

        [HttpPost]
        public JsonResult ChangeStatus(int id)
        {
            var res = ChangeStatusInternS(id);
            return Json(new
            {
                status = res
            });
        }
    }
}