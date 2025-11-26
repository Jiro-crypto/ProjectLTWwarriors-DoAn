using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectLTWwarriors.Models
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }

        public DateTime NgaySinh { get; set; }
        public string Email { get; set; }
    }
}