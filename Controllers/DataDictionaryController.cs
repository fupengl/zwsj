using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Xsl;
using Business;
using DataAccess;
using Ext.Net;
using Ext.Net.MVC;
using Infrastructure;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using PagedList;
using System.Linq.Dynamic;
using Service;
using ServiceStack.Common.Extensions;
using PartialViewResult = Ext.Net.MVC.PartialViewResult;

namespace ZhiDiExt.Controllers
{
    [Authorize]
    public class DataDictionaryController : BaseController.BaseController
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
                Model = new DataDictionary {}
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
                Model = _service.GetDataDictionary(id)
            }; 
            
        }        

        public ActionResult Delete(int id)
        {
            if (id == -1)
            {                
                return this.Direct();
            }

            _service.DeleteDataDictionary(id);
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
            return EnumerableExtensions.ToList<object>(_service.GetAllDataDictionary());
        }   

        public override ActionResult Refresh()
        {
            this.GetCmp<Store>("storeDataDictionary").Reload();
            return this.Direct();
        }

        public ActionResult Insert(DataDictionary entity)
        {
            if (ModelState.IsValid)
            {
                if (_service.GetDataDictionaryByKeyName(entity.SubCategory, entity.KeyName) == null)
                {
                    if (string.IsNullOrEmpty(entity.RecordType))
                    {
                        entity.RecordType = RecordType.User.ToString();
                    }
                    entity.CreatedBy = GetCurrentUserName();
                    entity.CreatedDate = DateTime.Now;

                    _service.AddDataDictionary(entity);
                    _service.Save();
                }
                else
                {
                    X.Msg.Alert(MsgInfo.TI_SHI, MsgInfo.INSERT_CHECK_EXIT_CODE).Show();
                    return this.Direct();
                }

                this.GetCmp<Window>("windowDataDictionary").Hide();
                this.GetCmp<Store>("storeDataDictionary").Reload();     
                
                return this.Direct();
            }
            return this.Direct();
        }

        public ActionResult Update(DataDictionary entity)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(entity.RecordType))
                {
                    entity.RecordType = RecordType.User.ToString();
                }
                entity.LastModifiedBy = GetCurrentUserName();
                entity.LastModifiedDate = DateTime.Now;
                _service.UpdateDataDictionary(entity);
                _service.Save();
                
                var window = this.GetCmp<Window>("windowDataDictionary");
                window.Hide();
                this.GetCmp<Store>("storeDataDictionary").Reload();
                return this.Direct();
            }
            
            return this.Direct();
        }

        public JsonResult GetDataDictionary(string category, string subCategory)
        {
            return Json(_service.GetDataDictionary(category, subCategory), JsonRequestBehavior.AllowGet);
        }

    }
}
