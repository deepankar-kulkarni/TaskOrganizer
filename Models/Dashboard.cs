using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskOrganizer.Models
{
    public class Dashboard
    {
        public string Assignee { get; set; }
        public Nullable<int> Pending { get; set; }
        public Nullable<int> OnHold { get; set; }
        public Nullable<int> InProgress { get; set; }
        public Nullable<int> Completed { get; set; }
        public Nullable<int> NotStarted { get; set; }
    }
}