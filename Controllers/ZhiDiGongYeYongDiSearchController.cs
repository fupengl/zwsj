using System.Web.Mvc;

namespace ZhiDiExt.Controllers
{
    public class ZhiDiGongYeYongDiSearchController : Controller
    {
        public ActionResult Index()
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return View();
            }
            return RedirectToAction("LoginWeb", "Account");
        }
    }
}