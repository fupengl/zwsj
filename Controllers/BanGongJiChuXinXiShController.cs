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
//using ServiceStack.Common.Extensions;
using ServiceStack.Common;

namespace ZhiDiExt.Controllers
{
    [Authorize]
    public class BanGongJiChuXinXiShController : BaseController.BaseController
    {
        readonly BanGongService _service = new BanGongService(GetActiveOrgFilter());
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
                Model = new BanGongJiChuXinXi()
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
                Model = _service.GetBanGongJiChuXinXiById(id)
            }; 
            
        }        

        public ActionResult Delete(int id)
        {
            if (id == -1)
            {                
                return this.Direct();
            }

            _service.DeleteBanGongJiChuXinXi(id);
            _service.Save();
            return View("Index", GetToolarPermission());
        }

        public ActionResult DeleteAll()
        {
            _service.DeleteAllBanGongJiChuXinXi();
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
            //return EnumerableExtensions.ToList<object>(_service.GetAllBanGongJiChuXinXis());
            return _service.GetAllBanGongJiChuXinXis().ToObjects<object>();
        }   

        public override ActionResult Refresh()
        {
            this.GetCmp<Store>("storeBanGongJiChuXinXi").Reload();
            return this.Direct();
        }

        public ActionResult Insert(BanGongJiChuXinXi entity)
        {
            if (ModelState.IsValid)
            {
                entity.OrgNamePath = GetOrganizationNamePath();
                entity.CreatedBy = GetCurrentUserName();
                entity.CreatedDate = DateTime.Now;

                _service.AddBanGongJiChuXinXi(entity);
                _service.Save();
                
                this.GetCmp<Window>("windowBanGongJiChuXinXi").Hide();
                this.GetCmp<Store>("storeBanGongJiChuXinXi").Reload();     
                
                return this.Direct();
            }
            return this.Direct();
        }

        public ActionResult Update(BanGongJiChuXinXi entity)
        {
            if (ModelState.IsValid)
            {

                entity.LastModifiedBy = GetCurrentUserName();
                entity.LastModifiedDate = DateTime.Now;

                _service.UpdateBanGongJiChuXinXi(entity);
                _service.Save();
                
                var window = this.GetCmp<Window>("windowBanGongJiChuXinXi");
                window.Hide();
                this.GetCmp<Store>("storeBanGongJiChuXinXi").Reload();
                return this.Direct();
            }
            
            return this.Direct();
        }
        
    }
}
