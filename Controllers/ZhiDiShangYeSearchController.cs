using System.Web.Mvc;

namespace ZhiDiExt.Controllers
{
    /// <summary>
    /// 商业查询控制器
    /// </summary>
    public class ZhiDiShangYeSearchController : Controller
    {
        public ActionResult Index()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return View();
            }
            return RedirectToAction("LoginWeb", "Account");
        }
    }
}
