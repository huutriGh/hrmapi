using HRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM.Services
{
    public interface IHelper
    {
        void SendEmail(string subject, List<EmployeeLeave> mainContent, string mailTo, string cc);
        string Encrypt(string input);
        bool validPassword(string password, string hashedPassword);
    }
}
