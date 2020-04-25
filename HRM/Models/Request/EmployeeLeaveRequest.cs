using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRM.Models.Request
{
    public class EmployeeLeaveRequest
    {
        public string action { get; set; }
        public List<ScheduleLeave> added { get; set; }
        public List<ScheduleLeave> changed { get; set; }
        public List<ScheduleLeave> deleted { get; set; }
        public int key { get; set; }


        public class ScheduleLeave
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
            public string StartTimezone { get; set; }
            public string EndTimezone { get; set; }

        }
    }

}