using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DataAccess;
using Ext.Net;
using Ext.Net.MVC;
using Infrastructure;
using Service;
using ServiceStack.Common;
using ZhiDiExt.Models;
using EnumerableExtensions = ServiceStack.Common.Extensions.EnumerableExtensions;
using PartialViewResult = Ext.Net.MVC.PartialViewResult;

namespace ZhiDiExt.Controllers
{
    public class DocumentCommentController : BaseController.BaseController
    {
        readonly DocumentCommentService _docService = new DocumentCommentService();

        public ActionResult GetAll(int id, StoreRequestParameters parameters = null)
        {
            var data = _docService.GetByDocumentId(id, 0, int.MaxValue).ToObjects();

            if (parameters == null)
            {
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return this.Store(FilterSortAndPaging(data, parameters), data.Count());
        }

        protected override List<object> GetData()
        {
            return _docService.GetByDocumentId(0, 0, 0).ToObjects();
        }

        public override ActionResult Refresh()
        {
            this.GetCmp<Store>("storeDocumentComment").Reload();
            return this.Direct();
        }

    }
}
