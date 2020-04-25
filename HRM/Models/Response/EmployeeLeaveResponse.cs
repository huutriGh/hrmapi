using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRM.Models.Response
{
    public class EmployeeLeaveResponse
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Residence { get; set; }
        public string cLocation { get; set; }
        public string TellOrEmail { get; set; }
        public string PersonCover { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsAllDay { get; set; }
        public bool IsHalfDay { get; set; }
        public string CategoryColor { get; set; }
        public Int16 DepartmentID { get; set; }
        public string BusinessEntityID { get; set; }
        public bool IsReadonly { get; set; }
    }
}