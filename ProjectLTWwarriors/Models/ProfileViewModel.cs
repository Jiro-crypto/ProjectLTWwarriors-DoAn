using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectLTWwarriors.Models
{
    public class ProfileViewModel
    {
        public User User { get; set; }
        public List<DonHang> DonHangs { get; set; }
    }

}