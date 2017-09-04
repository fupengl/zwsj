using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using Service;

namespace ZhiDiExt.Controllers
{
    public class ZhiDiShangYeSearchB35Controller : Controller
    {
        private readonly WuYeService _wuYeService = new WuYeService();

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

        public string Autocomplete(string query, string city, string diqu)
            //string query, 
        {
            var query1 = _wuYeService.SearchWuYe(query, WuYeYongTus.ShangYe.ToString(), city, diqu, "");
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
