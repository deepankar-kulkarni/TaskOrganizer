//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TaskOrganizer.database
{
    using System;
    using System.Collections.Generic;
    
    public partial class CommentHistory
    {
        public int CommentId { get; set; }
        public Nullable<int> UserId { get; set; }
        public int TaskId { get; set; }
        public string Commenter { get; set; }
        public string Comment { get; set; }
        public Nullable<System.DateTime> Commented_On { get; set; }
        public string Created_By { get; set; }
        public string Modified_By { get; set; }
        public Nullable<System.DateTime> Created_On { get; set; }
        public Nullable<System.DateTime> Modified_On { get; set; }
    }
}
