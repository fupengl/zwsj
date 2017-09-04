using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DataAccess;
using Ext.Net;
using Ext.Net.MVC;
using Infrastructure;
using Service;
//using ServiceStack.Common.Extensions;
using ServiceStack.Common;
using PartialViewResult = Ext.Net.MVC.PartialViewResult;


namespace ZhiDiExt.Controllers
{
    [Authorize]
    public class CategoryController : BaseController.BaseController
    {
        private readonly DataDictionaryService _service = new DataDictionaryService();

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
                Model = new Category()
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
                Model = _service.GetCategory(id)
            }; 
            
        }        

        public ActionResult Delete(int id)
        {
            if (id == -1)
            {                
                return this.Direct();
            }

            _service.DeleteCategory(id);
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
            return _service.GetAllCategory().ToObjects<object>();
        }   

        public override ActionResult Refresh()
        {
            this.GetCmp<Store>(ExtHelper.storeCategory.ToString()).Reload();
            return this.Direct();
        }

        public ActionResult Insert(Category entity)
        {
            if (ModelState.IsValid)
            {
                if (_service.GetCategoryByCode(entity.Code) == null)
                {
                    if (string.IsNullOrEmpty(entity.RecordType))
                    {
                        entity.RecordType = RecordType.User.ToString();
                    }
                    entity.CreatedBy = GetCurrentUserName();
                    entity.CreatedDate = DateTime.Now;
                    _service.AddCategory(entity);
                    _service.Save();
                }
                else
                {
                    X.Msg.Alert(MsgInfo.TI_SHI, MsgInfo.INSERT_CHECK_EXIT_CODE).Show();
                    return this.Direct();
                }

                this.GetCmp<Window>(ExtHelper.windowCategory.ToString()).Hide();
                this.GetCmp<Store>(ExtHelper.storeCategory.ToString()).Reload();     
                
                return this.Direct();
            }
            return this.Direct();
        }

        public ActionResult Update(Category entity)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(entity.RecordType))
                {
                    entity.RecordType = RecordType.User.ToString();
                }
                entity.LastModifiedBy = GetCurrentUserName();
                entity.LastModifiedDate = DateTime.Now;
                _service.UpdateCategory(entity);
                _service.Save();
                
                var window = this.GetCmp<Window>(ExtHelper.windowCategory.ToString());
                window.Hide();
                this.GetCmp<Store>(ExtHelper.storeCategory.ToString()).Reload();
                return this.Direct();
            }
            
            return this.Direct();
        }
    }
}
