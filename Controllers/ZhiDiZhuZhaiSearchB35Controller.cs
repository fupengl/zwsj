using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using Service;
using System.Text;
using System.Web;
using System;

namespace ZhiDiExt.Controllers
{
    public class ZhiDiZhuZhaiSearchB35Controller : Controller
    {
        //
        // GET: /ZhiDiSearch/
        private readonly WuYeService _wuYeService = new WuYeService();
        public ActionResult Index(string requestFrom = "")
        {
            if (SetCookies(requestFrom.ToLower()))
            {
                return View();
            }
            else
            {
                return RedirectToAction("LoginWeb", "Account");
            }
 
        }

        private bool SetCookies(string requestFrom)
        {

            bool bSetSucessfull = false;

            if (string.IsNullOrEmpty(requestFrom))
            {
                if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    Session["CurrentCity"] = HttpUtility.UrlDecode(Request.Cookies["CurrentCity"].Value, Encoding.GetEncoding("UTF-8"));
                    Session["QuYu"] = "全部";
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                //智屋请求
                string strCity = "苏州市";

                //如果已经登录，则获取登录城市
                if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    strCity = Request.Cookies["CurrentCity"].Value.ToString();
                }

                //如果请求有“城市”这个参数，则覆盖原有城市
                if (System.Web.HttpContext.Current.Request["city"] != null) strCity = System.Web.HttpContext.Current.Request["city"].ToString();
                string strQuYu = System.Web.HttpContext.Current.Request["quyu"] != null ? System.Web.HttpContext.Current.Request["quyu"].ToString() : "全部";
                string strDiZhi = System.Web.HttpContext.Current.Request["dizhi"] != null ? System.Web.HttpContext.Current.Request["dizhi"].ToString() : "";
                string strKeyword = System.Web.HttpContext.Current.Request["keyword"] != null ? System.Web.HttpContext.Current.Request["keyword"].ToString() : "";
                string strDoSearch = System.Web.HttpContext.Current.Request["dosearch"] != null ? System.Web.HttpContext.Current.Request["dosearch"].ToString() : "";

                //先要进行一次解码
                while (strQuYu.Contains("%"))
                {
                    strQuYu = HttpUtility.UrlDecode(strQuYu);
                }
                //区域如果是空的话，则改成“全部”
                if (strQuYu.Trim().Length == 0)
                {
                    strQuYu = "全部";
                }

                //hack for request from zwjy
                if (requestFrom.ToLower() == "zwjy")
                {
                    //对区域进行判断，例如，如果是 "吴江市_Shi", 则，把 strCity 改成 "吴江", 然后把 strQuYu = "全部"
                    string[] arrQuYu = strQuYu.Split('_');
                    if (arrQuYu.Length > 1 && arrQuYu[1] == "Shi")
                    {
                        strCity = arrQuYu[0];
                        strQuYu = "全部";
                    }
                    else
                    {
                        strQuYu = arrQuYu[0];
                    }
                }


                Session["CurrentCity"] = strCity;
                Session["QuYu"] = strQuYu;

               
                SetCookie("CurrentCity", HttpUtility.UrlEncode(strCity, Encoding.GetEncoding("UTF-8")));
                SetCookie("RequestFrom", requestFrom);
                SetCookie("QuYu", HttpUtility.UrlEncode(strQuYu, Encoding.GetEncoding("UTF-8")));
                SetCookie("DiZhi", HttpUtility.UrlEncode(strDiZhi, Encoding.GetEncoding("UTF-8")));
                SetCookie("Keyword", HttpUtility.UrlEncode(strKeyword, Encoding.GetEncoding("UTF-8")));
                SetCookie("DoSearch", HttpUtility.UrlEncode(strDoSearch, Encoding.GetEncoding("UTF-8")));

                //来自ZWJY的请求，并且还没有认证过的用户，自动注册成游客
                if (requestFrom.ToLower() == "zwjy" && !System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    Response.SetAuthCookie("游客", false, string.Format("{0}{1}", strCity, "|false"));
                    SetCookie("UserType", "Formal");
                    bSetSucessfull = true;
                }
                if (requestFrom.ToLower() == "zwjy" || System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    bSetSucessfull = true;
                }

               
            
            }

            return bSetSucessfull;
        }

        private void SetCookie(string type, string value)
        {
            HttpContext.Response.Cookies.Add(new HttpCookie(type,
                value));
        }


        private bool DeleteCookie(string strName)
        {
            try
            {
                HttpCookie Cookie = new HttpCookie(strName);
                //Cookie.Domain = ".xxx.com";//当要跨域名访问的时候,给cookie指定域名即可,格式为.xxx.com
                Cookie.Expires = DateTime.Now.AddDays(-1);
                System.Web.HttpContext.Current.Response.Cookies.Add(Cookie);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string Autocomplete(string query, string city, string diqu, string wuyeleixin, string jiagefanwei)//string query, 
        {
            var query1 = _wuYeService.SearchWuYe(query, WuYeYongTus.ZhuZhai.ToString(), city, diqu, wuyeleixin);
            var suggestions = new JArray();
            foreach (var q in query1)
            {
                var suggestion = new JObject
                {
                    {"data", q.WuYeBianHao}, 
                    {"value", q.WuYeMingCheng}
                
                };
                suggestions.Add(suggestion);
            }
            
            
            var result = new JObject 
            {
                {"query", "Unit"}, 
                {"suggestions", suggestions}
            };

            return result.ToString();
        }

    }
}
