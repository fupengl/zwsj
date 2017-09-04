using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Infrastructure;
using Service;
using ServiceStack.Common.Extensions;
using PartialViewResult = Ext.Net.MVC.PartialViewResult;

namespace ZhiDiExt.Controllers
{
    [Authorize]
    public class WeiTuoPingGuController : BaseController.BaseController
    {
        private readonly WeiTuoPingGuService _weiTuoPingGuService = new WeiTuoPingGuService(GetActiveOrgFilter());

        public ActionResult Index()
        {
            return View(GetToolarPermission());
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
                ViewName = ExtHelper.Edit.ToString(),
                Model = _weiTuoPingGuService.GetById(id)
            };
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

        #region Overrides of BaseController

        protected override List<object> GetData()
        {
            return _weiTuoPingGuService.GetAllWeiTuoPingGu().ToObjects();
        }

        public override ActionResult Refresh()
        {
            this.GetCmp<Store>("storeWeiTuoPingGu").Reload();
            return this.Direct();
        }

        #endregion
    }
}