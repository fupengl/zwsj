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
    public class BanGongLouCengXiuZhengController : BaseController.BaseController
    {
        private readonly BanGongService _service = new BanGongService(GetActiveOrgFilter());

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
                Model = new BanGongLouCengXiuZheng()
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
                Model = _service.GetBanGongLouCengXiuZheng(id)
            }; 
            
        }        

        public ActionResult Delete(int id)
        {
            if (id == -1)
            {                
                return this.Direct();
            }

            _service.DeleteBanGongLouCengXiuZheng(id);
            _service.Save();
            return View("Index", GetToolarPermission());
        }

        public ActionResult DeleteAll()
        {
            _service.DeleteAllBanGongLouCengXiuZheng();
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
            //return EnumerableExtensions.ToList<object>(_service.GetAllBanGongLouCengXiuZheng());
            return _service.GetAllBanGongLouCengXiuZheng().ToObjects<object>();
        }   


        public override ActionResult Refresh()
        {
            this.GetCmp<Store>("storeBanGongLouCengXiuZheng").Reload();
            return this.Direct();
        }

        public ActionResult Insert(BanGongLouCengXiuZheng entity)
        {
            if (ModelState.IsValid)
            {
                entity.OrgNamePath = GetOrganizationNamePath();
                entity.CreatedBy = GetCurrentUserName();
                entity.CreatedDate = DateTime.Now;

                _service.AddBanGongLouCengXiuZheng(entity);
                _service.Save();
                
                this.GetCmp<Window>("windowBanGongLouCengXiuZheng").Hide();
                this.GetCmp<Store>("storeBanGongLouCengXiuZheng").Reload();     
                
                return this.Direct();
            }
            return this.Direct();
        }

        public ActionResult Update(BanGongLouCengXiuZheng entity)
        {
            if (ModelState.IsValid)
            {

                entity.LastModifiedBy = GetCurrentUserName();
                entity.LastModifiedDate = DateTime.Now;

                _service.UpdateBanGongLouCengXiuZheng(entity);
                _service.Save();
                
                var window = this.GetCmp<Window>("windowBanGongLouCengXiuZheng");
                window.Hide();
                this.GetCmp<Store>("storeBanGongLouCengXiuZheng").Reload();
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
