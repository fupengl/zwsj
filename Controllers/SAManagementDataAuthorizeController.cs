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
using Service.HierarchyDto;
using EnumerableExtensions = ServiceStack.Common.Extensions.EnumerableExtensions;
using PartialViewResult = Ext.Net.MVC.PartialViewResult;

namespace ZhiDiExt.Controllers
{
    [Authorize]
    public class SAManagementDataAuthorizeController : BaseController.BaseController
    {
        //private string _assignedOrgId = "45980ad8-ebfe-447b-a6de-b060314ec847";
        private RbacService _service=new RbacService();

        //
        // GET: /SADataAuthorize/

        public ActionResult Index()
        {
            return View(GetToolarPermission());
        }


        protected override List<object> GetData()
        {
            throw new NotImplementedException();
        }

        public override ActionResult Refresh()
        {
            throw new NotImplementedException();
        }

        public ActionResult GetChildren(string node)
        {
            return Json(_service.GetManagementAuthorize(node, GetOrganizationId()), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetManagementRoleAuthorize(string managementOrgId)
        {
            var data = _service.GetManagementDataRoleAuthorize(GetOrganizationId(), managementOrgId);

            return this.Store(data, data.Count());
        }


        public ActionResult SaveManagementDataAuthorize(string managementOrgId, string authorizes)
        {
            ChangeRecords<ManagementAuthorizeDTO> records =
                new StoreDataHandler(authorizes).BatchObjectData<ManagementAuthorizeDTO>();


            var curOrganization = _service.GetOrgById(managementOrgId);

            foreach (var updatedRec in records.Updated)
            {
                if (updatedRec.Granted.ToLower() == "true")
                {
                    SA_Authorize role = new SA_Authorize
                    {
                        OrgId = curOrganization.Id,
                        OrgName = curOrganization.Name,
                        OrgIdPath = curOrganization.IdPath,
                        OrgNamePath = curOrganization.NamePath,
                        AuthorizeRoleId = updatedRec.RoleId,
                        CreatedBy = GetCurrentUserName(),
                        CreatedDate = DateTime.Now,
                        CreatedByIdPath = GetOrganizationIdPath(),
                        CreatedByNamePath = GetOrganizationNamePath()
                    };
                    _service.AddAuthorize(role);
                }
                else
                {
                    _service.DeleteAuthorize(curOrganization.Id, updatedRec.RoleId);
                }
            }
            _service.Save();

            return this.Direct();
        }
    }
}
