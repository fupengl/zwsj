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
using ServiceStack.Common;
using EnumerableExtensions = ServiceStack.Common.Extensions.EnumerableExtensions;
using PartialViewResult = Ext.Net.MVC.PartialViewResult;

namespace ZhiDiExt.Controllers
{
    [Authorize]
    public class ZhuZhaiChengJiaoController : BaseController.BaseController
    {
        ZhuZhaiService _service=new ZhuZhaiService(GetActiveOrgFilter());

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
                    Model = new ZhuZhaiChengJiao { }
                };
            }
            return new PartialViewResult
            {
                ViewName = "CreateSh",
                Model = new ZhuZhaiChengJiao {WuYeBianHao = wuYeBianHao,WuYeMingCheng = _service.GetWuYeMingCheng(wuYeBianHao)}
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
                Model = _service.GetZhuZhaiChengJiaoById(id)
            }; 
            
        }        

        public ActionResult Delete(int id)
        {
            if (id == -1)
            {                
                return this.Direct();
            }

            _service.DeleteZhuZhaiChengJiao(id);
            _service.Save();
            return View("Index", GetToolarPermission());
        }

        public ActionResult DeleteAll()
        {
            _service.DeleteAllZhuZhaiChengJiao();
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
            return EnumerableExtensions.ToList<object>(_service.GetAllZhuZhaiChengJiao());
        }   


        public override ActionResult Refresh()
        {
            this.GetCmp<Store>("storeZhuZhaiChengJiao").Reload();
            return this.Direct();
        }

        public ActionResult Insert(ZhuZhaiChengJiao entity)
        {
            if (ModelState.IsValid)
            {
                entity.OrgNamePath = GetOrganizationNamePath();
                entity.CreatedBy = GetCurrentUserName();
                entity.CreatedDate = DateTime.Now;

                _service.AddZhuZhaiChengJiao(entity);
                _service.Save();
                
                this.GetCmp<Window>("windowZhuZhaiChengJiao").Hide();
                this.GetCmp<Store>("storeZhuZhaiChengJiao").Reload();     
                
                return this.Direct();
            }
            return this.Direct();
        }

        public ActionResult Update(ZhuZhaiChengJiao entity)
        {
            if (ModelState.IsValid)
            {

                entity.LastModifiedBy = GetCurrentUserName();
                entity.LastModifiedDate = DateTime.Now;

                _service.UpdateZhuZhaiChengJiao(entity);
                _service.Save();
                
                var window = this.GetCmp<Window>("windowZhuZhaiChengJiao");
                window.Hide();
                this.GetCmp<Store>("storeZhuZhaiChengJiao").Reload();
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
                    _service.GetAllZhuZhaiChengJiao().Where(x => x.WuYeBianHao == wuYeBianHao));

            if (parameters == null)
            {
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return this.Store(FilterSortAndPaging(data, parameters), data.Count());
        }
    }
}
