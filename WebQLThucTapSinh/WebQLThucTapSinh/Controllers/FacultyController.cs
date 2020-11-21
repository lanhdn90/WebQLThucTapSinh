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
            //var company = Session["CompanyID"].ToString();
            var company = "ASDFGHJR";
            var list = (from a in database.Person
                        join b in database.Organization on a.CompanyID equals b.ID
                        where a.CompanyID == company && a.RoleID == 3
                        select new RepresentativeClass()
                        {
                            SchoolID = a.SchoolID,
                            Fax = b.Fax,
                            Address = b.Address,
                            Phone = b.Phone,
                            Email = b.Email,
                            Status = b.Status,
                        }).ToList();
            List<RepresentativeClass> listFaculty = new List<RepresentativeClass>();
            foreach (var item in list)
            {
                var count = list.Where(x => x.SchoolID == item.SchoolID).Count();
                if(count == 1)
                {
                    listFaculty.Add(item);
                }
                else
                {
                    var number = listFaculty.Where(x => x.SchoolID == item.SchoolID).Count();
                    if(number == 0)
                    {
                        listFaculty.Add(item);
                    }
                }
            }
            return View(listFaculty);
        }
    }
}