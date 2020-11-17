using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebQLThucTapSinh.Controllers;
using WebQLThucTapSinh.Models;

namespace WebQLThucTapSinh.Common
{
    public class Company
    {
        //Xóa các câu hỏi liên quan tới taksID
        public bool DeleteListQuestion(int id)
        {
            try
            {
                WebDatabaseEntities database = new WebDatabaseEntities();
                var listQuestion = database.Question.Where(x => x.TaskID == id).ToList();
                if(listQuestion != null)
                {
                    foreach (var item in listQuestion)
                    {
                        new QuestionController().DeleteQuestion(item.QuestionID);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        //Xóa Row IntershipWithTask có TaskId = id
        public void DeleteIntershipWithTask(int id)
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            // Tìm IntershipWithTask theo id
            var model = database.IntershipWithTask.Find(id);
            // lưu các biến internshipId , sort
            var internshipId = model.InternshipID;
            var sort = model.Sort;
            // tìm IntershipWithTask cuối cùng trong table IntershipWithTask
            var count = database.IntershipWithTask.ToList().Count();
            var modelend = database.IntershipWithTask.Find(count);
            //Gán các giá trị của modelend vào model
            model.InternshipID = modelend.InternshipID;
            model.TaskID = modelend.TaskID;
            model.Sort = modelend.Sort;
            //Remove modelend
            database.IntershipWithTask.Remove(modelend);
            database.SaveChanges();
            //lấy listIntershipWithTask dựa trên internshipId
            var listIntershipWithTask = database.IntershipWithTask.Where(x => x.InternshipID == internshipId).OrderBy(c => c.Sort).ToList();
            // Update sort trong listIntershipWithTask
            foreach (var item in listIntershipWithTask)
            {
                if (item.Sort > sort)
                {
                    item.Sort = item.Sort - 1;
                    database.SaveChanges();
                }
            }
        }

        //Xóa list row IntershipWithTask
        public bool DeleteListIntershipWithTask(int taskId, int internshipId)
        {
            try
            {
                WebDatabaseEntities database = new WebDatabaseEntities();
                // Tạo ListTasks
                List<IntershipWithTask> ListTasks = new List<IntershipWithTask>();
                if (taskId != 0)
                {
                    // Nếu taskId != 0
                    ListTasks = database.IntershipWithTask.Where(x => x.TaskID == taskId).ToList();
                }
                else
                {
                    // Nếu taskId == 0
                    ListTasks = database.IntershipWithTask.Where(x => x.InternshipID == internshipId).ToList();
                }
                if(ListTasks != null)
                {
                    foreach (var item in ListTasks)
                    {
                        //Delete row in IntershipWithTask dựa vào id of row 
                        DeleteIntershipWithTask(item.ID);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }

        }

        public bool UpdateIntern(int id)
        {
            try
            {
                WebDatabaseEntities database = new WebDatabaseEntities();
                var listIntern = database.Intern.Where(x => x.InternshipID == id).ToList();
                if(listIntern != null)
                {
                    foreach(var item in listIntern)
                    {
                        item.InternshipID = null;
                        item.Result = 0;
                    }
                    database.SaveChanges();
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