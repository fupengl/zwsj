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
using ServiceStack.Common.Extensions;
using PartialViewResult = Ext.Net.MVC.PartialViewResult;

namespace ZhiDiExt.Controllers
{
    [Authorize]
    public class SubCategoryController : BaseController.BaseController
    {
        DataDictionaryService _service=new DataDictionaryService();

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
                Model = new SubCategory {}
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
                Model = _service.GetSubCategory(id)
            }; 
            
        }        

        public ActionResult Delete(int id)
        {
            if (id == -1)
            {                
                return this.Direct();
            }

            _service.DeleteSubCategory(id);
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
            return EnumerableExtensions.ToList<object>(_service.GetAllSubCategory());
        }   

        public override ActionResult Refresh()
        {
            this.GetCmp<Store>("storeSubCategory").Reload();
            return this.Direct();
        }

        public ActionResult Insert(SubCategory entity)
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

                    _service.AddSubCategory(entity);
                    _service.Save();
                }
                else
                {
                    X.Msg.Alert(MsgInfo.TI_SHI, MsgInfo.INSERT_CHECK_EXIT_CODE).Show();
                    return this.Direct();
                }

                this.GetCmp<Window>("windowSubCategory").Hide();
                this.GetCmp<Store>("storeSubCategory").Reload();     
                
                return this.Direct();
            }
            return this.Direct();
        }

        public ActionResult Update(SubCategory entity)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(entity.RecordType))
                {
                    entity.RecordType = RecordType.User.ToString();
                }

                entity.LastModifiedBy = GetCurrentUserName();
                entity.LastModifiedDate = DateTime.Now;

                _service.UpdateSubCategory(entity);
                _service.Save();
                
                var window = this.GetCmp<Window>("windowSubCategory");
                window.Hide();
                this.GetCmp<Store>("storeSubCategory").Reload();
                return this.Direct();
            }
            
            return this.Direct();
        }

        public JsonResult GetAllByCategoryId(string category)
        {
            if (!string.IsNullOrEmpty(category))
            {
                return Json(_service.GetAllSubCategoryByCategoryId(category), JsonRequestBehavior.AllowGet);
            }
            return Json(_service.GetAllSubCategory(), JsonRequestBehavior.AllowGet);
        }
    }
}
