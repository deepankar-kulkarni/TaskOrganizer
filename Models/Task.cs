using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TaskOrganizer.Models
{
    public class Task
    {
        public int TaskId { get; set; }
        public Nullable<int> UserId { get; set; }
        public string TaskName { get; set; }
        public string Assignee { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DueDate { get; set; }
        public string TaskDescription { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> CompletedDate { get; set; }
        public string CompletedBy { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> TaskCreationDate { get; set; }
        public bool Status { get; set; }
        public string Stage { get; set; }
        public int Priority { get; set; }
        public priority Prioritytext{ get; set; }
        public string comment { get; set; }
        public int TaskCount { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> FromDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> ToDate { get; set; }
        public List<String> taskHistory { get; set; }
        public List<CommentsHistory> commentHistory = new List<CommentsHistory>();
    }
}