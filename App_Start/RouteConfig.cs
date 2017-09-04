using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace ZhiDiExt
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            //routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "Api/Mobile/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            //var config = GlobalConfiguration.Configuration;

            //var xmlFormateer = config.Formatters
            //    .Where(f =>
            //    {
            //        return f.SupportedMediaTypes.Any(v => v.MediaType == "text/xml");
            //    })
            //.FirstOrDefault();

            //if (xmlFormateer != null)
            //{
            //    config.Formatters.Remove(xmlFormateer);
            //}
        }
    }
}