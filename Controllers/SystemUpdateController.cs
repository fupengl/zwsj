using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Service;
using Ext.Net;
using Ext.Net.MVC;
using EnumerableExtensions = ServiceStack.Common.Extensions.EnumerableExtensions;
using PartialViewResult = Ext.Net.MVC.PartialViewResult;

namespace ZhiDiExt.Controllers
{
    public class SystemUpdateController : BaseController.BaseController
    {
        //SystemUpdateBLL _bll = new SystemUpdateBLL();
        private readonly SystemUpdateService _systemUpdateService = new SystemUpdateService();

        //
        // GET: /DataDictionary/

        public ActionResult Index()
        {
            return View(GetToolarPermission());
            //return View();
        }

        //public PartialViewResult Create()
        //{
        //    return new PartialViewResult
        //    {
        //        ViewName = "Create",
        //        Model = new SystemUpdate {}
        //    };
        //}

        //public ActionResult Edit(int id)
        //{
        //    if (id == -1)
        //    {
        //        X.Msg.Alert(MsgInfo.TI_SHI, MsgInfo.PLEASE_SELECT_ROW_TO_EDIT).Show();
        //        return this.Direct();
        //    }
        //    return new PartialViewResult
        //    {
        //        ViewName = "Edit",
        //        Model = _bll.GetById(id)
        //    }; 
            
        //}        

        public ActionResult Delete(int id)
        {
            if (id == -1)
            {
                return this.Direct();
            }

            _systemUpdateService.DeleteSystemUpdate(_systemUpdateService.GetSystemUpdateById(id));
            _systemUpdateService.Save();
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
            return EnumerableExtensions.ToList<object>(_systemUpdateService.GetAllSystemUpdate());
        }   

        public override ActionResult Refresh()
        {
            this.GetCmp<Store>("storeSystemUpdate").Reload();
            return this.Direct();
        }

        //public ActionResult Insert(SystemUpdate entity)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        entity.CreatedBy = GetCurrentUserName();
        //        entity.CreatedDate = DateTime.Now;

        //        _bll.Add(entity);
        //        _bll.Save();
                
        //        this.GetCmp<Window>("windowSystemUpdate").Hide();
        //        this.GetCmp<Store>("storeSystemUpdate").Reload();     
                
        //        return this.Direct();
        //    }
        //    return this.Direct();
        //}

        //public ActionResult Update(SystemUpdate entity)
        //{
        //    if (ModelState.IsValid)
        //    {

        //        entity.LastModifiedBy = GetCurrentUserName();
        //        entity.LastModifiedDate = DateTime.Now;

        //        _bll.Update(entity);
        //        _bll.Save();
                
        //        var window = this.GetCmp<Window>("windowSystemUpdate");
        //        window.Hide();
        //        this.GetCmp<Store>("storeSystemUpdate").Reload();
        //        return this.Direct();
        //    }
            
        //    return this.Direct();
        //}

//----------------------------------自动补全--------------------------------------//
//        public ActionResult AutoComplete(int start, int limit, int page, string query)
//        {
//            var data = _bll.GetAll().Where(x => x.WuYeBianHao.Contains(query)).OrderBy(x=>x.WuYeBianHao).ToPagedList(page, limit);
//            return this.Store(data, data.Count);
//        }
//----------------------------------自动补全--------------------------------------//

    }
}
