using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HumanResourceManagement
{
    // ...existing code...
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Enable attribute routing
            routes.MapMvcAttributeRoutes();

            // Ensure the default route does not conflict with Areas
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "DangNhap", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
