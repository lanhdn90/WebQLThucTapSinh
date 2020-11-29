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

        public bool ChangeStatusUser(string id, int roleId)
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            var com = database.Organization.SingleOrDefault(x => x.ID == id);
            if (com.Status == true)
            {
                var listPerson = database.Person.Where(x => x.SchoolID == id && x.RoleID == roleId).ToList();
                if(listPerson != null)
                {
                    foreach (var item in listPerson)
                    {
                        var model = database.Users.SingleOrDefault(a => a.PersonID == item.PersonID);
                        if(model.Status == true)
                        {
                            model.Status = false;
                            database.SaveChanges();
                        }
                    }
                    
                }
                com.Status = false;
            }
            else
            {
                com.Status = true;

            }
            database.SaveChanges();
            return com.Status;
        }

        public bool UpdateOrganization(Organization organization, int roileId)
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            var model = database.Organization.SingleOrDefault(x => x.ID == organization.ID);
            //roileId = 1 là Faculty
            if (roileId == 1)
            {
                try
                {
                    model.Name = organization.Name;
                    model.Phone = organization.Phone;
                    model.Fax = organization.Fax;
                    model.Email = organization.Email;
                    database.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
                
            }
            else
            {
                try
                {
                    model.Name = organization.Name;
                    model.Address = organization.Address;
                    model.Phone = organization.Phone;
                    model.Fax = organization.Fax;
                    model.Image = organization.Image;
                    model.Logo = organization.Logo;
                    model.Note = organization.Note;
                    model.Email = organization.Email;
                    database.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public bool FindOrgan(string companiID)
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            var model = database.Organization.Find(companiID);
            if (model == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool FindPerson(string personID)
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            var model = database.Person.Find(personID);
            if (model == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Create(Organization organi, int roleId)
        {
            try
            {
                WebDatabaseEntities database = new WebDatabaseEntities();
                Organization organ = new Organization();
                Share share = new Share();
                string comID;
                do
                {
                    comID = new Share().RandomText();
                } while (new Company().FindOrgan(comID) == false);
                organ.ID = comID;
                organ.Name = organi.Name;
                organ.Address = organi.Address;
                organ.Phone = organi.Phone;
                organ.Fax = organi.Fax;
                organ.Image = organi.Image;
                organ.Logo = organi.Logo;
                organ.Note = organi.Note;
                organ.Email = organi.Email;
                organ.StartDay = DateTime.Now;
                organ.ExpiryDate = 1;
                organ.Status = true;
                organ.SendEmail = false;
                database.Organization.Add(organ);
                database.SaveChanges();
                var model = database.Organization.Find(comID);
                if (model == null)
                {
                    return false;
                }
                else
                {
                    Person person = new Person();
                    string personID;
                    do
                    {
                        personID = share.RandomText();
                    } while (personID == comID && FindPerson(personID) == false);
                    person.PersonID = personID;
                    person.CompanyID = comID;
                    if (roleId == 2)
                    {
                        person.RoleID = 2;
                        if (share.InsertPerson(person))
                        {
                            database.Organization.Find(comID).PersonID = personID;
                            database.Person.Find(personID).Email = organi.Email;
                            database.SaveChanges();
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        person.RoleID = 6;
                        if (share.InsertPerson(person))
                        {
                            database.Organization.Find(comID).PersonID = personID;
                            database.Person.Find(personID).Email = organi.Email;
                            database.SaveChanges();
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        //Xóa danh sách task của 1 thằng leader
        public bool DeleteListTask(string personID)
        {
            try
            {
                WebDatabaseEntities database = new WebDatabaseEntities();
                var list = database.Task.Where(x => x.PersonID == personID).ToList();
                if(list != null)
                {
                    foreach (var item in list)
                    {
                        //Xóa danh sách câu hỏi của từng bài học
                        new TasksController().DeleteTask(item.TaskID);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        //xóa danh sách khoa học của thằng leader
        public bool DeleteListInterShip(string personID)
        {
            try
            {
                WebDatabaseEntities database = new WebDatabaseEntities();
                var list = database.InternShip.Where(x => x.PersonID == personID).ToList();
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        new InternShipController().DeleteInternShip(item.InternshipID);
                    }
                }     
                return true;
            }
            catch
            {
                return false;
            }
        }

        //Xóa 1 thằng leader
        public bool DeleteLeader(string personID)
        {
            if(DeleteListTask(personID) && DeleteListInterShip(personID))
            {
                if(new Share().DeletePerson(personID))
                {
                    //Xóa Leader thành công
                    return true;
                }
                else
                {
                    //Xóa Leader thất bại
                    return false;
                }
            }
            else
            {
                //Xóa Leader thất bại
                return false; 
            }
        }

        //Xóa danh sách leader của 1 công ty
        public bool DeleteListLeader(string companiID)
        {
            try
            {
                WebDatabaseEntities database = new WebDatabaseEntities();
                var listper = database.Person.Where(x => x.CompanyID == companiID && x.RoleID == 4).ToList();
                if(listper != null)
                {
                    foreach (var item in listper)
                    {
                        DeleteLeader(item.PersonID);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
            
        }

        //Xóa khóa học chưa có quản lý của công ty
        public bool DeleteListInternShipNo(string companiID)
        {
            try
            {
                WebDatabaseEntities database = new WebDatabaseEntities();
                var list = database.InternShip.Where(x => x.CompanyID == companiID && x.PersonID == null).ToList();
                if(list != null)
                {
                    foreach(var item in list)
                    {
                        new InternShipController().deleteInternShip(item.InternshipID);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        //cập nhật lại PersonID cho organzation
        public bool UpdatePersonOfOrganzation(string id)
        {
            try
            {
                WebDatabaseEntities database = new WebDatabaseEntities();
                var model = database.Organization.SingleOrDefault(x => x.ID == id);
                model.PersonID = "ZXCVBNML";
                database.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
            
        }

        public bool deleteOrganzation(string id)
        {
            try
            {
                WebDatabaseEntities database = new WebDatabaseEntities();
                var model = database.Organization.SingleOrDefault(x => x.ID == id);
                database.Organization.Remove(model);
                database.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }

        }


    }
}