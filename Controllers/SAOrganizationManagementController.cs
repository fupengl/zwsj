using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business;
using DataAccess;
using Ext.Net;
using Ext.Net.MVC;
using Infrastructure;
using Service;
using ServiceStack.Common.Extensions;
using PartialViewResult = Ext.Net.MVC.PartialViewResult;

namespace ZhiDiExt.Controllers
{
    [Authorize]
    public class SAOrganizationManagementController : BaseController.BaseController
    {
        RbacService _service=new RbacService();
        //private string _orgId = "45980ad8-ebfe-447b-a6de-b060314ec847";
        //
        // GET: /DataDictionary/

        public ActionResult Index()
        {
            return View(GetToolarPermission());
        }

        public ActionResult GetChildren(string node)
        {
            var data = _service.GetManagementAuthorize(node, GetOrganizationId());
            return Json(data, JsonRequestBehavior.AllowGet);
        }



        protected override List<object> GetData()
        {
            throw new NotImplementedException();
        }

        public override ActionResult Refresh()
        {
            throw new NotImplementedException();
        }
    }
}
