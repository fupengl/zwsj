using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using Newtonsoft.Json;
using Service;
using ServiceStack;
using ZhiDiExt.Caching;
using ZhiDiExt.Mobile;
using ZhiDiExt.WebReq;
using ZhiDiShuJuWepAPI;


namespace ZhiDiExt
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new MvcExceptionAttribute());
        }

        //public static void RegisterRoutes(RouteCollection routes)
        //{
        //    routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
        //    routes.IgnoreRoute("{exclude}/{extnet}/ext.axd");

        //    routes.MapRoute(
        //        "Default", // Route name
        //        "{controller}/{action}/{id}", // URL with parameters
        //        new { controller = "Main", action = "Index", id = UrlParameter.Optional } // Parameter defaults
        //    );
        //}

        //public static void Register(HttpConfiguration config)
        //{
        //    config.Routes.MapHttpRoute(
        //        name: "DefaultApi",
        //        routeTemplate: "api/{controller}/{id}",
        //        defaults: new { id = RouteParameter.Optional }
        //    );
        //}

        protected void Application_Start()
        {
            log4net.Config.XmlConfigurator.Configure();

            AreaRegistration.RegisterAllAreas();

            //WebApiConfig.Register(GlobalConfiguration.Configuration);

            RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            GlobalConfiguration.Configuration.Filters.Add(new HttpExceptionAttribute());
            //FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            //RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //AuthConfig.RegisterAuth();
            BundleMobileConfig.RegisterBundles(BundleTable.Bundles);

            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            GlobalConfiguration.Configuration.Formatters
                .JsonFormatter.SerializerSettings.ContractResolver = new ExcludeEntityKeyContractResolver();

            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerSelector), new AreaHttpControllerSelector(GlobalConfiguration.Configuration));

        }

        private int GetCacheTime()
        {
            return Convert.ToInt32(ConfigurationManager.AppSettings["CacheTime"]);
        }

        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            if (HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated)
            {
                HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                if (authCookie != null)
                {
                    FormsAuthenticationTicket authTicket =
                        FormsAuthentication.Decrypt(authCookie.Value);
                    var userData = JsonConvert.DeserializeObject<string>(authTicket.UserData);
                    if (userData.Split('|').LastOrDefault() == "false")
                        return;
                    try
                    {
                        string cacheKey = string.Format("{0}_{1}", CacheKeyList.USER_PRINCIPAL, authTicket.Name);

                        if (CacheHelper.GetCacheManager().IsSet(cacheKey))
                        {
                            //HttpContext.Current.User = CacheHelper.GetCacheManager().Get<MyPrincipal>(cacheKey);
                        }
                        else
                        {
                            userData = JsonConvert.DeserializeObject<string>(authTicket.UserData);
                            var orgId = userData.Split('|').FirstOrDefault();//JsonConvert.DeserializeObject<string>(authTicket.UserData);
                            var securityService = new SecurityService();
                            var myPrincipal = securityService.GetPrincipal(authTicket.Name, orgId);
                            CacheHelper.GetCacheManager().Set(cacheKey, myPrincipal, GetCacheTime());


                            //userData = JsonConvert.DeserializeObject<string>(authTicket.UserData);
                            //if (userData.Split('|').LastOrDefault() == "true")
                            //{
                            //    var securityService = new SecurityService();
                            //    var myPrincipal = securityService.GetPrincipal(authTicket.Name, userData.Split('|').FirstOrDefault());
                            //    CacheHelper.GetCacheManager().Set(cacheKey, myPrincipal, GetCacheTime());
                            //    HttpContext.Current.User = CacheHelper.GetCacheManager().Get<MyPrincipal>(cacheKey);
                            //}
                            //else
                            //{
                            //    CacheHelper.GetCacheManager().Set(cacheKey, userData.Split('|').FirstOrDefault(), GetCacheTime());
                            //}

                        }
                        HttpContext.Current.User = CacheHelper.GetCacheManager().Get<MyPrincipal>(cacheKey);
                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }
                }
            }
        }

        protected void Application_BeginRequest()
        {
            if (HttpContext.Current.Request.HttpMethod.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase))
            {
                HttpContext.Current.Response.End();
            }
        }

        #region Overrides of HttpApplication

        /// <summary>
        /// 在添加所有事件处理程序模块之后执行自定义初始化代码。
        /// </summary>
        public override void Init()
        {
            this.PostAuthenticateRequest += (sender, e) => HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
            base.Init();
        }

        #endregion
    }
}