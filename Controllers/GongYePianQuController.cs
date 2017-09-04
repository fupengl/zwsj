using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DataAccess;
using Ext.Net;
using Ext.Net.MVC;
using Infrastructure;
using Service;
using EnumerableExtensions = ServiceStack.Common.Extensions.EnumerableExtensions;
using PartialViewResult = Ext.Net.MVC.PartialViewResult;

namespace ZhiDiExt.Controllers
{
    public class GongYePianQuController : BaseController.BaseController
    {
        private readonly GongYePianQuService _gongYePianQuService = new GongYePianQuService(GetActiveOrgFilter());

        //
        // GET: /DataDictionary/

        public ActionResult Index()
        {
            return View(GetToolarPermission());
            //return View();
        }

        public PartialViewResult Create()
        {
            return new PartialViewResult
            {
                ViewName = "Create",
                Model = new GongYePianQu ()
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
                Model = _gongYePianQuService.GetGongYePianQuById(id)
            }; 
            
        }        

        public ActionResult Delete(int id)
        {
            if (id == -1)
            {                
                return this.Direct();
            }

            _gongYePianQuService.DeleteGongYePianQu(_gongYePianQuService.GetGongYePianQuById(id));
            _gongYePianQuService.Save();
            return View("Index");
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
            return EnumerableExtensions.ToList<object>(_gongYePianQuService.GetAllGongYePianQu());
        }

        public ActionResult GetAllGongYePianQu()
        {
           return  this.Store(_gongYePianQuService.GetAllGongYePianQu());
        }

        public override ActionResult Refresh()
        {
            this.GetCmp<Store>("storeGongYePianQu").Reload();
            return this.Direct();
        }

        public ActionResult Insert(GongYePianQu entity)
        {
            if (ModelState.IsValid)
            {
                entity.CreatedBy = GetCurrentUserName();
                entity.CreatedDate = DateTime.Now;
                entity.OrgNamePath = GetOrganizationNamePath();
                entity.AreaPath = GetOrganizationAreaPath();

                _gongYePianQuService.AddGongYePianQu(entity);
                _gongYePianQuService.Save();
                
                this.GetCmp<Window>("windowGongYePianQu").Hide();
                this.GetCmp<Store>("storeGongYePianQu").Reload();     
                
                return this.Direct();
            }
            return this.Direct();
        }

        public ActionResult Update(GongYePianQu entity)
        {
            if (ModelState.IsValid)
            {

                entity.LastModifiedBy = GetCurrentUserName();
                entity.LastModifiedDate = DateTime.Now;

                _gongYePianQuService.UpdateGongYePianQu(entity);
                _gongYePianQuService.Save();
                
                var window = this.GetCmp<Window>("windowGongYePianQu");
                window.Hide();
                this.GetCmp<Store>("storeGongYePianQu").Reload();
                return this.Direct();
            }
            
            return this.Direct();
        }

//----------------------------------自动补全--------------------------------------//
//        public ActionResult AutoComplete(int start, int limit, int page, string query)
//        {
//            var data = _bll.GetAll().Where(x => x.WuYeBianHao.Contains(query)).OrderBy(x=>x.WuYeBianHao).ToPagedList(page, limit);
//            return this.Store(data, data.Count);
//        }
//----------------------------------自动补全--------------------------------------//

    }
}
