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
    public class ZhuZhaiPingGuController : BaseController.BaseController
    {
        private readonly ZhuZhaiService _service=new ZhuZhaiService(GetActiveOrgFilter());
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
                ViewName = ExtHelper.Create.ToString(),//"Create",
                Model = new ZhuZhaiPingGu()
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
                ViewName = ExtHelper.Edit.ToString(),//"Edit",
                Model = _service.GetZhuZhaiPingGuById(id)
            }; 
            
        }        

        public ActionResult Delete(int id)
        {
            if (id == -1)
            {                
                return this.Direct();
            }

            _service.DeleteZhuZhaiPingGu(id);
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
            return _service.GetAllZhuZhaiPingGu().ToObjects<object>();
        }   


        public override ActionResult Refresh()
        {
            this.GetCmp<Store>("storeZhuZhaiPingGu").Reload();
            return this.Direct();
        }

        public ActionResult Insert(ZhuZhaiPingGu entity)
        {
            if (ModelState.IsValid)
            {
                entity.OrgNamePath = GetOrganizationNamePath();
                entity.Man5Nian = GetCheckBoxValue(entity.Man5Nian);
                entity.CreatedBy = GetCurrentUserName();
                entity.CreatedDate = DateTime.Now;

                _service.AddZhuZhaiPingGu(entity);
                _service.Save();
                
                this.GetCmp<Window>("windowZhuZhaiPingGu").Hide();
                this.GetCmp<Store>("storeZhuZhaiPingGu").Reload();     
                
                return this.Direct();
            }
            return this.Direct();
        }

        public ActionResult Update(ZhuZhaiPingGu entity)
        {
            if (ModelState.IsValid)
            {
                entity.Man5Nian = GetCheckBoxValue(entity.Man5Nian);
                entity.LastModifiedBy = GetCurrentUserName();
                entity.LastModifiedDate = DateTime.Now;

                _service.UpdateZhuZhaiPingGu(entity);
                _service.Save();
                
                var window = this.GetCmp<Window>("windowZhuZhaiPingGu");
                window.Hide();
                this.GetCmp<Store>("storeZhuZhaiPingGu").Reload();
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
    }
}
