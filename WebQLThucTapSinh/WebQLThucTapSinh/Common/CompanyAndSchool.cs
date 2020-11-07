using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebQLThucTapSinh.Models;

namespace WebQLThucTapSinh.Common
{
    public class CompanyAndSchool
    {
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
                string OriID;
                do
                {
                    OriID = new Share().RandomText();
                } while (new CompanyAndSchool().FindOrgan(OriID) == false);
                organ.ID = OriID;
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
                var model = database.Organization.Find(OriID);
                if (model == null)
                {
                    return false;
                }
                else
                {
                    string personID;
                    do
                    {
                        personID = share.RandomText();
                    } while (personID == OriID && FindPerson(personID) == false);
                    if (roleId == 2)
                    {
                        if (share.insertPerson(personID, OriID, null, 2))
                        {
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
                        if (share.insertPerson(personID, null, OriID, 3))
                        {
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

        public bool Update(Organization organ)
        {
            try
            {
                WebDatabaseEntities database = new WebDatabaseEntities();
                var model = database.Organization.Find(organ.ID);
                model.Name = organ.Name;
                model.Address = organ.Address;
                model.Fax = organ.Fax;
                model.Phone = organ.Phone;
                model.Image = organ.Image;
                model.Note = organ.Note;
                model.Email = organ.Email;
                model.Logo = organ.Logo;
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