﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRM.Models.Response
{
    public class LeavePendingApprove
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public string Sumary { get; set; }
        public string Priority { get; set; }
        public string Tags { get; set; }
        public string Assignee { get; set; }
        public string RankId { get; set; }

    }
}