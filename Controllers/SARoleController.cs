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
using EnumerableExtensions = ServiceStack.Common.Extensions.EnumerableExtensions;
using PartialViewResult = Ext.Net.MVC.PartialViewResult;

namespace ZhiDiExt.Controllers
{
    [Authorize]
    public class SARoleController : BaseController.BaseController
    {
        RbacService _service=new RbacService();
        //
        // GET: /DataDictionary/

        public ActionResult Index()
        {
            return View(GetToolarPermission());
        }

        public PartialViewResult Create()
        {
            return new PartialViewResult
            {
                ViewName = "Create",
                Model = new SA_Role {RoleType = RoleTypes.Admin.ToString()}
            };
        }

        public ActionResult Edit(int id)
        {
            if (id == -1)
            {
                X.Msg.Alert(MsgInfo.TI_SHI, MsgInfo.PLEASE_SELECT_ROW_TO_EDIT).Show();
                return this.Direct();
            }
            return new PartialViewResult
            {
                ViewName = "Edit",
                Model = _service.GetRoleById(id)
            }; 
            
        }        

        public ActionResult Delete(int id)
        {
            if (id == -1)
            {                
                return this.Direct();
            }

            _service.DeleteRole(id);
            _service.Save();
            return View("Index", GetToolarPermission());
        }


        public ActionResult GetAdminRoles(StoreRequestParameters parameters = null)
        {
            var data = EnumerableExtensions.ToList<object>(_service.GetAdminRole());

            if (parameters == null)
            {
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return this.Store(FilterSortAndPaging(data, parameters), data.Count());
        }


        public ActionResult GetDataRoles(StoreRequestParameters parameters = null)
        {
            var data = EnumerableExtensions.ToList<object>(_service.GetDataRole());

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
            this.GetCmp<Store>("storeSA_Role").Reload();
            return this.Direct();
        }

        public ActionResult Insert(SA_Role entity)
        {
            if (ModelState.IsValid)
            {
                entity.CreatedBy = GetCurrentUserName();
                entity.CreatedDate = DateTime.Now;

                _service.AddRole(entity);
                _service.Save();
                
                this.GetCmp<Window>("windowSA_Role").Hide();
                this.GetCmp<Store>("storeSA_Role").Reload();     
                
                return this.Direct();
            }
            return this.Direct();
        }

        public ActionResult Update(SA_Role entity)
        {
            if (ModelState.IsValid)
            {

                entity.LastModifiedBy = GetCurrentUserName();
                entity.LastModifiedDate = DateTime.Now;

                _service.UpdateRole(entity);
                _service.Save();
                
                var window = this.GetCmp<Window>("windowSA_Role");
                window.Hide();
                this.GetCmp<Store>("storeSA_Role").Reload();
                return this.Direct();
            }
            
            return this.Direct();
        }

    }
}
