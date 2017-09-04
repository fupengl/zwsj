using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DataAccess;
using Newtonsoft.Json.Linq;
using Service;
using ZhiDiExt.Models;

namespace ZhiDiExt.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/
        private readonly XinZhengQuYuService _xinZhengQuYuService = new XinZhengQuYuService();

        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        public ActionResult LoginWeb()
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "ZhiDiZhuZhaiSearch");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(UserModel userModel)
        {
            var securityService = new SecurityService();
            var user = new SA_User
            {
                Name = userModel.Name,
                Password = userModel.Password
            };

            var authenticatedUser = securityService.GetUser(user);
            if (authenticatedUser != null)
            {
                Response.SetAuthCookie(authenticatedUser.Name, false, //authenticatedUser.Id);
                    string.Format("{0}{1}", authenticatedUser.Id, "|true"));
                return RedirectToAction("Index", "TianYuanDataCenter");//"ZhiDiZhuZhaiSearch");//TianYuanDataCenter
            }
            return View();
        }

        [HttpPost]
        public ActionResult LoginWeb(UserModel userModel)
        {
            if (ModelState.IsValid)
            {
                var securityService = new SecurityService();
                var user = new ExtrenalUser
                {
                    Name = userModel.Name,
                    Password = userModel.Password
                };
                var authenticatedUser = securityService.GetExtrenalUser(user.Name, user.Password);
                if (authenticatedUser != null)
                {
                    Response.SetAuthCookie(authenticatedUser.Name, false, //authenticatedUser.City);
                        string.Format("{0}{1}", authenticatedUser.City, "|false"));
                    SetCookie("CurrentCity", HttpUtility.UrlEncode(authenticatedUser.City.Split('/').LastOrDefault(), Encoding.GetEncoding("UTF-8")));
                    SetCookie("UserType", authenticatedUser.UserType);
                    return RedirectToAction("Index", "ZhiDiZhuZhaiSearch");
                    //return RedirectToAction("Index", "TianYuanDataCenter");
                }
                ViewBag.UserName = "用户名或密码错误！";
                ModelState.AddModelError("", "The user name or password provided is incorrect.");
                return View(userModel);
            }

            return View(userModel);
        }

        [HttpPost]
        public ActionResult LoginUser(string city)
        {
            Response.SetAuthCookie("游客", false, //authenticatedUser.City);
                        string.Format("{0}{1}", city, "|false"));
            //Session["City"] = authenticatedUser.City.Split('/').LastOrDefault();
            //HttpContext.Response.Cookies.Add(new HttpCookie("CurrentCity",
            //    authenticatedUser.City.Split('/').LastOrDefault()));
            SetCookie("CurrentCity", HttpUtility.UrlEncode(city, Encoding.GetEncoding("UTF-8")));
            SetCookie("UserType", "Formal");
            return RedirectToAction("Index", "ZhiDiZhuZhaiSearch");

        }

        private void SetCookie(string type, string value)
        {
            HttpContext.Response.Cookies.Add(new HttpCookie(type,
                value));
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            //Tom changed
            //return RedirectToAction("Index", "TianYuanDataCenter");
            return RedirectToAction("LoginWeb", "Account");
        }

        public ActionResult WebLogout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "ZhiDiZhuZhaiSearch");
        }

        public JArray GetUserCurrentCityByUserName(string userName)
        {
            var securityService = new SecurityService();
            var entity = securityService.GetExtrenalUser(userName);
            return new JArray(entity.City.Split('/').LastOrDefault());

            //@User.Identity.Name
        }

        public JArray GetCityList(string userType)
        {
            var query = _xinZhengQuYuService.GetCityList(userType);
            var jArray = new JArray();
            foreach (var q in query)
            {
                var jObject = new JObject
                {
                    {"Id",q.Id},
                    {"Description",q.Description},
                    {"Level",q.Level},
                    {"Enable",q.Enable}
                };
                jArray.Add(jObject);
            }
            return jArray;
            //return JObject.Parse(JsonConvert.SerializeObject(query));

        }
        //public ActionResult Get(string city, string callback)
        //{
        //    //var context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context
        //    //HttpRequestBase request = context.Request;//定义传统request对象
        //    //string city = request.Form["city"];

        //    var queryLogService = new QueryLogService();

        //    //var jObject = new JObject();
        //    //try
        //    //{
        //    //    jObject.Add("result", "success");
        //    //    jObject.Add("xiaoQuList", queryLogService.GetAllQueryLog(city));
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    jObject.Add("result", "fail");
        //    //    jObject.Add("info", ex.ToString());
        //    //}
        //    //return new JsonpResult<object>(new { jObject }, callback);
        //    //return new JsonpResult<object>(new { result = "success", xiaoQuList = queryLogService.GetAllQueryLog1(city) }, callback);


        //    //return JsonpResult<JObject>(jObject); 
        //    //return new JsonpResult(jObject);

        //}

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <returns></returns>
        public ActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// 忘记密码
        /// </summary>
        /// <returns></returns>
        public ActionResult ForgetPwd()
        {
            return View();
        }

        /// <summary>
        /// 个人中心
        /// </summary>
        /// <returns></returns>
        public ActionResult PersonalCenter()
        {
            return View();
        }

        /// <summary>
        /// 机构介绍
        /// </summary>
        /// <returns></returns>
        public ActionResult CompanyIntroduce()
        {
            return View();
        }
    }
}
