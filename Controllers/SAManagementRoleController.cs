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
    public class SAManagementRoleController : BaseController.BaseController
    {
        RbacService _service=new RbacService();
        //
        // GET: /SAManagementRole/

        public ActionResult Index()
        {
            return View(GetToolarPermission());
        }

        public ActionResult GetManagementRoles(string orgId, StoreRequestParameters parameters = null)
        {

            var data = EnumerableExtensions.ToList<object>(_service.GetManagementRoles(orgId));
            
            if (parameters == null)
            {
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return this.Store(FilterSortAndPaging(data, parameters), data.Count());
        }

        protected override List<object> GetData()
        {
            throw new NotImplementedException();
        }

        public override ActionResult Refresh()
        {
            this.GetCmp<Store>("storeSA_ManagementRole").Reload();
            return this.Direct();
        }

        public ActionResult SaveManagementRole(string currOrg, string roleManagement)
        {
            ChangeRecords<ManagementRoleDTO> records =
                new StoreDataHandler(roleManagement).BatchObjectData<ManagementRoleDTO>();

            var curOrganization = _service.GetOrgById(currOrg);

            foreach (var updatedRec in records.Updated)
            {
                if (updatedRec.Granted.ToLower() == "true")
                {
                    SA_ManagementRole role=new SA_ManagementRole
                    {
                        OrgId = curOrganization.Id,
                        OrgName = curOrganization.Name,
                        OrgIdPath = curOrganization.IdPath,
                        OrgNamePath = curOrganization.NamePath,
                        RoleId = ConvertToInt(updatedRec.Id),
                        RoleType = updatedRec.RoleType,
                        CreatedBy = GetCurrentUserName(),
                        CreatedDate = DateTime.Now,
                        CreatedByIdPath = GetOrganizationIdPath(),
                        CreatedByNamePath = GetOrganizationNamePath()
                    };
                    _service.AddManagementRole(role);
                }
                else
                {
                    _service.DeleteManagementRole(updatedRec.ManagementId);
                }
            }
            _service.Save();

            return this.Direct();
        }
    }
}
