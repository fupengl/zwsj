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
    public class KeHuLianXiRenController : BaseController.BaseController
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
                Model = new KeHuLianXiRen {}
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
                Model = _service.GetKeHuLianXiRenById(id)
            }; 
            
        }        

        public ActionResult Delete(int id)
        {
            if (id == -1)
            {                
                return this.Direct();
            }

            _service.DeleteKeHuLianXiRen(id);
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
            return EnumerableExtensions.ToList<object>(_service.GetAllKeHuLianXiRen());
        }   

        public override ActionResult Refresh()
        {
            this.GetCmp<Store>("storeKeHuLianXiRen").Reload();
            return this.Direct();
        }

        public ActionResult Insert(KeHuLianXiRen entity)
        {
            if (ModelState.IsValid)
            {
                entity.CreatedBy = GetCurrentUserName();
                entity.CreatedDate = DateTime.Now;

                _service.AddKeHuLianXiRen(entity);
                _service.Save();
                
                this.GetCmp<Window>("windowKeHuLianXiRen").Hide();
                this.GetCmp<Store>("storeKeHuLianXiRen").Reload();     
                
                return this.Direct();
            }
            return this.Direct();
        }

        public ActionResult Update(KeHuLianXiRen entity)
        {
            if (ModelState.IsValid)
            {

                entity.LastModifiedBy = GetCurrentUserName();
                entity.LastModifiedDate = DateTime.Now;

                _service.UpdateKeHuLianXiRen(entity);
                _service.Save();
                
                var window = this.GetCmp<Window>("windowKeHuLianXiRen");
                window.Hide();
                this.GetCmp<Store>("storeKeHuLianXiRen").Reload();
                return this.Direct();
            }
            
            return this.Direct();
        }
    }
}
