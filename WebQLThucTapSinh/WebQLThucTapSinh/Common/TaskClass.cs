using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebQLThucTapSinh.Common
{
    public class TaskClass
    {
        public int TaskID { get; set; }
        public string TaskName { get; set; }
        public string Note { get; set; }
        public string Video { get; set; }
        public Nullable<int> Questions { get; set; }
        public Nullable<int> NumberOfQuestions { get; set; }
        public Nullable<int> Result { get; set; }
        public string PersonID { get; set; }
        public string FullName { get; set; }
    }
}