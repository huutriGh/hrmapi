
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;



namespace HRM.Models
{
    [Table("EmployeeLeave")]
    public class EmployeeLeave
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LeaveId { get; set; }
        public string BusinessEntityID { get; set; }
        public string LeaveTypeId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Residence { get; set; }
        public string Contact { get; set; }
        public string PersonToCover { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Intime { get; set; }
        public string UserId { get; set; }
        public bool IsAllDay { get;  set; }
        public string ToLocation { get;  set; }
        
        public byte Status { get; set; }
        public string PersonApproved { get; set; }
        public string PersonVerified { get; set; }
        
        public DateTime ? DateApproved { get; set; }
       
        public DateTime ? DateVerified { get; set; }
        public DateTime ? DateApplied { get; set; }
        public Int16 DepartmentId { get; set; }

       
    }
}