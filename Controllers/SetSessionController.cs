using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;
using Newtonsoft.Json.Linq;

namespace ZhiDiExt.Controllers
{
    public class SetSessionController : Controller
    {
        public ActionResult SetVariable(string key, string value)
        {
            Session[key] = value;
            return Json(new { success = value });
        }

        [WebMethod(EnableSession = true)]
        public JObject GetVariable(string key)
        {
            var s =  Session[key].ToString();
            var jb = new JObject();
            jb.Add("Value", s);
            return jb;
        }

        public JObject GetCookie(string key)
        {
            var currentCity = Utf8Decode(HttpContext.Request.Cookies[key].Value);
            return new JObject
            {
                { key, currentCity}
            };
        }

        [System.Web.Http.HttpPost]
        public JObject SetCookie(string key, string value)
        {
            if (key == "CurrentCity") { Session["CurrentCity"] = value; };
            var httpCookie = HttpContext.Request.Cookies[key];
            if (httpCookie != null)
            {
                httpCookie.Value = Utf8Encode(value);
                Response.SetCookie(httpCookie);
                //Response.Cookies["Info"]["user"] = "2"
            }
            else
            {
                //var cookie = new HttpCookie("userInfo");
                //var ts = new TimeSpan(1, 0, 0, 0);
                //var dt = DateTime.Now;
                //cookie.Expires = dt.Add(ts);
                //cookie.Values.Add(key, value);
                //Response.AppendCookie(cookie);
                HttpContext.Response.Cookies.Add(new HttpCookie(key, value));
            }
            return new JObject {{"Value", value}};
        }

        private string Utf8Encode(string value)
        {
            return HttpUtility.UrlEncode(value, Encoding.GetEncoding("UTF-8"));
        }

        private string Utf8Decode(string value)
        {
            return HttpUtility.UrlDecode(value, Encoding.GetEncoding("UTF-8"));
        }
    }
}
