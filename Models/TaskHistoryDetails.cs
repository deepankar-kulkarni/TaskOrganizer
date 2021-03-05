using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskOrganizer.Models
{
    public class TaskHistoryDetails
    {
        public int id { get; set; }
        public Nullable<int> UserId { get; set; }
        public int TaskId { get; set; }
        public string Action { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
    }
}