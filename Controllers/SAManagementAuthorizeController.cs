using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business;
using DataAccess;
using Ext.Net;
using Ext.Net.MVC;
using Service;
using Service.HierarchyDto;
using ServiceStack.Common.Extensions;

namespace ZhiDiExt.Controllers
{
    [Authorize]
    public class SAManagementAuthorizeController : BaseController.BaseController
    {              
        public  RbacService _service=new RbacService();
        //private string _orgId = "45980ad8-ebfe-447b-a6de-b060314ec847";
        //
        // GET: /SAManagementAuthorize/

        public ActionResult Index()
        {
            return View(GetToolarPermission());
        }

        public ActionResult GetChildren(string node)
        {
            return Json(_service.GetManagementAuthorize(node, GetOrganizationId()), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetManagementRoleAuthorize(string managementOrgId)
        {
            var data = _service.GetManagementAdminRoleAuthorize(GetOrganizationId(), managementOrgId);
            return this.Store(data, data.Count());
        }

        public ActionResult SaveManagementAuthorize(string managementOrgId, string authorizes)
        {
            
            ChangeRecords<ManagementAuthorizeDTO> records =
                new StoreDataHandler(authorizes).BatchObjectData<ManagementAuthorizeDTO>();
            SA_Organization managementOrg = _service.GetOrgById(managementOrgId);
            foreach (var rec in records.Updated)
            {
                if (rec.Granted.ToLower() == "true")
                {
                    SA_Authorize authorize=new SA_Authorize();
                    authorize.OrgId = managementOrg.Id;
                    authorize.OrgName = managementOrg.Name;
                    authorize.OrgIdPath = managementOrg.IdPath;
                    authorize.OrgNamePath = managementOrg.NamePath;
                    authorize.AuthorizeRoleId = rec.RoleId;
                    authorize.CreatedBy = GetCurrentUserName();
                    authorize.CreatedDate = DateTime.Now;
                    authorize.CreatedByIdPath = GetOrganizationIdPath();
                    authorize.CreatedByNamePath = GetOrganizationNamePath();
                    _service.AddAuthorize(authorize);
                }
                else
                {
                    _service.DeleteAuthorize(managementOrg.Id, rec.RoleId);
                }
            }
            _service.Save();
            return this.Direct();
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
