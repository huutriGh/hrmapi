using HRM.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM.Services
{
    public interface IEmployee
    {
        IEnumerable<EmployeeResponse> GetEmployeesWithDepartment();
        IEnumerable<dynamic> GetAssignee(string BusinessEntityID);
        dynamic GetEmployeesRemainingHours(string BusinessEntityID);
    }
}
