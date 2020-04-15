using HRM.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace HRM.Services.ServiceImp
{
    public class EmmloyeeImp : IEmployee
    {
        private readonly IApplication application;
        public EmmloyeeImp(IApplication application)
        {
            this.application = application;
        }
        public IEnumerable<EmployeeResponse> GetEmployeesWithDepartment()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT");
            sb.AppendLine("D.DepartmentID, E.BusinessEntityID, E.LastName + ' ' + E.FirstName as EmployeeName, '#3d5afe' as Color");
            sb.AppendLine("FROM ");
            sb.AppendLine("Employee E");
            sb.AppendLine("Inner join EmployeeDepartmentHistory H ON H.BusinessEntityID = E.BusinessEntityID");
            sb.AppendLine("left join Department D ON H.DepartmentID = D.DepartmentID");
            var response = application.GetContext().Database.SqlQuery<EmployeeResponse>(sb.ToString()).ToList();
            return response;
        }
    }
}