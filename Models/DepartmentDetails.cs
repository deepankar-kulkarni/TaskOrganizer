using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskOrganizer.Models
{
    public class DepartmentDetails
    {
        public int DeptId { get; set; }
        public string DepartmentName { get; set; }
        public Nullable<int> ManagerId { get; set; }
        public string ManagerName { get; set; }
    }
}