using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business;
using DataAccess;
using Ext.Net.MVC;
using Service;
using ServiceStack.Common.Extensions;

namespace ZhiDiExt.Controllers
{
    [Authorize]
    public class SAManagementManagementController :  BaseController.BaseController
    {
        private RbacService _service=new RbacService();
        //private string _assignedOrgId = "45980ad8-ebfe-447b-a6de-b060314ec847";

        //
        // GET: /SAManagementManagement/

        public ActionResult Index()
        {
            return View(GetToolarPermission());
        }

        public ActionResult GetChildren(string node)
        {
            return Json(_service.GetManagementAuthorize(node, GetOrganizationId()), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBusinessManagement(string node, string orgId, string managementType)
        {
            //var data = _service.GetAdminManagementTree(node, orgId, managementType);
            var data = _service.GetAdminManagementTree(node,orgId, GetOrganizationId(), managementType);
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
