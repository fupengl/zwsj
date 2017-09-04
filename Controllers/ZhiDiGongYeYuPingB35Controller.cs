using System;
using System.Linq;
using System.Web.Mvc;
using Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Service;
using Service.DTOs;

namespace ZhiDiExt.Controllers
{
    public class ZhiDiGongYeYuPingB35Controller : Controller
    {
        //
        // GET: /GongYeYuPing/
        private readonly XiuZhengTiXiService _xiuZhengTiXiService = new XiuZhengTiXiService();
        private readonly GongYePingGuService _gongYePingGuService = new GongYePingGuService();
        private readonly GongYePianQuService _gongYeZhaoPianService = new GongYePianQuService();
        private readonly XinZhengQuYuService _xinZhengQuYuService = new XinZhengQuYuService();

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

        public JObject GetGongYePingGuCanShu(string city)
        {
            int cityId = _xinZhengQuYuService.GetAreaIdByCity(city);
            var gongYePingGuCanShuDto = new GongYePingGuCanShuDto
            {
                XiuZhengXiShu = _xiuZhengTiXiService.GetXiuZhengXiShuByCity(cityId),
                GongYePianQu = _gongYePingGuService.GetGongYePianQuByCity(cityId)
            };
            if (gongYePingGuCanShuDto.XiuZhengXiShu.Any() && gongYePingGuCanShuDto.GongYePianQu.Any())
            {
                gongYePingGuCanShuDto.Result = "true";
            }
            else
            {
                gongYePingGuCanShuDto.Result = "false";
            }
            return JObject.Parse(JsonConvert.SerializeObject(gongYePingGuCanShuDto));
        }

        public JArray GetImageById(int id)
        {
            var jAarry = new JArray();
            var query = _gongYeZhaoPianService.GetGongYePianQuZhaoPianByGongYePianQuId(id);
            if (query.Any())
            {
                foreach (var q in query)
                {
                    //Tom changed
                    //jAarry.Add(q.Url); //.Remove(0, 2));
                    jAarry.Add(Url.Content(q.Url));
                }
                //urls.AddRange(query.Select(x => x.Url.Remove(0, 2)));
            }
            else
            {
                ////Tom changed
                //jAarry.Add(MsgInfo.ZanWuTuPianUrl);//"Photo/0.png");
                jAarry.Add(Url.Content(MsgInfo.ZanWuTuPianUrl));//"Photo/0.png");

            }
            return jAarry;
        }

        public JArray GetPingGuJieGuo(int gongYePianQuId, decimal? tuDiMianJi, DateTime? zhongZhiRiQi, DateTime? guJiaShiDian,
            decimal? jianZhuMianJi, string jianZhuJieGouDaiMa, decimal? yiShiYongNianXian, decimal? pinJunCengGao,
            decimal? diXiaMianJi, string city)
        {
            int cityId = _xinZhengQuYuService.GetAreaIdByCity(city);
            return _gongYePingGuService.GetPingGuJiaGe(gongYePianQuId, tuDiMianJi, zhongZhiRiQi.HasValue ? zhongZhiRiQi.Value : DateTime.Now,
                guJiaShiDian.HasValue ? guJiaShiDian.Value : DateTime.Now, jianZhuMianJi, jianZhuJieGouDaiMa, yiShiYongNianXian, pinJunCengGao, diXiaMianJi, cityId);
            //return JObject.Parse(JsonConvert.SerializeObject(query));  
        }

    }
}
