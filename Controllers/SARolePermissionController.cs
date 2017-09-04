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

namespace ZhiDiExt.Controllers
{
    [Authorize]
    public class SARolePermissionController : BaseController.BaseController
    {
        private RbacService _service=new RbacService()
            ;
        public StoreResult GetRolePermission(string node, string roleId)
        {
            var data = _service.GetAdminRolePermission(ConvertToInt(node),ConvertToInt(roleId));
            
            return this.Store(data);
        }

        protected override List<object> GetData()
        {
            throw new NotImplementedException();
        }

        public override ActionResult Refresh()
        {
            throw new NotImplementedException();
        }

        public ActionResult SavePermission(string rolePermissions)
        {
            ChangeRecords<SA_RolePermission> records =
                new StoreDataHandler(rolePermissions).BatchObjectData<SA_RolePermission>();
            foreach (var saRolePermission in records.Updated)
            {
                _service.InsertOrUpdateRolePermission(saRolePermission);
            }
            _service.Save();
            return this.Direct();
        }
    }
}
