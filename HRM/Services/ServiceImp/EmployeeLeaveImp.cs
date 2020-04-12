using HRM.Models;
using HRM.Models.Request;
using HRM.Models.Response;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace HRM.Services.ServiceImp
{
    public class EmployeeLeaveImp : IEmployeeLeave
    {
        private readonly IApplication application;
        public EmployeeLeaveImp(IApplication application)
        {
            this.application = application;
        }

        public LeaveApproveRequest Approve(LeaveApproveRequest param, string businessEntityID, string userId, string role)
        {
            if (param.action == "update" || (param.action == "batch" && param.changed.Count() > 0)) // this block of code will execute while inserting the appointments
            {
                var lRole = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(role);
                var funtionId = application.GetContext().UserFunction.Where(u => u.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase));
                StringBuilder sb = new StringBuilder();
              
                foreach (var value in param.changed)
                {
                    sb.Clear();
                    //Apply not your mine
                    if (value.Status.Equals("Applied", StringComparison.OrdinalIgnoreCase) && !businessEntityID.Equals(value.RankId))
                    {
                        throw new Exception("You can't not apply other's Leave.");
                    }
                    else if(value.Status.Equals("Verified", StringComparison.OrdinalIgnoreCase) && !lRole.Contains("F2"))
                    {
                        throw new Exception("Permission is deny. You can not Verify this Leave");
                    }
                    else if (value.Status.Equals("Approved", StringComparison.OrdinalIgnoreCase) && !lRole.Contains("F1"))
                    {
                        throw new Exception("Permission is deny. You can not Approve this Leave.");
                    }

                    sb.AppendLine("select L.LeaveStatusDesc from EmployeeLeave EL");
                    sb.AppendLine("left join LeaveStatus L on EL.Status = L.LeaveStatusId");
                    sb.AppendLine("where LeaveId = @LeaveId");
                    var currentLeaveStatus = application.GetContext().Database.SqlQuery<string>(sb.ToString(), new SqlParameter("@LeaveId", value.Id)).First().ToString();

                    if(currentLeaveStatus.Equals("Approved", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new Exception("Permission is deny. You can not  change status of this Leave that Approved ");
                    }
                    else if(currentLeaveStatus.Equals("Verified", StringComparison.OrdinalIgnoreCase) && (value.Status.Equals("Applied", StringComparison.OrdinalIgnoreCase) || value.Status.Equals("Created", StringComparison.OrdinalIgnoreCase)))
                    {
                        throw new Exception("Permission is deny. You can not  change status of this Leave back to  Applied or Created ");
                    }


                    sb.Clear();

                    if (value.Status.Equals("Applied", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.AppendLine("update EmployeeLeave set DateApplied = getdate(), Status = 1 where LeaveId =@LeaveId and businessEntityID =@businessEntityID and Status = 0");

                    }
                    else if (value.Status.Equals("Verified", StringComparison.OrdinalIgnoreCase) && funtionId.Where(f => f.FunctionID.Equals("F2", StringComparison.OrdinalIgnoreCase)).Count() > 0)
                    {
                        sb.AppendLine("update EmployeeLeave set DateVerified = getdate(), PersonVerified = @businessEntityID, Status = 2 where LeaveId =@LeaveId and Status = 1");

                    }
                    else if (value.Status.Equals("Approved", StringComparison.OrdinalIgnoreCase) && funtionId.Where(f => f.FunctionID.Equals("F1", StringComparison.OrdinalIgnoreCase)).Count() > 0)
                    {
                        sb.AppendLine("update EmployeeLeave set DateApproved = getdate(), PersonApproved = @businessEntityID, Status = 3 where LeaveId =@LeaveId and status = 2");
                    }

                    var parameter = new SqlParameter[]
                    {
                    new SqlParameter("@businessEntityID", businessEntityID),
                    new SqlParameter("@LeaveId", value.Id)
                    };
                    if (sb.Length > 0)
                    {
                        int rowEffect = application.GetContext().Database.ExecuteSqlCommand(sb.ToString(), parameter);
                    }


                }

            }
            return param;
        }

      

        public IEnumerable<EmployeeLeaveResponse> GetEmployeeLeaves(RangeDateRequest dateRequest, string businessEntityID, bool dashBoard = false)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT l.LeaveId AS Id, l.LeaveTypeId AS Subject, l.Residence , l.ToLocation AS cLocation,");
            sb.AppendLine("L.Contact AS TellOrEmail, l.PersonToCover as PersonCover, l.StartTime, l.EndTime, l.IsAllDay, T.CategoryColor,");
            sb.AppendLine("case when L.Status = 0 then cast (0 as bit) else cast (1 as bit) end as IsReadonly,");
            sb.AppendLine("D.DepartmentID, E.BusinessEntityID");
            sb.AppendLine("FROM ");
            sb.AppendLine("Employee E inner join EmployeeLeave L ON E.BusinessEntityID = L.BusinessEntityID");
            sb.AppendLine("left join LeaveType T ON L.LeaveTypeId = T.LeaveTypeId");
            sb.AppendLine("Inner join EmployeeDepartmentHistory H ON H.BusinessEntityID = E.BusinessEntityID");
            sb.AppendLine("left join Department D ON H.DepartmentID = D.DepartmentID");
            sb.AppendLine("Where L.StartTime between @StartDate and @EndDate");
            var paramter = new List<SqlParameter>();
            paramter.Add(new SqlParameter("@StartDate", dateRequest.StartDate.ToString("yyyy-MM-dd 00:00:00")));
            paramter.Add(new SqlParameter("@EndDate", dateRequest.EndDate.ToString("yyyy-MM-dd 23:59:59")));

            if (!dashBoard)
            {
                sb.AppendLine("and E.BusinessEntityID= @BusinessEntityID");
                paramter.Add(new SqlParameter("@BusinessEntityID", businessEntityID));
            }
           
            // var response = application.GetContext().Database.SqlQuery<EmployeeLeaveResponse>(sb.ToString(), new SqlParameter("@BusinessEntityID", businessEntityID)).ToList();
            var response = application.GetContext().Database.SqlQuery<EmployeeLeaveResponse>(sb.ToString(), paramter.ToArray()).ToList();
            
            return response;
        }

        public IEnumerable<LeavePendingApprove> GetLeavesPendingAprrove(dynamic data, string businessEntityID)
        {
            StringBuilder sb = new StringBuilder();
            
            sb.AppendLine("DECLARE @Manager hierarchyid  ");
            sb.AppendLine("SELECT @Manager = OrganizationNode from  Employee where BusinessEntityID = @BusinessEntityID");
            sb.AppendLine("select EL.LeaveId as Id,EL.BusinessEntityID as RankId, cast (EL.LeaveId as nvarchar(10)) + ' - ' + LT.Description as Title,LS.LeaveStatusDesc as Status,");
            sb.AppendLine("null as Sumary, 'Normal' as Priority,CONVERT(varchar(10),StartTime, 103) + ' - ' + CONVERT(varchar(10),EndTime, 103) as Tags,");
            sb.AppendLine("QUOTENAME(EL.BusinessEntityID)  + E.FullName as Assignee");
            sb.AppendLine("from Employee E ");
            sb.AppendLine("inner join EmployeeLeave EL on E.BusinessEntityID = El.BusinessEntityID");
            sb.AppendLine("left join LeaveStatus LS on EL.Status = LS.LeaveStatusId");
            sb.AppendLine("left join LeaveType LT on EL.LeaveTypeId = LT.LeaveTypeId");
            sb.AppendLine("where E.BusinessEntityID in (");
            sb.AppendLine("select BusinessEntityID");
            sb.AppendLine("from Employee");
            sb.AppendLine("where (OrganizationNode.IsDescendantOf(@Manager)=1)");
            var node = application.GetContext().Database.SqlQuery<Int16>("select OrganizationLevel from Employee where BusinessEntityID =@businessEntityID", new SqlParameter("@BusinessEntityID", businessEntityID)).First();
            if (node < 1)
            {
                sb.AppendLine("and OrganizationLevel <=1 )");
            }
            else
            {
                sb.AppendLine("and OrganizationNode <> hierarchyid::GetRoot() )");
            }
            var response = application.GetContext().Database.SqlQuery<LeavePendingApprove>(sb.ToString(), new SqlParameter("@BusinessEntityID", businessEntityID)).ToList();
            return response;
        }

        public void insertUpdateRemoveEmployeeLeave(EmployeeLeaveRequest param, string userId, string businessEntityID)

        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("select top 1 ED.DepartmentID from EmployeeDepartmentHistory ED");
            sb.AppendLine("where ED.BusinessEntityID =@BusinessEntityID and EndDate is null");
            var departmentId = application.GetContext().Database.SqlQuery<Int16>(sb.ToString(), new SqlParameter("@BusinessEntityID", businessEntityID)).FirstOrDefault();
            
            if (param.action == "insert" || (param.action == "batch" && param.added.Count > 0)) // this block of code will execute while inserting the appointments
            {
                var value = param.added[0];
                
                EmployeeLeave appointment = new EmployeeLeave()
                {

                    StartTime = value.StartTime.ToLocalTime(),
                    EndTime = value.EndTime.ToLocalTime(),
                    LeaveTypeId = value.Subject,
                    IsAllDay = value.IsAllDay,
                    Residence = value.Residence,
                    ToLocation = string.IsNullOrEmpty(value.cLocation) ? null: value.cLocation,
                    Contact = value.TellOrEmail,
                    PersonToCover = value.PersonCover,
                    UserId = userId,
                    BusinessEntityID = businessEntityID,
                    Status = 0,
                    DepartmentId = departmentId


                };
                if (application.GetContext().EmployeeLeave.ToList().Contains(appointment))
                {
                    return;
                }
                application.GetContext().EmployeeLeave.Add(appointment);
                application.GetContext().SaveChanges();
            }
            if (param.action == "update" || (param.action == "batch" && param.changed.Count > 0)) // this block of code will execute while updating the appointment
            {
                var value = param.changed[0];
                var filterData = application.GetContext().EmployeeLeave.Where(c => c.LeaveId == value.Id);
                if (filterData.Count() > 0)
                {

                    EmployeeLeave appointment = application.GetContext().EmployeeLeave.Single(A => A.LeaveId == value.Id);
                    appointment.StartTime = value.StartTime.ToLocalTime();
                    appointment.EndTime = value.EndTime.ToLocalTime();
                    appointment.LeaveTypeId = value.Subject;
                    appointment.IsAllDay = value.IsAllDay;
                    appointment.Residence = value.Residence;
                    appointment.ToLocation = value.cLocation;
                    appointment.Contact = value.TellOrEmail;
                    appointment.PersonToCover = value.PersonCover;
                    appointment.UserId = userId;
                    appointment.BusinessEntityID = businessEntityID;
                    appointment.DepartmentId = departmentId;

                }
                application.GetContext().SaveChanges();
            }
            if (param.action == "remove" || (param.action == "batch" && param.deleted.Count > 0)) // this block of code will execute while removing the appointment
            {
                if (param.action == "remove")
                {

                    EmployeeLeave appointment = application.GetContext().EmployeeLeave.Where(c => c.LeaveId == param.key).FirstOrDefault();
                    if (appointment != null)
                    {
                        application.GetContext().EmployeeLeave.Remove(appointment);
                    }
                }
                else
                {

                    foreach (var apps in param.deleted)
                    {

                        EmployeeLeave appointment = application.GetContext().EmployeeLeave.Where(c => c.LeaveId == apps.Id).FirstOrDefault();
                        if (apps != null)
                        {
                            application.GetContext().EmployeeLeave.Remove(appointment);
                        }
                    }
                }
                application.GetContext().SaveChanges();
            }
         //   return Json(application.GetContext().EmployeeLeave.ToList());
        }
    }
}