using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using Service;

namespace ZhiDiExt.Controllers
{
    public class ZhiDiBanGongDetailController : Controller
    {
        //
        // GET: /ZhiDiBanGongDetail/
        private readonly WuYeService _wuYeService = new WuYeService();

        public ActionResult Index()
        {
            //if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            //{
            //    return View();
            //}
            //return RedirectToAction("LoginWeb", "Account");
            return View();
        }

        public JObject GetBanGongJiChuXinXi(string wuYeBianHao)
        {
            return _wuYeService.GetJiChuXinXi(WuYeYongTus.BanGong.ToString(), wuYeBianHao);
        }

        public JArray GetImage(string wuYeBianHao)
        {
            var jArray = new JArray();
            var query = _wuYeService.WebGetImageFileName(WuYeYongTus.BanGong.ToString(), wuYeBianHao);
            foreach (var q in query)
            {
                jArray.Add(q.Remove(0, 2));
            }
            return jArray;
        }

        public JObject GetJiaGeZouShi(string wuYeBianHao)
        {
            var jObject = new JObject();
            var query = _wuYeService.GetJiZhunJia(WuYeYongTus.BanGong.ToString(), wuYeBianHao);
            var dictGengXinJiDu = query.GroupBy(x => x.GenXinJiDuDesc).ToDictionary(x => x.Key, y => y.ToList());
            var jArrayGengXinJiDu = new JArray();
            foreach (var gengXinJiDu in dictGengXinJiDu)
            {
                jArrayGengXinJiDu.Add(gengXinJiDu.Key);
            }
            jObject.Add("time", jArrayGengXinJiDu);


            //var dictJiZhunJia = query.GroupBy(x => x.WuYeLeiXin).ToDictionary(x => x.Key, y => y.ToList()); //_wuYeService.GetJiZhunJia("ZhuZhai", wuYeBianHao).GroupBy(x => x.WuYeLeiXin).ToDictionary(x => x.Key, y => y.ToList());
            var jArrayJiZhunJia = new JArray();
            var jObjectJiZhunJia = new JObject { { "name", "售价" } };

            var jObjectZuJin = new JObject { { "name", "租金" } };
            var jaJiZhunJia = new JArray();
            var jaZuJin = new JArray();
            foreach (var q in query)
            {
                jaJiZhunJia.Add(q.JiZhunJia);
                jaZuJin.Add(q.ZuJin);
            }
            jObjectJiZhunJia.Add("data", jaJiZhunJia);
            jObjectZuJin.Add("data", jaZuJin);
            jArrayJiZhunJia.Add(jObjectJiZhunJia);
            jArrayJiZhunJia.Add(jObjectZuJin);
            jObject.Add("jiaGe", jArrayJiZhunJia);

            return jObject;
        }

    }
}
