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
    public class SADataAuthorizeController : BaseController.BaseController
    {
        private RbacService _service=new RbacService();
        //
        // GET: /SADataAuthorize/

        public ActionResult Index()
        {
            return View(GetToolarPermission());
        }        

        public ActionResult SaveAuthorize(string orgId, string authorizes)
        {
            //IUnitOfWork uow = RepositoryHelper.GetUnitOfWork();
            //SA_OrganizationBLL organizationBLL = new SA_OrganizationBLL(uow);
            //SA_AuthorizeBLL authorizeBLL = new SA_AuthorizeBLL(uow);

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
            var data = _service.GetAuthorizeDataRoleByOrg(orgId);

            return this.Store(data, data.Count());
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
