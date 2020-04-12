using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRM.Models
{
    public class User
    {
        public string UserID { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string BusinessEntityID { get; set; }
    }
}