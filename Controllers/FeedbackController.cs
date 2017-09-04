using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Infrastructure;
using Service;
using PartialViewResult = Ext.Net.MVC.PartialViewResult;
using ServiceStack.Common;
using ZhiDiExt.Mobile.Model;

namespace ZhiDiExt.Controllers
{
    public class FeedbackController : BaseController.BaseController
    {
        private readonly FeedbackService _service = new FeedbackService();

        //
        // GET: /DataDictionary/

        public ActionResult Index()
        {
            return View(GetToolarPermission());
        }

        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                X.Msg.Alert(MsgInfo.TI_SHI, MsgInfo.PLEASE_SELECT_ROW_TO_EDIT).Show();
                return this.Direct();
            }
            var entity = _service.GetFeedbackById(id);
            var viewName = ExtHelper.Edit.ToString();
            if (entity.Type == MsgInfo.LiuYanGuJiaShi)
            {
                var model = Newtonsoft.Json.JsonConvert.DeserializeObject<LiuYanGuJiaShi>(entity.FeedbackContent);
                model.CreateDate = entity.CreateDate;
                model.Areapath = entity.AreaPath;
                model.Account = entity.Account;
                model.Type = entity.Type;
                return new PartialViewResult
                {
                    ViewName = viewName + "LiuYan",
                    Model = model
                };
            }
            return new PartialViewResult
            {
                ViewName = viewName,
                Model = entity
            };
        }

        public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return this.Direct();
            }

            _service.DeleteFeedback(id);
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
            var filter = GetActiveOrgFilter();
            return _service.GetAreaFeedback(filter).ToObjects<object>();
        }

        public override ActionResult Refresh()
        {
            this.GetCmp<Store>(ExtHelper.storeFeedback.ToString()).Reload();
            return this.Direct();
        }
    }
}
