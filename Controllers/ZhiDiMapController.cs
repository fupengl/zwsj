using System.Globalization;
using System.Web.Mvc;
using PagedList;
using Service;
using Service.DTOs;

namespace ZhiDiExt.Controllers
{
    public class ZhiDiMapController : Controller
    {
        readonly XinZhengQuYuService _xinZhengQuYuService = new XinZhengQuYuService();
        //
        // GET: /BaiduMap/

        public ActionResult Index()
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return View();
            }
            return RedirectToAction("LoginWeb", "Account");
        }

        //public JsonResult QueryData(string wuYeYongTu, float minLat, float minLng, float maxLat, float maxLng, string zoom)
        //{
        //    WuYeService weYeService=new WuYeService();
        //    var data = weYeService.GetWuYeByArea(wuYeYongTu,"13");

        //    return Json(data,JsonRequestBehavior.AllowGet);
        //}
        
        //增加行政区域和物业类型的查询要素
        public JsonResult QueryData(string city, string wuYeYongTu, string keyword, string diqu, string wuyeleixing, int? page, int pageSize = 10, string zuoLuo="")
        {
            //string areaId = _xinZhengQuYuService.GetAreaIdByCity(city).ToString(CultureInfo.InvariantCulture);
            var pageNumber = (page ?? 1) + 1;
            var wuYeService = new WuYeService();
            var data = wuYeService.WebSearchWuYe(keyword, wuYeYongTu, city, diqu, wuyeleixing,zuoLuo);
            //var pagedData = data.ToPagedList(pageNumber, pageSize);

            //var jsonResponsePageList = new JsonResponsePageList(data.Count, pagedData);

            //return Json(jsonResponsePageList, JsonRequestBehavior.AllowGet);
            var pagedData = data.ToPagedList(pageNumber, pageSize);
            var jsonResponsePageList = new JsonResponsePageList(data.Count, pagedData,data);
            return Json(jsonResponsePageList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult QueryTuDiData(string city, string nianFen, string diKuaiBianHao, string xiangMuZuoLuo,
            string jinDeDanWei, string diKuaiYongTu, int? page, int pageSize = 10)
        {
            var pageNumber = (page ?? 1) + 1;
            var wuYeService = new WuYeService();
            var data = wuYeService.SearchJingYingXingYongDi(city, diKuaiBianHao, xiangMuZuoLuo, jinDeDanWei,
                diKuaiYongTu, nianFen);
            var pagedData = data.ToPagedList(pageNumber, pageSize);
            
            var jsonResponsePageList = new JsonResponsePageListTu(data.Count, pagedData, data);

            return Json(jsonResponsePageList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult QueryDataInSight(string city, string wuYeYongTu, string keyword, float minLat, float minLng, float maxLat, float maxLng, int zoom)
        {
            var areaId = _xinZhengQuYuService.GetAreaIdByCity(city).ToString(CultureInfo.InvariantCulture);
            var wuYeService = new WuYeService();
            var data = wuYeService.SearchWuYeByArea(wuYeYongTu, areaId, minLng, minLat, maxLng, maxLat, keyword);
            //var data = wuYeService.GetWuYeByArea(wuYeYongTu, "13").Take(100).ToList();

            var jsonResponse = new JsonResponse<WuYeDto>(data.Count, data);

            return Json(jsonResponse, JsonRequestBehavior.AllowGet);
        }
    }
}
