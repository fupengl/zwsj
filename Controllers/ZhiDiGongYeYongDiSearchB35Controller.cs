using System.Globalization;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;

namespace ZhiDiExt.Controllers
{
    public class ZhiDiGongYeYongDiSearchB35Controller : Controller
    {
        //
        // GET: /ZhiDiGongYeYongDiSearch/
        //private readonly WuYeService _wuYeService = new WuYeService();

        public ActionResult Index()
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (System.Web.HttpContext.Current.User.Identity.Name == "游客")
                {
                    return RedirectToAction("Index", "ZhiDiZhuZhaiSearch");
                }
                return View();
            }
            return RedirectToAction("LoginWeb", "Account");
        }

        
        public string Autocomplete(string query, string diqu, string wuyeleixin, string jiagefanwei)
        {
            var suggestions = new JArray();
            for (int i = 0; i < 100; i++)
            {
                var suggestion = new JObject {{"data", i}, {"value", "A_" + i.ToString(CultureInfo.InvariantCulture)}};
                suggestions.Add(suggestion);
            }

            var result = new JObject {{"query", "Unit"}, {"suggestions", suggestions}};

            return result.ToString();
        }
    }
}
