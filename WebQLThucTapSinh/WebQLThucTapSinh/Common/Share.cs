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
    }
}