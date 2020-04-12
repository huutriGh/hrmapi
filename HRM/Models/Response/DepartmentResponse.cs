using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRM.Models.Response
{
    public class DepartmentResponse
    {
        public Int16 DepartmentID { get; set; }
        public string DepartmentName { get; set; }

        public string Color { get; set; }
    }
}