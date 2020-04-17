using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HRM.Models
{

    [Table("SystemConfig")]
    public class SystemConfig
    {
        [Key]
        public int Id { get; set; }
        public string Email { get; set; }
        public string EmailUser { get; set; }
        public string PasswordEmail { get; set; }
        public string EmailTemplate { get; set; }

        public string MailServer { get; set; }
        public string EmailTemplateAprroved { get; set; }
    }
}