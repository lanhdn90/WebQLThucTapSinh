using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

        public bool insertPerson(string code, string company, string school, int role)
        {
            
            try
            {
                WebDatabaseEntities database = new WebDatabaseEntities();
                Person person = new Person();
                person.PersonID = code;
                person.RoleID = role;
                person.CompanyID = company;
                person.SchoolID = school;
                database.Person.Add(person);
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