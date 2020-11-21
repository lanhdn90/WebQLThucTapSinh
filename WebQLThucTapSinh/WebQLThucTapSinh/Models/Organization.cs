//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebQLThucTapSinh.Models
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public partial class Organization
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Organization()
        {
            this.InternShip = new HashSet<InternShip>();
            this.Person = new HashSet<Person>();
            this.Person1 = new HashSet<Person>();
        }
    
        public string ID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Image { get; set; }
        public string Logo { get; set; }
        [AllowHtml]
        public string Note { get; set; }
        public string Email { get; set; }
        public Nullable<System.DateTime> StartDay { get; set; }
        public Nullable<int> ExpiryDate { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<bool> SendEmail { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InternShip> InternShip { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Person> Person { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Person> Person1 { get; set; }
    }
}
