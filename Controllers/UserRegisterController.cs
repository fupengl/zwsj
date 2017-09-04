using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Newtonsoft.Json.Linq;
using Service;
using DataAccess;
using ServiceStack.Common.Extensions;

namespace ZhiDiExt.Controllers
{
    public class UserRegisterController : BaseController.BaseController
    {
        private readonly XinZhengQuYuService _xinZhengQuYuService = new XinZhengQuYuService();
        private readonly UserRegisterService _userRegisterService = new UserRegisterService(GetActiveOrgFilter());
        private readonly ExtrenalUserService _service = new ExtrenalUserService();
        //private readonly SecurityService _securityService = new SecurityService();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Phone()
        {
            return View();
        }

        public ActionResult UserInfo()
        {
            return View();
        }

        public ActionResult Unbound()
        {
            return View();
        }

        public ActionResult DownloadApp()
        {
            return View();
        }

        public ActionResult Test()
        {
            return View();
        }

        public ActionResult GetAll(StoreRequestParameters parameters = null)
        {
            var data = GetData();

            if (parameters == null)
            {
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return this.Store(FilterSortAndPaging(data, parameters), data.Count);
        }

        protected override List<object> GetData()
        {
            return _userRegisterService.GetAllUserRegister().ToObjects();
        }

        public override ActionResult Refresh()
        {
            this.GetCmp<Store>("storeUserRegister").Reload();
            return this.Direct();
        }


        public JArray GetCityList()
        {
            var jArray = new JArray();
            var query = _xinZhengQuYuService.GetAllCity();
            
            foreach (var q in query)
            {
                var jObject = new JObject
                {
                    {"AreaPath", q.TreePath},
                    {"CityName", q.Description}
                };
                jArray.Add(jObject);
            }
            return jArray;
        }

        /// <summary>
        /// 公众用户注册接口
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="phone"></param>
        /// <param name="email"></param>
        /// <param name="industry"></param>
        /// <param name="city"></param>
        /// <returns></returns>
        [HttpPost]
        public JObject PostRegistrationApplication(string userName, string phone, string email, string industry,
            string city)
        {
            //var jObject = new JObject();
            try
            {
                var entity = new UserRegister
                {
                    UserName = userName,
                    Phone = phone,
                    EMail = email,
                    Industry = industry,
                    City = city
                };
                _userRegisterService.AddUserRegister(entity);
                _userRegisterService.Save();
                return new JObject
                {
                    {"Result","success"},
                    {"Info","感谢您申请试用智地数据平台，我们的工作人员会尽快与你取得联系。"}
                };
            }
            catch (Exception ex)
            {
                return new JObject
                {
                    {"Result","fail"},
                    {"Info",ex.ToString()}
                };
            }
        }

        /// <summary>
        /// 用户更换手机解绑接口
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="unboundPassword"></param>
        /// <returns></returns>
        [HttpPost]
        public JObject PostUnbound(string userName, string unboundPassword)
        {
            try
            {
                return _service.UnboundPassword(userName, unboundPassword);

            }
            catch (Exception ex)
            {
                return new JObject
                {
                    {"Result","fail"},
                    {"Info",ex.ToString()}
                };
            }
        }


        /// <summary>
        /// 专业版用户完善个人信息
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="deviceId"></param>
        /// <param name="account"></param>
        /// <param name="industry"></param>
        /// <param name="unboundPasswordConfirm"></param>
        /// <returns></returns>
        [HttpPost]
        public JObject PostUserInfo(string userName, string deviceId, string account, string industry, string unboundPasswordConfirm)
        {
            try
            {
                return _service.EditUserInfo(userName, deviceId, account, industry, unboundPasswordConfirm);

            }
            catch (Exception ex)
            {
                return new JObject
                {
                    {"Result","fail"},
                    {"Info",ex.ToString()}
                };
            }

        }

    }
}
