using HRM.Models.Request;
using HRM.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM.Services
{
    public interface IEmployeeLeave
    {
        IEnumerable<EmployeeLeaveResponse> GetEmployeeLeaves(RangeDateRequest dateRequest, string businessEntityID, bool dashBoard = false);
        void insertUpdateRemoveEmployeeLeave(EmployeeLeaveRequest param, string userId, string businessEntityID);
      
        IEnumerable<LeavePendingApprove> GetLeavesPendingAprrove(dynamic data, string businessEntityID);

        /// <summary>
        /// Approve proccess
        /// </summary>
        /// <param name="data"></param>
        /// <param name="businessEntityID"></param>
        /// <param name="userId"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        LeaveApproveRequest Approve(LeaveApproveRequest data, string businessEntityID, string userId, string role);

    } 
}
