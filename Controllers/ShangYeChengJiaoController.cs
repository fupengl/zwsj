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
    public class ShangYeChengJiaoController : BaseController.BaseController
    {
        readonly ShangYeService _service = new ShangYeService(GetActiveOrgFilter());
        //
        // GET: /DataDictionary/

        public ActionResult Index()
        {
            return View(GetToolarPermission());
        }

        public PartialViewResult Create(string wuYeBianHao="")
        {
            if (string.IsNullOrEmpty(wuYeBianHao))
            {
                return new PartialViewResult
                {
                    ViewName = "Create",
                    Model = new ShangYeChengJiao { }
                }; 
            }
            return new PartialViewResult
            {
                ViewName = "CreateSh",
                Model = new ShangYeChengJiao { WuYeBianHao = wuYeBianHao,WuYeMingCheng = _service.GetWuYeMingCheng(wuYeBianHao)}
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
                Model = _service.GetShangYeChengJiaoById(id)
            }; 
            
        }        

        public ActionResult Delete(int id)
        {
            if (id == -1)
            {                
                return this.Direct();
            }

            _service.DeleteShangYeChengJiao(id);
            _service.Save();
            return View("Index", GetToolarPermission());
        }

        public ActionResult DeleteAll()
        {
            _service.DeleteAllShangYeChengJiao();
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
            return EnumerableExtensions.ToList<object>(_service.GetAllShangYeChengJiao());
        }   

        public override ActionResult Refresh()
        {
            this.GetCmp<Store>("storeShangYeChengJiao").Reload();
            return this.Direct();
        }

        public ActionResult Insert(ShangYeChengJiao entity)
        {
            if (ModelState.IsValid)
            {
                entity.OrgNamePath = GetOrganizationNamePath();
                entity.CreatedBy = GetCurrentUserName();
                entity.CreatedDate = DateTime.Now;

                _service.AddShangYeChengJiao(entity);
                _service.Save();
                
                this.GetCmp<Window>("windowShangYeChengJiao").Hide();
                this.GetCmp<Store>("storeShangYeChengJiao").Reload();     
                
                return this.Direct();
            }
            return this.Direct();
        }

        public ActionResult Update(ShangYeChengJiao entity)
        {
            if (ModelState.IsValid)
            {

                entity.LastModifiedBy = GetCurrentUserName();
                entity.LastModifiedDate = DateTime.Now;

                _service.UpdateShangYeChengJiao(entity);
                _service.Save();
                
                var window = this.GetCmp<Window>("windowShangYeChengJiao");
                window.Hide();
                this.GetCmp<Store>("storeShangYeChengJiao").Reload();
                return this.Direct();
            }
            
            return this.Direct();
        }

        public ActionResult UpdateWuYeMingCheng(string wuYeBianHao)
        {
            if (!string.IsNullOrEmpty(wuYeBianHao))
            {
                this.GetCmp<TextField>("WuYeMingCheng").Value = _service.GetWuYeMingCheng(wuYeBianHao);
                return this.Direct();
            }

            return this.Direct();
        }

        public ActionResult GetByWuYeBianHao(string wuYeBianHao, StoreRequestParameters parameters = null)
        {
            var data =
                EnumerableExtensions.ToList<object>(
                    _service.GetAllShangYeChengJiao().Where(x => x.WuYeBianHao == wuYeBianHao));

            if (parameters == null)
            {
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return this.Store(FilterSortAndPaging(data, parameters), data.Count());
        }
    }
}
