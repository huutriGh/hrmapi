using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRM.Models.Response
{
    public class EmployeeResponse
    {
        public Int16 DepartmentID { get; set; }
        public string BusinessEntityID { get; set; }
        public string EmployeeName { get; set; }
        public string Color { get; set; }
    }
}