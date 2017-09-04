using System.Web.Mvc;
using DataAccess;
using Ext.Net.MVC;
using Service;

namespace ZhiDiExt.Controllers
{
    public class MarkerController : Controller
    {
        
        //
        // GET: /Marker/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexLouDong()
        {
            return View();
        }

        public JsonResult GetLouDong(string wuYeBianHao)
        {
            var uow = RepositoryHelper.GetUnitOfWork();
            var service = new ZhuZhaiService(uow);
            var data = service.GetAllZhuZhaiLouDongXiuZhengByWuYeBianHao(wuYeBianHao);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        

        public ActionResult SaveMarkerLouDong(int id, double ptX, double ptY)
        {
            if (ptX == 0 || ptY == 0) return this.Direct();
            var uow = RepositoryHelper.GetUnitOfWork();
            var service = new ZhuZhaiService(uow);
            var entity = service.GetZhuZhaiLouDongXiuZhengById(id);
            if (entity != null)
            {
                entity.JingWeiDu = string.Format("{0},{1}", ptX, ptY);
                service.UpdateZhuZhaiLouDongXiuZheng(entity);
                service.Save();
                return this.Direct();
            }
            return this.Direct();
        }

        public ActionResult SaveMarker(int id, string wuYeYongTu, double ptX, double ptY)
        {
            if (ptX == 0 || ptY == 0) return this.Direct();
 
            IUnitOfWork uow = RepositoryHelper.GetUnitOfWork();
            switch (wuYeYongTu)
            {
                case "ZhuZhai":
                {
                    var zhuZhaiService = new ZhuZhaiService(uow);
                    var jichuXinXi = zhuZhaiService.GetZhuZhaiJiChuXinXiById(id);
                    jichuXinXi.Px = ptX;
                    jichuXinXi.Py = ptY;
                    break;
                }
                case "ShangYe":
                {
                    var shangYeService = new ShangYeService(uow);
                    var jichuXinXi = shangYeService.GetShangYeJiChuXinXiById(id);
                    jichuXinXi.Px = ptX;
                    jichuXinXi.Py = ptY;
                    break;
                }
                case "BanGong":
                {
                    var banGongService = new BanGongService(uow);
                    var jichuXinXi = banGongService.GetBanGongJiChuXinXiById(id);
                    jichuXinXi.Px = ptX;
                    jichuXinXi.Py = ptY;
                    break;
                }
                case "JingYingXingYongDi":
                {
                    var jingYingXingYongDiService = new JingYingXingYongDiService(uow);
                    var jichuXinXi = jingYingXingYongDiService.GetJingYingXingYongDiById(id);
                    jichuXinXi.Px = ptX;
                    jichuXinXi.Py = ptY;
                    break;
                }
                default:
                    break;
            }
            uow.Commit();
            return this.Direct();
        }
    }
}
