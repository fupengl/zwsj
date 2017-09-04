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
using PartialViewResult = Ext.Net.MVC.PartialViewResult;
using ServiceStack.Common.Extensions;

namespace ZhiDiExt.Controllers
{
    [Authorize]
    public class KeHuController : BaseController.BaseController
    {
        KeHuService _service=new KeHuService();

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
                Model = new KeHu {}
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
                Model = _service.GetKeHuById(id)
            }; 
            
        }        

        public ActionResult Delete(int id)
        {
            if (id == -1)
            {                
                return this.Direct();
            }

            _service.DeleteKeHu(id);
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
            return EnumerableExtensions.ToList<object>(_service.GetAllKeHu());
        }   


        public override ActionResult Refresh()
        {
            this.GetCmp<Store>("storeKeHu").Reload();
            return this.Direct();
        }

        public ActionResult Insert(KeHu entity)
        {
            if (ModelState.IsValid)
            {
                entity.CreatedBy = GetCurrentUserName();
                entity.CreatedDate = DateTime.Now;

                _service.AddKeHu(entity);
                _service.Save();
                
                this.GetCmp<Window>("windowKeHu").Hide();
                this.GetCmp<Store>("storeKeHu").Reload();     
                
                return this.Direct();
            }
            return this.Direct();
        }

        public ActionResult Update(KeHu entity)
        {
            if (ModelState.IsValid)
            {

                entity.LastModifiedBy = GetCurrentUserName();
                entity.LastModifiedDate = DateTime.Now;

                _service.UpdateKeHu(entity);
                _service.Save();
                
                var window = this.GetCmp<Window>("windowKeHu");
                window.Hide();
                this.GetCmp<Store>("storeKeHu").Reload();
                return this.Direct();
            }
            
            return this.Direct();
        }

    }
}
