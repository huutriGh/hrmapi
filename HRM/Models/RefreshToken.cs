using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRM.Models
{
    public class RefreshToken
    {
        public string ID { get; set; }
        public string UserName { get;  set; }
        public string ClientID { get;  set; }
        public DateTime IssuedTime { get; set; }
        public DateTime ExpiredTime { get; set; }
        public string ProtectedTicket { get; set; }
    }
}