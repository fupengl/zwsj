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
    public class BanGongChengJiaoController : BaseController.BaseController
    {
        private readonly BanGongService _service = new BanGongService(GetActiveOrgFilter());
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
                    Model = new BanGongChengJiao()
                };
            }
            return new PartialViewResult
            {
                ViewName = "CreateSh",
                Model = new BanGongChengJiao {WuYeBianHao = wuYeBianHao,WuYeMingCheng = _service.GetWuYeMingCheng(wuYeBianHao)}
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
                Model = _service.GetBanGongChengJiao(id)
            }; 
            
        }        

        public ActionResult Delete(int id)
        {
            if (id == -1)
            {                
                return this.Direct();
            }

            _service.DeleteBanGongChengJiao(id);
            _service.Save();
            return View("Index", GetToolarPermission());
        }

        public ActionResult DeleteAll()
        {
            _service.DeleteAllBanGongChengJiao();
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
            return EnumerableExtensions.ToList<object>(_service.GetAllBanGongChengJiao());
        }  

        public override ActionResult Refresh()
        {
            this.GetCmp<Store>("storeBanGongChengJiao").Reload();
            return this.Direct();
        }

        public ActionResult Insert(BanGongChengJiao entity)
        {
            if (ModelState.IsValid)
            {
                entity.OrgNamePath = GetOrganizationNamePath();
                entity.CreatedBy = GetCurrentUserName();
                entity.CreatedDate = DateTime.Now;
                
                _service.AddBanGongChengJiao(entity);
                _service.Save();
                
                this.GetCmp<Window>("windowBanGongChengJiao").Hide();
                this.GetCmp<Store>("storeBanGongChengJiao").Reload();     
                
                return this.Direct();
            }
            return this.Direct();
        }

        public ActionResult Update(BanGongChengJiao entity)
        {
            if (ModelState.IsValid)
            {
                entity.LastModifiedBy = GetCurrentUserName();
                entity.LastModifiedDate = DateTime.Now;

                _service.UpdateBanGongChengJiao(entity);
                _service.Save();
                
                var window = this.GetCmp<Window>("windowBanGongChengJiao");
                window.Hide();
                this.GetCmp<Store>("storeBanGongChengJiao").Reload();
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
                    _service.GetAllBanGongChengJiao().Where(x => x.WuYeBianHao == wuYeBianHao));

            if (parameters == null)
            {
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return this.Store(FilterSortAndPaging(data, parameters), data.Count());
        }
    }
}
