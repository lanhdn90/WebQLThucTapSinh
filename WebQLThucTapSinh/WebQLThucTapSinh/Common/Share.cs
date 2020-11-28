using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebQLThucTapSinh.Controllers;
using WebQLThucTapSinh.Models;

namespace WebQLThucTapSinh.Common
{
    public class Share
    {
        public string RandomText()
        {
            Random rd = new Random();
            string TextRd = null;
            string Text;
            for (int i = 0; i < 8; i++)
            {
                Text = Convert.ToString((char)rd.Next(65, 90));
                TextRd = TextRd + Text;
            }
            return TextRd;
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

        public bool InsertPerson(Person person)
        {
            try
            {
                WebDatabaseEntities database = new WebDatabaseEntities();
                database.Person.Add(person);
                database.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
            
        }

        public bool ChangeStatusUser(string id)
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            var com = database.Users.SingleOrDefault(x => x.PersonID == id);
            if (com.Status == true)
            {
                com.Status = false;
            }
            else
            {
                com.Status = true;

            }
            database.SaveChanges();
            return com.Status;
        }

        public bool Update(Person person)
        {
            try
            {
                WebDatabaseEntities database = new WebDatabaseEntities();
                var model = database.Person.Find(person.PersonID);
                model.LastName = person.LastName;
                model.FirstName = person.FirstName;
                model.Birthday = person.Birthday;
                model.Gender = person.Gender;
                model.Address = person.Address;
                model.Phone = person.Phone;
                model.Email = person.Email;
                model.Image = person.Image;
                if(person.CompanyID == null)
                {
                    model.CompanyID = model.CompanyID;
                }
                else
                {
                    model.CompanyID = person.CompanyID;
                }
                if(person.SchoolID == null)
                {
                    model.SchoolID = model.SchoolID;
                }
                else
                {
                    model.SchoolID = person.SchoolID;
                }
                
                database.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateIntern(string schoolId, string newSchoolID)
        {
            try
            {
                WebDatabaseEntities database = new WebDatabaseEntities();
                var listIntern = database.Person.Where(x => x.SchoolID == schoolId && x.RoleID == 5).ToList();
                if (listIntern != null)
                {
                    foreach (var item in listIntern)
                    {
                        item.SchoolID = newSchoolID;
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

        //Xóa 1 người
        public bool DeletePerson(string id)
        {
            try
            {
                WebDatabaseEntities database = new WebDatabaseEntities();
                var userName = database.Users.SingleOrDefault(c => c.PersonID == id);
                if (userName != null)
                {
                    database.Users.Remove(userName);
                    database.SaveChanges();
                }
                var model = database.Person.Find(id);
                database.Person.Remove(model);
                database.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool InsertOrganization(Organization organization)
        {
            try
            {
                WebDatabaseEntities database = new WebDatabaseEntities();
                string comID;
                do
                {
                    comID = RandomText();
                } while (FindOrgan(comID) == false);
                organization.ID = comID;
                database.Organization.Add(organization);
                database.SaveChanges();
                return true;
            }
            catch
            {
                return false;
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

        public List<Organization> listOrgan(int roleid)
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            List<Person> listPerson = database.Person.Where(x => x.RoleID == roleid).ToList();
            List<Organization> listOrgan = new List<Organization>();
            if (roleid == 2)
            {
                foreach (var item in listPerson)
                {
                    var model = database.Organization.Find(item.CompanyID);
                    listOrgan.Add(model);
                }
            }
            else
            {
                foreach (var item in listPerson)
                {
                    var model = database.Organization.Find(item.SchoolID);
                    listOrgan.Add(model);
                }
            }
            return listOrgan;
        }

        // Caaph nhật trrangj thái của công ty
        public bool ChangeStatus(string id, int role)
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            var com = database.Organization.Find(id);
            if (com.Status == true)
            {
                ChangesStatusOrgan(id, role, true);
                com.Status = false;
            }
            else
            {
                ChangesStatusOrgan(id, role, false);
                com.Status = true;
            }
            database.SaveChanges();
            return com.Status;

        }

        public void ChangesStatusOrgan(string id, int role, bool status)
        {
            WebDatabaseEntities database = new WebDatabaseEntities();
            var findP = database.Person.SingleOrDefault(x => x.CompanyID == id && x.RoleID == role);
            var findU = database.Users.SingleOrDefault(x => x.PersonID == findP.PersonID);
            if (findU != null)
            {
                findU.Status = !status;
            }
            database.SaveChanges();
            List<Person> list = new List<Person>();
            if (role == 2)
            {
                list = database.Person.Where(x => x.CompanyID == id && x.RoleID == 4).ToList();
            }
            else
            {
                list = database.Person.Where(x => x.CompanyID == id && x.RoleID == 3).ToList();
            }
            if(list != null)
            {
                if (role == 2)
                {
                    foreach (var item in list)
                    {
                        if(new LeaderController().ChangeStatusLeader(item.PersonID) == status)
                        {
                            new LeaderController().ChangeStatusLeader(item.PersonID);
                        }
                    }
                }
                else
                {
                    foreach (var item in list)
                    {
                        if (ChangeStatusUser(item.PersonID) == status)
                        {
                            ChangeStatusUser(item.PersonID); 
                        }
                    }
                }
            }
        }

        public bool DeleteOrganzation(string id, int roleId)
        {
            if(new Company().UpdatePersonOfOrganzation(id))
            {
                WebDatabaseEntities database = new WebDatabaseEntities();
                Person person = new Person();
                if (roleId == 2)
                {
                    person = database.Person.SingleOrDefault(x => x.CompanyID == id && x.RoleID == 2);
                }
                else
                {
                    person = database.Person.SingleOrDefault(x => x.CompanyID == id && x.RoleID == 6);
                }
                if(new Share().DeletePerson(person.PersonID))
                {
                    return new Company().deleteOrganzation(id);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

    }
}