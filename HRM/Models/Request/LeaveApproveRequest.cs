using HRM.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRM.Models.Request
{
    public class LeaveApproveRequest
    {
        public string action { get; set; }
        public IEnumerable<LeavePendingApprove> changed { get; set; }
        public IEnumerable<LeavePendingApprove> added { get; set; }
        public IEnumerable<LeavePendingApprove> deleted { get; set; }

    }
}