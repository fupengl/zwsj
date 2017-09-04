using System;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using Service;
using ZhiDiExt.BaseController;

namespace ZhiDiExt.Controllers
{
    public class ZhiDiZhuZhaiDetailController : WebBaseController
    {
        //
        // GET: /ZhiDiZhuZhaiDetail/
        private readonly WuYeService _wuYeService = new WuYeService();
        private readonly XiuZhengTiXiService _xiuZhengTiXiService = new XiuZhengTiXiService();

        public ActionResult Index(string wuYeBianHao)
        {
            
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return View();
            }
            return RedirectToAction("LoginWeb", "Account");
        }

        public JObject GetZhuZhaiJiChuXinXi(string wuYeBianHao)
        {
            return _wuYeService.GetJiChuXinXi(WuYeYongTus.ZhuZhai.ToString(), wuYeBianHao);
        }

        public JArray GetImage(string wuYeBianHao)
        {
            var jArray = new JArray();
            var query = _wuYeService.WebGetImageFileName(WuYeYongTus.ZhuZhai.ToString(), wuYeBianHao);
            foreach (var q in query)
            {
                jArray.Add(q.Remove(0,2));
            }
            return jArray;
        }

        public JArray GetPingGuJieGuo(string bianHao, string cityId, string wuYeLeiXingId, string daiMa, string jianZhuMianJi, string geLouMianJi, string suoZaiCeng, string zuiGaoCeng)
        {
            return _xiuZhengTiXiService.GetZongXiuZhengYinSu(bianHao, "", cityId, "ZhuZhai", wuYeLeiXingId, daiMa, jianZhuMianJi, geLouMianJi,
                suoZaiCeng, zuiGaoCeng,"Test");
        }

        public JArray GetWuYeLeiXing(string wuYeBianHao)
        {
            var jArray = new JArray();
            var query = _wuYeService.GetWuYeLeiXingByBianHao(wuYeBianHao);
            foreach (var q in query)
            {
                jArray.Add(
                    new JObject
                    {
                        {"id",q.WuYeLeiXingId},
                        {"type",q.Type}
                    });
            }
            return jArray;
        }

        public JObject GetZuiGaoCeng()
        {
            var query = _xiuZhengTiXiService.GetZuiGaoCeng();
            return new JObject {{"louCeng", Convert.ToInt32(query)}};
        }


        public JObject GetJiaGeZouShi(string wuYeBianHao)
        {

            var jObject = new JObject();
            var query = _wuYeService.GetJiZhunJiaMonthWeb(WuYeYongTus.ZhuZhai.ToString(), wuYeBianHao);
            var dictGengXinJiDu = query.GroupBy(x => x.GenXinMonth).ToDictionary(x => x.Key, y => y.ToList());
            var jArrayGengXinJiDu = new JArray();
            foreach (var gengXinJiDu in dictGengXinJiDu)
            {
                jArrayGengXinJiDu.Add(gengXinJiDu.Key);
            }
            jObject.Add("time", jArrayGengXinJiDu);

            var jArrayJiZhunJia = new JArray();
            var dictJiZhunJia = query.OrderBy(y=>y.GenXinJiDu).GroupBy(x => x.WuYeLeiXin).ToDictionary(x => x.Key, y => y.ToList()); //_wuYeService.GetJiZhunJia("ZhuZhai", wuYeBianHao).GroupBy(x => x.WuYeLeiXin).ToDictionary(x => x.Key, y => y.ToList());
            foreach (var jiZhunJia in dictJiZhunJia)
            {
                //jArrayJiZhunJia.Add(new JObject { { "name", jiZhunJia.Key } });
                var jObjectJiZhunJia = new JObject
                {
                    {"name", jiZhunJia.Key}
                };
                var ja = new JArray();
                foreach (var j in jiZhunJia.Value)
                {
                    if (j.JiZhunJia == 0)
                    {
                        ja.Add(null);
                    }
                    else
                    {
                        ja.Add(j.JiZhunJia);
                    }
                       
                }
                jObjectJiZhunJia.Add("data", ja);
                jArrayJiZhunJia.Add(jObjectJiZhunJia);
            }
            jObject.Add("jiaGe", jArrayJiZhunJia);
            //return jArrayJiZhunJia;
            return jObject;
        }

        //public JObject GetJiaGeZouShi(string wuYeBianHao)
        //{

        //    var jObject = new JObject();
        //    var query = _wuYeService.GetJiZhunJia(WuYeYongTus.ZhuZhai.ToString(), wuYeBianHao);
        //    var dictGengXinJiDu = query.GroupBy(x => x.GenXinJiDuDesc).ToDictionary(x => x.Key, y => y.ToList());
        //    var jArrayGengXinJiDu = new JArray();
        //    foreach (var gengXinJiDu in dictGengXinJiDu)
        //    {
        //        jArrayGengXinJiDu.Add(gengXinJiDu.Key);
        //    }
        //    jObject.Add("time", jArrayGengXinJiDu);

        //    var jArrayJiZhunJia = new JArray();
        //    var dictJiZhunJia = query.GroupBy(x => x.WuYeLeiXin).ToDictionary(x => x.Key, y => y.ToList()); //_wuYeService.GetJiZhunJia("ZhuZhai", wuYeBianHao).GroupBy(x => x.WuYeLeiXin).ToDictionary(x => x.Key, y => y.ToList());
        //    foreach (var jiZhunJia in dictJiZhunJia)
        //    {
        //        //jArrayJiZhunJia.Add(new JObject { { "name", jiZhunJia.Key } });
        //        var jObjectJiZhunJia = new JObject
        //        {
        //            {"name", jiZhunJia.Key}
        //        };
        //        var ja = new JArray();
        //        foreach (var j in jiZhunJia.Value)
        //        {
        //            ja.Add(j.JiZhunJia);
        //        }
        //        jObjectJiZhunJia.Add("data", ja);
        //        jArrayJiZhunJia.Add(jObjectJiZhunJia);
        //    }
        //    jObject.Add("jiaGe", jArrayJiZhunJia);
        //    //return jArrayJiZhunJia;
        //    return jObject;
        //}



        


    }
}
