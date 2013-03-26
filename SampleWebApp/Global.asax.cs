using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Toolbelt.Web;

namespace SampleWebApp
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // Example 1) AppOfflineModule.Filter.IsEnable = () => DateTime.UtcNow <= DateTime.Parse("Feb 18, 2013");
            // Example 2) Return 503 instead of 404.
            //AppOfflineModule.Filter.Action = (app) =>
            //{
            //    app.Response.StatusCode = 503;
            //    app.Response.End();
            //};
        }
    }
}