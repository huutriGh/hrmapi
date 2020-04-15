using HRM.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HRM.Services.ServiceImp
{
    public class DepartmentImp : IDepartment
    {
        private readonly IApplication application;
        public DepartmentImp(IApplication application)
        {
            this.application = application;
        }
        public IEnumerable<DepartmentResponse> getDepartment()
        {
            string sql = "SELECT  DepartmentID, Name DepartmentName  , '#3d5afe' as Color from Department";
            var response = application.GetContext().Database.SqlQuery<DepartmentResponse>(sql).ToList();
            return response;
        }
    }
}