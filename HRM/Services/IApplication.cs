using HRM.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace HRM.Services
{
    public interface IApplication
    {
        ApplicationContext GetContext();

    }
}