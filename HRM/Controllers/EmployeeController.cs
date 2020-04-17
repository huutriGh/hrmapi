using HRM.Models.Response;
using HRM.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
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
        [Route("api/Employee/EmployeeLeaveRemainingHours")]
        [HttpGet]
        public IHttpActionResult GetEmployeeLeaveRemainingHours()
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var businessEntityID = claimsIdentity.FindFirst(ClaimTypes.Actor)?.Value;
            return Ok(employee.GetEmployeesRemainingHours(businessEntityID));
        }

    }
}
