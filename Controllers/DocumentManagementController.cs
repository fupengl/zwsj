using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DataAccess;
using Ext.Net;
using Ext.Net.MVC;
using Infrastructure;
using Service;
using ZhiDiExt.Models;
using EnumerableExtensions = ServiceStack.Common.Extensions.EnumerableExtensions;
using PartialViewResult = Ext.Net.MVC.PartialViewResult;

namespace ZhiDiExt.Controllers
{
    public class DocumentManagementController : BaseController.BaseController
    {
        readonly DocumentService _docService = new DocumentService(GetActiveOrgFilter());

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
                ViewName = "DocumentUpload",
                Model = 
                     new DocumentUploadModel
                     {
                         DocumentType = "ÖÜ¿¯",
                         ReleaseDate = DateTime.Today,
                         AreaPath = GetOrganizationAreaPath(),
                     }
            };
        }

        public ActionResult ViewComment(int id)
        {
            if (id == -1)
            {
                X.Msg.Alert(MsgInfo.TI_SHI, MsgInfo.PLEASE_SELECT_ROW_TO_EDIT).Show();
                return this.Direct();
            }
            return new PartialViewResult
            {
                ViewName = "ViewComment",
                Model = id
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
                Model = _docService.GetDocumentById(id)
            }; 
            
        }        

        public ActionResult Delete(int id)
        {
            if (id == -1)
            {                
                return this.Direct();
            }

            _docService.DeleteDocument(id);
            _docService.Save();
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
            return EnumerableExtensions.ToList<object>(_docService.GetAllDocuments());
        }   

        public override ActionResult Refresh()
        {
            this.GetCmp<Store>("storeDocumentManagement").Reload();
            return this.Direct();
        }

        public ActionResult Insert(DocumentManagement entity)
        {
            if (ModelState.IsValid)
            {
                entity.CreatedBy = GetCurrentUserName();
                entity.CreatedDate = DateTime.Now;

                _docService.AddDocument(entity);
                _docService.Save();
                
                this.GetCmp<Window>("windowDocumentManagement").Hide();
                this.GetCmp<Store>("storeDocumentManagement").Reload();     
                
                return this.Direct();
            }
            return this.Direct();
        }

        public ActionResult Update(DocumentManagement entity)
        {
            if (ModelState.IsValid)
            {

                entity.LastModifiedBy = GetCurrentUserName();
                entity.LastModifiedDate = DateTime.Now;

                _docService.UpdateDocument(entity);
                _docService.Save();
                
                var window = this.GetCmp<Window>("windowDocumentManagement");
                window.Hide();
                this.GetCmp<Store>("storeDocumentManagement").Reload();
                return this.Direct();
            }
            
            return this.Direct();
        }

        public ActionResult Download(string id)
        {
            var doc = _docService.GetDocumentById(ConvertToInt(id));
            return Download(doc.Url, doc.ContentType);
        }

    }
}
