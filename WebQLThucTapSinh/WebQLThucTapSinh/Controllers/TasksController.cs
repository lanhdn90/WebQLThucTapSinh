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

        
    }
}