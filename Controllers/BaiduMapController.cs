using System.Globalization;
using System.Web.Mvc;
using PagedList;
using Service;
using Service.DTOs;

namespace ZhiDiExt.Controllers
{
    public class BaiduMapController : Controller
    {
        readonly XinZhengQuYuService _xinZhengQuYuService = new XinZhengQuYuService();
        //
        // GET: /BaiduMap/

        public ActionResult Index()
        {
            return View();
        }

        //public JsonResult QueryData(string wuYeYongTu, float minLat, float minLng, float maxLat, float maxLng, string zoom)
        //{
        //    WuYeService weYeService=new WuYeService();
        //    var data = weYeService.GetWuYeByArea(wuYeYongTu,"13");
            
        //    return Json(data,JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult QueryData(string city,string wuYeYongTu, string keyword, int? page, int pageSize = 10)
        //{
        //    //string areaId = _xinZhengQuYuService.GetAreaIdByCity(city).ToString();
        //    var pageNumber = (page ?? 1) + 1;
        //    var wuYeService=new WuYeService();
        //    var data = wuYeService.Query(keyword,wuYeYongTu);
        //    var pagedData = data.ToPagedList(pageNumber, pageSize);

        //    var jsonResponsePageList = new JsonResponsePageList(data.Count, pagedData);

        //    return Json(jsonResponsePageList, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult QueryDataInSight(string city, string wuYeYongTu, string keyword, float minLat, float minLng, float maxLat, float maxLng, int zoom)
        //{
        //    string areaId = _xinZhengQuYuService.GetAreaIdByCity(city).ToString(CultureInfo.InvariantCulture);
        //    var wuYeService = new WuYeService();
        //    var data = wuYeService.SearchWuYeByArea(wuYeYongTu, areaId, minLng, minLat, maxLng, maxLat, keyword);
        //    //var data = wuYeService.GetWuYeByArea(wuYeYongTu, "13").Take(100).ToList();

        //    var jsonResponse = new JsonResponse<WuYeDto>(data.Count, data);

        //    return Json(jsonResponse, JsonRequestBehavior.AllowGet);
        //}
    }
}
