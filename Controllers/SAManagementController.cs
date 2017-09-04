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
    public class SAManagementController : BaseController.BaseController
    {
        private RbacService _service=new RbacService();
        //
        // GET: /SABusinessManagement/

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


        public StoreResult GetBusinessManagement(string node,string orgId, string managementType)
        {
            var data=_service.GetBusinessManagement(node, orgId, managementType);
            
            return this.Store(data);
        }

        public ActionResult SaveBusinessManagement(string currOrg,string businessManagement)
        {
            ChangeRecords<BusinessManagementDTO> records =
                new StoreDataHandler(businessManagement).BatchObjectData<BusinessManagementDTO>();
            var curOrganization = _service.GetOrgById(currOrg);
            foreach (var updatedRec in records.Updated)
            {
                if (updatedRec.Granted.ToLower() == "true")
                {
                    var targetOrganization = _service.GetOrgById(updatedRec.Id);
                    SA_Management busManagement = new SA_Management
                    {
                        ManagementType = updatedRec.ManagementType,
                        OrgId = curOrganization.Id,
                        OrgName = curOrganization.Name,
                        OrgIdPath = curOrganization.IdPath,
                        OrgNamePath = curOrganization.NamePath,
                        ManagementOrgId = targetOrganization.Id,
                        ManagementOrgName = targetOrganization.Name,
                        ManagementOrgIdPath = targetOrganization.IdPath,
                        ManagementOrgNamePath = targetOrganization.NamePath,
                        CreatedBy = GetCurrentUserName(),
                        CreatedDate = DateTime.Now,
                        CreatedByIdPath = GetOrganizationIdPath(),
                        CreatedByNamePath = GetOrganizationNamePath()
                    };

                    _service.AddBusinessManagement(busManagement);
                }
                else
                {
                    _service.DeleteBusinessManagement(currOrg, updatedRec.Id,ManagementTypes.Business.ToString());
                }
            }
            _service.Save();
            return this.Direct();
        }
    }
}
