using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Dispatcher;

namespace ZhiDiExt
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //var suffix = typeof (DefaultHttpControllerSelector).GetField("ControllerSuffix",
            //    System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            //if (suffix != null)
            //    suffix.SetValue(null, "ApiController");

            /*config.Routes.MapHttpRoute(
                name: "WebApi",
                routeTemplate: "api/web/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );*/

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{type}/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
