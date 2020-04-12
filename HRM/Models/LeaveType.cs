using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HRM.Models
{
   
    [Table("LeaveType")]
    public class LeaveType
    {
        [Key]
        public string LeaveTypeId { get; set; }
        public string Description { get; set; }
        public string CategoryColor { get; set; }
    }
}