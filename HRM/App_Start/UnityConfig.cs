using HRM.Models;
using HRM.Services;
using HRM.Services.ServiceImp;
using System.Web.Http;
using Unity;
using Unity.WebApi;

namespace HRM
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();
           
            container.RegisterType<IAccount, AccountImp>();
            container.RegisterType<IApplication, ApplicationContext>(); 
            container.RegisterType<IEmployeeLeave, EmployeeLeaveImp>();
            container.RegisterType<IEmployee, EmmloyeeImp>();
            container.RegisterType<IDepartment, DepartmentImp>();
            container.RegisterType<IHelper, HelperIml>();

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}