using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebQLThucTapSinh.Common
{
    public class RepresentativeClass
    {
        public string PersonID { get; set; }
        public string NameOfCompany { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public Nullable<bool> Status { get; set; }
        public string CompanyID { get; set; }
        public string SchoolID { get; set; }
    }
}