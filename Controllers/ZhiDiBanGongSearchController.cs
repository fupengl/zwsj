using System.Web.Mvc;

namespace ZhiDiExt.Controllers
{
    public class ZhiDiBanGongSearchController : Controller
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