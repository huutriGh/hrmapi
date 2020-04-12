using HRM.Models.Response;
using HRM.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace HRM.Controllers
{
    [Authorize]
    [System.Web.Mvc.ValidateAntiForgeryToken]
    public class EmployeeController : ApiController
    {
        private readonly IEmployee employee;
        public EmployeeController(IEmployee employee)
        {
            this.employee = employee;
        }
        [Route("api/Employee/EmployeeLeaveGroup")]
        [HttpGet]
        public IEnumerable<EmployeeResponse> EmployeeLeaveGroup()
        {
            return employee.GetEmployeesWithDepartment();
        }

    }
}
