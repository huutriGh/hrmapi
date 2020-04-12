using HRM.Models;
using HRM.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Mvc;
using AuthorizeAttribute = System.Web.Http.AuthorizeAttribute;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;
using RouteAttribute = System.Web.Http.RouteAttribute;

namespace HRM.Controllers
{
   
    [Authorize]
    [ValidateAntiForgeryToken]
    public class LeaveController : ApiController
    {
        private readonly IApplication application;
        public LeaveController(IApplication application)
        {
            this.application = application;
        }
        [Route("api/Leave/GetLeaveType")]
        [HttpGet]
        public IEnumerable<LeaveType> GetLeaveType()
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var userId = claimsIdentity.FindFirst(claimsIdentity.NameClaimType)?.Value;
            return application.GetContext().LeaveType ;
        }
        [Route("api/Leave/GetLeaveStatus")]
        [HttpGet]
        public IEnumerable<LeaveStatus> GetLeaveStatus()
        {
            
            return application.GetContext().LeaveStatus.ToList();
        }

        // GET: api/Leave/5
        [Route("api/Leave/GetLeaveType/{id}")]
        [HttpGet]
        public LeaveType Get(string id)
        {
            return application.GetContext().LeaveType.Where(l => l.LeaveTypeId.Equals(id, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();
        }

        [Route("api/Leave/UpdateLeaveType")]
        [HttpPost]
        public IHttpActionResult UpdateLeaveType([FromBody]LeaveType leaveType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            

           application.GetContext().Entry(leaveType).State = EntityState.Modified;

            try
            {
                application.GetContext().SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new Exception(ex.Message.ToString());
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // PUT: api/Leave/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Leave/5
        public void Delete(int id)
        {

        }
    }
}
