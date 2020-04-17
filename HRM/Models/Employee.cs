using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRM.Models
{
    public class Employee
    {
        public Int16 RemainingVacationHours { get; set; }
        public Int16 PreviousYearVacationHours { get; set; }
        public bool WorkOnSaturday { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string FirstName { get; set; }
        public string FullName { get; internal set; }
    }
}