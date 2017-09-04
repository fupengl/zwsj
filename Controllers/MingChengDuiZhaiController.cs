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
    public class MingChengDuiZhaiController : BaseController.BaseController
    {
        private readonly MingChengDuiZhaiService _service = new MingChengDuiZhaiService(GetActiveOrgFilter());

        //
        // GET: /DataDictionary/

        public ActionResult Index(string wuYeYongTu)
        {
            ViewBag.WuYeYongTu = wuYeYongTu;
            return View(GetToolarPermission());
        }

        public PartialViewResult Create(string wuYeYongTu, string wuYeBianHao="")
        {
            if (string.IsNullOrEmpty(wuYeBianHao))
            {
                return new PartialViewResult
                {
                    ViewName = "Create",
                    Model = new MingChengDuiZhai {WuYeYongTu = GetWuYeYongTuChn(wuYeYongTu)}
                };
            }
            else
            {
                return new PartialViewResult
                {
                    ViewName = "CreateSh",
                    Model = new MingChengDuiZhai { WuYeYongTu = GetWuYeYongTuChn(wuYeYongTu),WuYeBianHao=wuYeBianHao }
                };
            }
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
                Model = _service.GetMingChengDuiZhaiById(id)
            }; 
            
        }        

        public ActionResult Delete(int id)
        {
            if (id == -1)
            {                
                return this.Direct();
            }

            _service.DeleteMingChengDuiZhai(id);
            _service.Save();
            return View("Index", GetToolarPermission());
        }

        public ActionResult DeleteAll()
        {
            _service.DeleteAllMingChengDuiZhai();
            _service.Save();
            return View("Index", GetToolarPermission());
        }

        public ActionResult GetAll(string wuYeYongTu, StoreRequestParameters parameters = null)
        {
            var data = EnumerableExtensions.ToList<object>(_service.GetAllMingChengDuiZhai(GetWuYeYongTuChn(wuYeYongTu)));

            if (parameters == null)
            {
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return this.Store(FilterSortAndPaging(data, parameters), data.Count());
        }

        protected override List<object> GetData()
        {
            //throw new NotImplementedException();
            return EnumerableExtensions.ToList<object>(_service.GetAllMingChengDuiZhai("住宅"));
        }   

        public override ActionResult Refresh()
        {
            this.GetCmp<Store>("storeMingChengDuiZhai").Reload();
            return this.Direct();
        }

        public ActionResult Insert(MingChengDuiZhai entity)
        {
            if (ModelState.IsValid)
            {
                entity.CreatedBy = GetCurrentUserName();
                entity.CreatedDate = DateTime.Now;
                entity.OrgNamePath = GetOrganizationNamePath();
                entity.AreaPath = GetOrganizationAreaPath();
                _service.AddMingChengDuiZhai(entity);
                _service.Save();
                
                this.GetCmp<Window>("windowMingChengDuiZhai").Hide();
                this.GetCmp<Store>("storeMingChengDuiZhai").Reload();     
                
                return this.Direct();
            }
            return this.Direct();
        }

        public ActionResult Update(MingChengDuiZhai entity)
        {
            if (ModelState.IsValid)
            {

                entity.LastModifiedBy = GetCurrentUserName();
                entity.LastModifiedDate = DateTime.Now;

                _service.UpdateMingChengDuiZhai(entity);
                _service.Save();
                
                var window = this.GetCmp<Window>("windowMingChengDuiZhai");
                window.Hide();
                this.GetCmp<Store>("storeMingChengDuiZhai").Reload();
                return this.Direct();
            }
            
            return this.Direct();
        }

        public ActionResult GetPingYin(string wuYeMingCheng)
        {
            this.GetCmp<TextField>("PinYing").Value = ChinesePinYin.GetPinYin(wuYeMingCheng);
            this.GetCmp<TextField>("PinYingShouZiMu").Value = ChinesePinYin.GetFirstPinyin(wuYeMingCheng);
            return this.Direct();
        }

        public ActionResult GetWuYeMingCheng(string wuYeMingCheng)
        {
            this.GetCmp<TextField>("WuYeMingCheng").Value = wuYeMingCheng;//ChinesePinYin.GetFirstPinyin(wuYeMingCheng);
            return this.Direct();
        }

        public ActionResult GetByWuYeBianHao(string wuYeBianHao,string wuYeYongTu, StoreRequestParameters parameters = null)
        {
            var data =
                EnumerableExtensions.ToList<object>(
                    _service.GetAllMingChengDuiZhai(GetWuYeYongTuChn(wuYeYongTu)).Where(x => x.WuYeBianHao == wuYeBianHao));

            if (parameters == null)
            {
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return this.Store(FilterSortAndPaging(data, parameters), data.Count());
        }
    }
}
