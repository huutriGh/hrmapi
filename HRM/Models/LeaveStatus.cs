using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HRM.Models
{
    [Table("LeaveStatus")]
    public class LeaveStatus
    {
        [Key]
        public byte LeaveStatusId { get; set; }
        public string LeaveStatusDesc { get; set; }
    }
}