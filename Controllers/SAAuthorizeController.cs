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
using NPOI.SS.Formula.Functions;
using Service;
using Service.HierarchyDto;
using EnumerableExtensions = ServiceStack.Common.Extensions.EnumerableExtensions;
using PartialViewResult = Ext.Net.MVC.PartialViewResult;

namespace ZhiDiExt.Controllers
{
    [Authorize]
    public class SAAuthorizeController : BaseController.BaseController
    {
        RbacService _service=new RbacService();
        //
        // GET: /DataDictionary/

        public ActionResult Index()
        {
            return View(GetToolarPermission());
        }

        protected override List<object> GetData()
        {            
            return EnumerableExtensions.ToList<object>(_service.GetAuthorizedAdminRoles());
        }

        public override ActionResult Refresh()
        {
            throw new NotImplementedException();
        }

        public ActionResult SaveAuthorize(string orgId, string authorizes)
        {
            ChangeRecords<ManagementAuthorizeDTO> records =
                new StoreDataHandler(authorizes).BatchObjectData<ManagementAuthorizeDTO>();


            var curOrganization = _service.GetOrgById(orgId);

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

        public ActionResult GetAuthorizeByOrganization(string orgId)
        {
            var data = _service.GetAuthorizeAdminRoleByOrg(orgId);

            return this.Store(data, data.Count());
        }
    }
}    
    
