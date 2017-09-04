using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DataAccess;
using Ext.Net;
using Ext.Net.MVC;
using Infrastructure;
using Service;
using PartialViewResult = Ext.Net.MVC.PartialViewResult;
using ServiceStack.Common.Extensions;

namespace ZhiDiExt.Controllers
{
    [Authorize]
    public class ShangYeJiChuXinXiShController : BaseController.BaseController
    {
        readonly ShangYeService _service=new ShangYeService(GetActiveOrgFilter());
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
                Model = new ShangYeJiChuXinXi()
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
                Model = _service.GetShangYeJiChuXinXiById(id)
            }; 
            
        }        

        public ActionResult Delete(int id)
        {
            if (id == -1)
            {                
                return this.Direct();
            }

            _service.DeleteShangYeJiChuXinXi(id);
            _service.Save();
            return View("Index", GetToolarPermission());
        }

        public ActionResult DeleteAll()
        {
            _service.DeleteAllShangYeJiChuXinXi();
            _service.Save();
            return View("Index", GetToolarPermission());
        }

        public ActionResult GetAll(StoreRequestParameters parameters = null)
        {
            var data = GetData();

            if (parameters == null)
            {
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return this.Store(FilterSortAndPaging(data, parameters), data.Count());
        }

        protected override List<object> GetData()
        {
            return EnumerableExtensions.ToList<object>(_service.GetAllShangYeJiChuXinXi());
        }   


        public override ActionResult Refresh()
        {
            this.GetCmp<Store>("storeShangYeJiChuXinXi").Reload();
            return this.Direct();
        }

        public ActionResult Insert(ShangYeJiChuXinXi entity)
        {
            if (ModelState.IsValid)
            {
                entity.OrgNamePath = GetOrganizationNamePath();
                entity.CreatedBy = GetCurrentUserName();
                entity.CreatedDate = DateTime.Now;

                _service.AddShangYeJiChuXinXi(entity);
                _service.Save();
                
                this.GetCmp<Window>("windowShangYeJiChuXinXi").Hide();
                this.GetCmp<Store>("storeShangYeJiChuXinXi").Reload();     
                
                return this.Direct();
            }
            return this.Direct();
        }

        public ActionResult Update(ShangYeJiChuXinXi entity)
        {
            if (ModelState.IsValid)
            {

                entity.LastModifiedBy = GetCurrentUserName();
                entity.LastModifiedDate = DateTime.Now;

                _service.UpdateShangYeJiChuXinXi(entity);
                _service.Save();
                
                var window = this.GetCmp<Window>("windowShangYeJiChuXinXi");
                window.Hide();
                this.GetCmp<Store>("storeShangYeJiChuXinXi").Reload();
                return this.Direct();
            }
            
            return this.Direct();
        }

    }
}
