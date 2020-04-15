using HRM.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace HRM.Models
{
    public class ApplicationContext : DbContext , IApplication
    {
        public ApplicationContext() : base("DefaultConnection")
        {
            Database.SetInitializer<ApplicationContext>(null);
        }
        public DbSet<User> users { get; set; }
        public DbSet<UserFunction> UserFunction { get; set; }
        public DbSet<LeaveType> LeaveType { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<EmployeeLeave> EmployeeLeave { get; set; }
        public DbSet<LeaveStatus> LeaveStatus { get; set; }
        public DbSet<SystemConfig> SystemConfig { get; set; }


        public ApplicationContext GetContext()
        {
            return this;
        }
    }
}