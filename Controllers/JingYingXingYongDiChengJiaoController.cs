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
    public class JingYingXingYongDiChengJiaoController : BaseController.BaseController
    {
        JingYingXingYongDiService _service = new JingYingXingYongDiService(GetActiveOrgFilter());

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
                Model = new JingYingXingYongDiChengJiao {}
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
                Model = _service.GetJingYingXingYongDiById(id)
            }; 
            
        }        

        public ActionResult Delete(int id)
        {
            if (id == -1)
            {                
                return this.Direct();
            }

            _service.DeleteJingYingXingYongDi(id);
            _service.Save();
            return View("Index", GetToolarPermission());
        }

        public ActionResult DeleteAll()
        {

            _service.DeleteAllJingYingXingYongDi();
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
            return EnumerableExtensions.ToList<object>(_service.GetAllJingYingXingYongDi());
        }   


        public override ActionResult Refresh()
        {
            this.GetCmp<Store>("storeJingYingXingYongDiChengJiao").Reload();
            return this.Direct();
        }

        public ActionResult Insert(JingYingXingYongDiChengJiao entity)
        {
            if (ModelState.IsValid)
            {
                entity.OrgNamePath = GetOrganizationNamePath();
                entity.CreatedBy = GetCurrentUserName();
                entity.CreatedDate = DateTime.Now;

                _service.AddJingYingXingYongDi(entity);
                _service.Save();
                
                this.GetCmp<Window>("windowJingYingXingYongDiChengJiao").Hide();
                this.GetCmp<Store>("storeJingYingXingYongDiChengJiao").Reload();     
                
                return this.Direct();
            }
            return this.Direct();
        }

        public ActionResult Update(JingYingXingYongDiChengJiao entity)
        {
            if (ModelState.IsValid)
            {

                entity.LastModifiedBy = GetCurrentUserName();
                entity.LastModifiedDate = DateTime.Now;

                _service.UpdateJingYingXingYongDi(entity);
                _service.Save();
                
                var window = this.GetCmp<Window>("windowJingYingXingYongDiChengJiao");
                window.Hide();
                this.GetCmp<Store>("storeJingYingXingYongDiChengJiao").Reload();
                return this.Direct();
            }
            
            return this.Direct();
        }

    }
}
