using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using Service;

namespace ZhiDiExt.Controllers
{
    public class ZhiDiGongYeYongDiDetailController : Controller
    {
        //
        // GET: /ZhiDiGongYeYongDiDetail/
        private readonly WuYeService _wuYeService = new WuYeService();

        public ActionResult Index()
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return View();
            }
            return RedirectToAction("LoginWeb", "Account");
        }

        public JObject GetTuDiJiChuXinXi(string wuYeBianHao)
        {
            return _wuYeService.GetJiChuXinXi(WuYeYongTus.JingYingXinYongDi.ToString(), Utf8Decode(wuYeBianHao));
        }

        private string Utf8Decode(string value)
        {
            return HttpUtility.UrlDecode(value, Encoding.GetEncoding("UTF-8"));
        }

        public JArray GetImage(string wuYeBianHao)
        {
            var jArray = new JArray();
            var query = _wuYeService.WebGetImageFileName(WuYeYongTus.JingYingXinYongDi.ToString(), wuYeBianHao);
            foreach (var q in query)
            {
                jArray.Add(q.Remove(0, 2));
            }
            return jArray;
        }

    }
}
