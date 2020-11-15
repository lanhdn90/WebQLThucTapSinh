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
    public class TasksController : Controller
    {
        // GET: Tasks
        public List<TaskClass> tasks()
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            //Sau khi làm sprint Login thì mở lại
            //var id = Session["Person"].ToString();
            var id = "PERSONBC";
            // Lấy roleID
            //var r = Convert.ToInt32(Session["Role"].ToString());
            var ro = 4;
            if (ro == 4)
            {
                var model = (from a in database.Task
                             join b in database.Person on a.PersonID equals b.PersonID
                             where b.PersonID == id
                             select new TaskClass
                             {
                                 TaskID = a.TaskID,
                                 TaskName = a.TaskName,
                                 Note = a.Note,
                                 Video = a.Video,
                                 Questions = database.Question.Where(x=>x.TaskID == a.TaskID).Count(),
                                 NumberOfQuestions = a.NumberOfQuestions,
                                 Result = a.Result,
                                 PersonID = a.PersonID,
                                 FullName = b.LastName + " " + b.FirstName
                             }).OrderBy(x => x.TaskID).ToList();
                return model;
            }
            else
            {
                //var comid = Session["CompanyID"].ToString();
                var comid = "ORGANIZC";
                var model = (from a in database.Task
                             join b in database.Person on a.PersonID equals b.PersonID
                             where b.CompanyID == comid && b.RoleID == 4
                             select new TaskClass
                             {
                                 TaskID = a.TaskID,
                                 TaskName = a.TaskName,
                                 Note = a.Note,
                                 Video = a.Video,
                                 Questions = database.Question.Where(x => x.TaskID == a.TaskID).Count(),
                                 NumberOfQuestions = a.NumberOfQuestions,
                                 Result = a.Result,
                                 PersonID = a.PersonID,
                                 FullName = b.LastName + " " + b.FirstName
                             }).OrderBy(x => x.PersonID).ToList();
                return model;
            }
        }

        public List<InternShip> listin()
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            //Sau khi làm sprint Login thì mở lại
            //var id = Session["Person"].ToString();
            var id = "PERSONBC";
            // Lấy roleID
            //var r = Convert.ToInt32(Session["Role"].ToString());
            var r = 4;
            var find = database.Person.Find(id);
            return database.InternShip.Where(x => x.CompanyID == find.CompanyID).ToList();
        }

        public ActionResult Index()
        {
            ViewBag.listin = listin();
            return View(tasks());
        }

        public bool AddTask(List<int> listTask, int id)
        {
            try
            {
                WebDatabaseEntities database = new WebDatabaseEntities();
                foreach (var i in listTask)
                {
                    // check Intership đã có task truyên vào hay chưa 
                    var listT = database.IntershipWithTask.Where(x => x.InternshipID == id).ToList();
                    IntershipWithTask model = new IntershipWithTask();
                    if (listT == null)
                    {
                        model = null;
                    }
                    else
                    {
                        model = listT.SingleOrDefault(c => c.TaskID == i);
                    }
                    if(model== null)
                    {
                        IntershipWithTask addTask = new IntershipWithTask();
                        var count = database.IntershipWithTask.Count();
                        addTask.ID = count + 1;
                        addTask.InternshipID = id;
                        addTask.TaskID = i;
                        var sort = database.IntershipWithTask.Where(x => x.InternshipID == id).Count();
                        addTask.Sort = sort + 1;
                        database.IntershipWithTask.Add(addTask);
                        database.SaveChanges();
                    }   
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Task task)
        {
            if (ModelState.IsValid)
            {
                if (CreateTask(task))
                {
                    ModelState.AddModelError("", "Thêm Bài học thành công");
                }
                else
                {
                    ModelState.AddModelError("", "Thêm Công ty thất bại");
                }
            }
            return View("Create");
        }

        public bool CreateTask(Task task)
        {
            try
            {
                WebDatabaseEntities database = new WebDatabaseEntities();
                //Sau khi làm sprint Login thì mở lại
                //var personID = Session["Person"].ToString();
                var personID = "PERSONBC";
                var count = database.Task.Count();
                Task t = new Task();
                t.TaskID = count + 1;
                t.TaskName = task.TaskName;
                t.Note = task.Note;
                t.Video = task.Video;
                t.PersonID = personID;
                database.Task.Add(t);
                database.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            var model = database.Task.Find(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(Task task)
        {
            if (ModelState.IsValid)
            {
                var model = Update(task);
                if (model)
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

        public bool Update(Task task)
        {
            try
            {
                WebDatabaseEntities database = new WebDatabaseEntities();
                var model = database.Task.Find(task.TaskID);
                model.TaskName = task.TaskName;
                model.Note = task.Note;
                model.Video = task.Video;
                model.NumberOfQuestions = task.NumberOfQuestions;
                model.Result = task.Result;
                database.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public int DeleteTask(int id)
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            Company company = new Company();
            if (company.DeleteListQuestion(id) == true && company.DeleteListIntershipWithTask(id, 0) == true)
            {
                database.Task.Remove(database.Task.Find(id));
                database.SaveChanges();
                return 1;
            }
            else
            {
                if(company.DeleteListQuestion(id) == false)
                {
                    return 2;
                }
                else
                {
                    if(company.DeleteListIntershipWithTask(id, 0) == false)
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

        public ActionResult Delete(int id)
        {
            JsonSerializerSettings json = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            var result = "";
            switch(DeleteTask(id))
            {
                case 1:
                    result = JsonConvert.SerializeObject("Xóa thành công", Formatting.Indented, json);
                    break;
                case 2:
                    result = JsonConvert.SerializeObject("Không thể xóa dánh sách câu hỏi", Formatting.Indented, json);
                    break;
                case 3:
                    result = JsonConvert.SerializeObject("Không thể xóa bài học ra khỏi khóa học", Formatting.Indented, json);
                    break;
                case 4:
                    result = JsonConvert.SerializeObject("Xóa thất bại", Formatting.Indented, json);
                    break;
            }

            return this.Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}