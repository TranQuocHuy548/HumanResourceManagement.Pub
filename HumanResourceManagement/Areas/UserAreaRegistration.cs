using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HumanResourceManagement.Areas.User
{
    public class UserAreaRegistration : AreaRegistration
    {
        public override string AreaName => "User";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "User_area_route", //  đặt tên route duy nhất, tránh lỗi trùng
                url: "User/{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "HumanResourceManagement.Areas.User.Controllers" } // khai báo namespace rõ ràng
            );
        }
    }
}