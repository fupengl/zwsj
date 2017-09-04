using System.Web.Mvc;

namespace ZhiDiExt.Controllers
{
    /// <summary>
    /// 住宅查询控制器
    /// </summary>
    public class ZhiDiZhuZhaiSearchController : Controller
    {
        public ActionResult Index()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return View();
            }
            return RedirectToAction("LoginWeb", "Account");
        }

        /// <summary>
        /// 住宅预评
        /// </summary>
        /// <returns></returns>
        public ActionResult YuPing()
        {
            return View();
        }
    }
}
