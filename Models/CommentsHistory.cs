using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskOrganizer.Models
{
    public class CommentsHistory
    {
        public int CommentId { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<int> TaskId { get; set; }
        public string Commenter { get; set; }
        public string Comment { get; set; }
        public DateTime Commented_On { get; set; }
    }
}