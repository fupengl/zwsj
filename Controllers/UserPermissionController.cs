using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business;
using Ext.Net.MVC;
using NPOI.SS.Formula.Functions;
using Service;

namespace ZhiDiExt.Controllers
{
    [Authorize]
    public class UserPermissionController : BaseController.BaseController
    {
        public StoreResult GetAuthorizedPermission(string node, string orgId)
        {
            AdminRoleService adminRoleService=new AdminRoleService();
            
            var data = adminRoleService.GetAdminAuthoirzeByOrgId(orgId).ToList();
            var query = data.Where(x => x.ParentId == ConvertToInt(node)).Select(
                x =>
                    new
                    {
                        x.Id,
                        x.ParentId,
                        x.Name,
                        x.Description,
                        x.Url,
                        Granted=x.Granted.ToString().ToLower(),
                        x.AreaPath,
                        x.ItemType,
                        leaf = !data.Any(o => o.ParentId == x.Id)
                    });

            return this.Store(query);
        }

        protected override List<object> GetData()
        {
            throw new NotImplementedException();
        }

        public override ActionResult Refresh()
        {
            throw new NotImplementedException();
        }

        public ActionResult GetAuthorizedDataPermission(string node, string orgId)
        {
            DataRoleService dataRoleService=new DataRoleService();
            
            var data = dataRoleService.GetDataAuthoirzeByOrgId(orgId).ToList();
            var query = data.Where(x => x.ParentId == (node == "root" ? "" : node)).Select(
                x =>
                    new
                    {
                        x.Id,
                        x.ParentId,
                        x.Name,
                        x.Description,
                        Granted = x.Granted.ToString().ToLower(),
                        leaf = !data.Any(o => o.ParentId == x.Id)
                    });

            return this.Store(query);

        }
    }
}
