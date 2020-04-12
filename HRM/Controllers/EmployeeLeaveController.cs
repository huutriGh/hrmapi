using HRM.Models;
using HRM.Models.Request;
using HRM.Models.Response;
using HRM.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Web.Http;
using AuthorizeAttribute = System.Web.Http.AuthorizeAttribute;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;
using RouteAttribute = System.Web.Http.RouteAttribute;

namespace HRM.Controllers
{

    [Authorize]
    [System.Web.Mvc.ValidateAntiForgeryToken]
    public class EmployeeLeaveController : ApiController
    {
     
        private readonly IEmployeeLeave employeeLeave;
        public EmployeeLeaveController(IEmployeeLeave employeeLeave)
        {
            this.employeeLeave = employeeLeave;
        }
        [Route("api/EmployeeLeave/GetEmployeeLeave")]
        [HttpPost]
        public IEnumerable<EmployeeLeaveResponse> GetEmployeeLeaves(RangeDateRequest dateRequest)
        {
            
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var businessEntityID = claimsIdentity.FindFirst(ClaimTypes.Actor)?.Value;
            return employeeLeave.GetEmployeeLeaves(dateRequest, businessEntityID);

        }
        [Route("api/EmployeeLeave/GetEmployeeLeaveDashBoard")]
        [HttpPost]
        public IEnumerable<EmployeeLeaveResponse> GetEmployeeLeavesDashBoard(RangeDateRequest dateRequest)
        {

            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var businessEntityID = claimsIdentity.FindFirst(ClaimTypes.Actor)?.Value;
            return employeeLeave.GetEmployeeLeaves(dateRequest, businessEntityID, true);

        }
        [Route("api/EmployeeLeave/GetLeavePendingApprove")]
        [HttpPost]
        public IEnumerable<LeavePendingApprove> GetLeavePendingApprove(dynamic para)
        {
          
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var businessEntityID = claimsIdentity.FindFirst(ClaimTypes.Actor)?.Value;
            return employeeLeave.GetLeavesPendingAprrove(para, businessEntityID);

        }  
        [Route("api/EmployeeLeave/Approve")]
        [HttpPost]
        public IHttpActionResult Approve(LeaveApproveRequest para)
        {
            try
            {
                var claimsIdentity = this.User.Identity as ClaimsIdentity;
                var businessEntityID = claimsIdentity.FindFirst(ClaimTypes.Actor)?.Value;
                var userId = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
                var role = claimsIdentity.FindFirst(ClaimTypes.Role)?.Value;
                employeeLeave.Approve(para, businessEntityID, userId, role);
                return StatusCode(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {

                return Content(HttpStatusCode.Forbidden, ex.Message);
            }
          
            
         

        } 

        [Route("api/EmployeeLeave/UpdateData")]
        [HttpPost]
        public IHttpActionResult insertUpdateRemoveEmployeeLeave(EmployeeLeaveRequest param)
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
            var businessEntityID = claimsIdentity.FindFirst(ClaimTypes.Actor)?.Value;
            employeeLeave.insertUpdateRemoveEmployeeLeave(param, userId, businessEntityID);
            return Ok();
          
        }
       
    }
}
