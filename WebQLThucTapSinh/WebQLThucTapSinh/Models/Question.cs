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
    
    public partial class Question
    {
        public int QuestionID { get; set; }
        public Nullable<int> TaskID { get; set; }
        public string Content { get; set; }
        public string Answer { get; set; }
        public string A { get; set; }
        public string B { get; set; }
        public string C { get; set; }
        public string D { get; set; }
    
        public virtual Task Task { get; set; }
    }
}
