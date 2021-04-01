using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskOrganizer.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Department { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string RecoveryQuestion { get; set; }
        public string RecoveryAns { get; set; }
        public string RecoveryEmail { get; set; }
        public string Role { get; set; }
        public string Mobile { get; set; }
        public string LoginId { get; set; }
    }
}