using HRM.Models;
using HRM.Models.Response;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
            sb.AppendLine("D.DepartmentID, E.BusinessEntityID, E.FirstName + ' ' + E.LastName as EmployeeName, '#3d5afe' as Color");
            sb.AppendLine("FROM ");
            sb.AppendLine("Employee E");
            sb.AppendLine("Inner join EmployeeDepartmentHistory H ON H.BusinessEntityID = E.BusinessEntityID");
            sb.AppendLine("left join Department D ON H.DepartmentID = D.DepartmentID");
            var response = application.GetContext().Database.SqlQuery<EmployeeResponse>(sb.ToString()).ToList();
            return response;
        }
        public dynamic GetEmployeesRemainingHours(string BusinessEntityID)
        {
           var response =  application.GetContext().Database.SqlQuery<Employee>("select RemainingVacationHours,PreviousYearVacationHours,WorkOnSaturday from Employee where BusinessEntityID  =@BusinessEntityID", new SqlParameter("@businessEntityID", BusinessEntityID)).FirstOrDefault();
           
                var RemainingVacationHours = (response.RemainingVacationHours / (8.0*60));
                var PreviousYearVacationHours = (response.PreviousYearVacationHours / (8.0*60));
                return new  {RemainingVacationHours, PreviousYearVacationHours };
        }

        public IEnumerable<dynamic> GetAssignee(string BusinessEntityID)
        {
            StringBuilder sb = new StringBuilder();
            var node = application.GetContext().Database.SqlQuery<Int16>("select OrganizationLevel from Employee where businessEntityID = @businessEntityID", new SqlParameter("@businessEntityID", BusinessEntityID)).SingleOrDefault();
            sb.AppendLine("DECLARE @Manager hierarchyid, @lv tinyint");
            sb.AppendLine("SELECT @Manager = OrganizationNode,@lv= OrganizationLevel from  Employee where BusinessEntityID = @businessEntityID");
            sb.AppendLine("select E.BusinessEntityID, QUOTENAME(E.BusinessEntityID) + E.FullName as FullName from Employee E");
            sb.AppendLine("left join ");
            sb.AppendLine("EmployeeDepartmentHistory EH on e.BusinessEntityID = EH.BusinessEntityID");
            sb.AppendLine("left join Users U on E.BusinessEntityID = u.BusinessEntityID");
            sb.AppendLine("left join UserFunction UF on u.UserID = UF.UserId ");
            sb.AppendLine("where 1 =1");
            if(node > 2)
            {
                sb.AppendLine("and DepartmentID  = (");
                sb.AppendLine("SELECT D.DepartmentID FROM  Employee E");
                sb.AppendLine("Inner join EmployeeDepartmentHistory H ON H.BusinessEntityID = E.BusinessEntityID");
                sb.AppendLine("left join Department D ON H.DepartmentID = D.DepartmentID");
                sb.AppendLine("where e.BusinessEntityID = @businessEntityID) ");
            }

            //if (node == 5)
            //{
            //    sb.AppendLine("and  uf.FunctionID in ('F1','F2','F3')");
            //}
            //else
            //{
            //    sb.AppendLine("and @Manager.IsDescendantOf(OrganizationNode)=1");
            //    sb.AppendLine("and E.BusinessEntityID <> @businessEntityID");
            //}
            sb.AppendLine("and  uf.FunctionID in ('F1','F2','F3')");
            sb.AppendLine("and E.BusinessEntityID <> @businessEntityID");
            sb.AppendLine("and OrganizationLevel < @lv");
            //sb.AppendLine("or E.BusinessEntityID = @businessEntityID");
            sb.AppendLine("group by  E.BusinessEntityID, E.FullName");

            var res = application.GetContext().Database.SqlQuery<Employee>(sb.ToString(), new SqlParameter("@businessEntityID", BusinessEntityID)).Select(e => new { e.BusinessEntityID, e.FullName }).ToList();
            return res;



        }
    }
}