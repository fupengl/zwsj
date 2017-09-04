using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Business;
using Ext.Net;
using Ext.Net.MVC;
using Infrastructure;
using Newtonsoft.Json;
using Service;
using ZhiDiExt.Caching;

namespace ZhiDiExt.Controllers
{
    [Authorize]
    public class TianYuanDataCenterController : Controller
    {
        private readonly RbacService _rbacService=new RbacService();
        //
        // GET: /TianYuanDataCenter/

        public ActionResult Index()
        {
            //var url = Request.Url;
            //var controller = RouteData.Values["controller"];
            //var action = RouteData.Values["action"];
            //string loginIp = HttpContext.Request.UserHostAddress;
            //var user = System.Web.HttpContext.Current.User as MyPrincipal;
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            var userData = JsonConvert.DeserializeObject<string>(authTicket.UserData);
            if (userData.Split('|').LastOrDefault() == "false")
                return RedirectToAction("Index", "ZhiDiZhuZhaiSearch");
            this.GetCmp<Button>("userName").Text = System.Web.HttpContext.Current.User.Identity.Name;
            this.GetCmp<Window>("Window2").Hide();
            return View();
        }

        public JsonResult SysMenu(string nodeType, int? id)
        {
            if (nodeType == "list")
            {
                return Json(_rbacService.GetMenus(), JsonRequestBehavior.AllowGet);
            }
            return Json(_rbacService.GetMenuItems(id ?? 0), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Logout()
        {
            string cacheKey = string.Format("{0}_{1}", CacheKeyList.USER_PRINCIPAL,
                System.Web.HttpContext.Current.User.Identity.Name);
            CacheHelper.GetCacheManager().Remove(cacheKey);
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public ActionResult Modify(string oldPassword, string newPassword)
        {
            var securityService = new SecurityService();
            var success = securityService.ModifyPassword(System.Web.HttpContext.Current.User.Identity.Name, oldPassword,
                newPassword);
            if (success)
            {
                X.Msg.Alert(MsgInfo.TI_SHI, MsgInfo.PasswordModifySuccess).Show();
                this.GetCmp<TextField>("oldPassword").Clear();
                this.GetCmp<TextField>("newPassword").Clear();
                this.GetCmp<TextField>("comfirmPassword").Clear();
                this.GetCmp<Window>("Window2").Hide();
            }
            else
            {
                X.Msg.Alert(MsgInfo.TI_SHI, MsgInfo.PasswordModifyFail).Show();
            }

            return this.Direct();
        }
    }
}
