using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Business.Extension;
using DataAccess;
using Ext.Net;
using Ext.Net.MVC;
using Infrastructure;
using Service;
using ServiceStack.Common;
using PartialViewResult = Ext.Net.MVC.PartialViewResult;

namespace ZhiDiExt.Controllers
{
    [Authorize]
    public class WuYeLeiXingPeiZhiController : BaseController.BaseController
    {
        //
        // GET: /WuYeLeiXingPeiZhi/
        private WuYeLeiXingPeiZhiService _service = new WuYeLeiXingPeiZhiService(GetActiveOrgFilter());

        public ActionResult Index()
        {
            return View(GetToolarPermission());
        }

        public PartialViewResult Create()
        {
            return new PartialViewResult
            {
                ViewName = ExtHelper.Create.ToString(),// "Create",
                Model = new WuYeLeiXingPeiZhi { AreaPath = GetOrganizationAreaPath() }
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
                ViewName = ExtHelper.Edit.ToString(),
                Model = _service.GetById(id)
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

        public ActionResult Insert(WuYeLeiXingPeiZhi entity)
        {
            if (ModelState.IsValid)
            {
                if (_service.GetByEntity(entity) == null)
                {
                    entity.Convert();
                    entity.YunXuPingGu = (!string.IsNullOrEmpty(entity.YunXuPingGu)).ToString().ToLower();
                    entity.AreaPath = GetOrganizationAreaPath();
                    entity.OrgNamePath = GetOrganizationNamePath();
                    entity.CreatedBy = GetCurrentUserName();
                    entity.CreatedDate = DateTime.Now;

                    _service.Add(entity);
                    _service.Save();
                }
                else
                {
                    X.Msg.Alert(MsgInfo.TI_SHI, MsgInfo.INSERT_CHECK_EXIT).Show();
                    return this.Direct();
                }

                this.GetCmp<Window>("windowWuYeLeiXingPeiZhi").Hide();
                this.GetCmp<Store>("storeWuYeLeiXingPeiZhi").Reload();

                return this.Direct();
            }
            return this.Direct();
        }

        public ActionResult Update(WuYeLeiXingPeiZhi entity)
        {
            if (ModelState.IsValid)
            {
                entity.Convert();
                entity.YunXuPingGu = (!string.IsNullOrEmpty(entity.YunXuPingGu)).ToString().ToLower();
                entity.AreaPath = GetOrganizationAreaPath();
                entity.OrgNamePath = GetOrganizationNamePath();
                entity.LastModifiedBy = GetCurrentUserName();
                entity.LastModifiedDate = DateTime.Now;

                _service.Update(entity);
                _service.Save();

                this.GetCmp<Window>("windowWuYeLeiXingPeiZhi").Hide();
                this.GetCmp<Store>("storeWuYeLeiXingPeiZhi").Reload();
                return this.Direct();
            }

            return this.Direct();
        }

        public ActionResult Delete(int id)
        {
            if (id == -1)
            {
                return this.Direct();
            }

            _service.Delete(id);
            _service.Save();
            return View("Index", GetToolarPermission());
        }

        public ActionResult DeleteAll()
        {
            _service.DeleteAll();
            _service.Save();
            return View("Index", GetToolarPermission());
        }

        #region Overrides of BaseController

        protected override List<object> GetData()
        {
            return _service.GetAllWuYeLeiXingPeiZhi().ToObjects<object>();
        }

        public override ActionResult Refresh()
        {
            this.GetCmp<Store>("storeWuYeLeiXingPeiZhi").Reload();
            return this.Direct();
        }

        #endregion
    }
}
