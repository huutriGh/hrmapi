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
    public class DepartmentController : ApiController
    {
        private readonly IDepartment department;
        public DepartmentController(IDepartment department)
        {
            this.department = department;
        }
      
        [Route("api/Department/getDepartment")]
        [HttpGet]
        public IEnumerable<dynamic> getDepartment()
        {

            return department.getDepartment();
        }

       
    }
}
