using System;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using Service;


namespace ZhiDiExt.Controllers
{
    public class ZhiDiYanXiB35Controller : Controller
    {
        private readonly DocumentService _documentService = new DocumentService();

        public ActionResult Index()
        {
            if (System.Web.HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return View();
            }
            return RedirectToAction("LoginWeb", "Account");
        }

        public JArray GetDocumetByType(string city, string type)
        {
            var query = _documentService.GetDocumentInType(city, type);
            var jArray = new JArray();
            foreach (var q in query)
            {
                jArray.Add(new JObject{
                {"TiTle",q.Title},
                {"ReleaseDate",q.ReleaseDate.HasValue ? q.ReleaseDate.Value.Date : DateTime.Now.Date},
                {"Url",q.Url.Remove(0,2)}
                });
            }
            return jArray;
        }

    }
}
