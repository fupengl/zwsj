using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataAccess;
using Ext.Net;
using Ext.Net.MVC;
using Infrastructure;
using Service;
using ServiceStack.Common;
using PartialViewResult = Ext.Net.MVC.PartialViewResult;

namespace ZhiDiExt.Controllers
{
    public class ZhuZhaiTaoFangZongHeXinXiController : BaseController.BaseController
    {
        private readonly ZhuZhaiService _service = new ZhuZhaiService(GetActiveOrgFilter());
        //
        // GET: /ZhuZhaiTaoFangZongHeXinXi/

        public ActionResult Index()
        {
            return View(GetToolarPermission());
        }

        public PartialViewResult Create()
        {
            return new PartialViewResult
            {
                ViewName = ExtHelper.Create.ToString(),// "Create",
                Model = new ZhuZhaiTaoFangZongHeXinXi()
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
                Model = _service.GetZhuZhaiTaoFangZongHeXinXiById(id)
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

        public ActionResult Insert(ZhuZhaiTaoFangZongHeXinXi entity)
        {
            if (ModelState.IsValid)
            {
                if (_service.GetZhuZhaiTaoFangZongHeXinXiByDanYuanBianHao(entity.DanYuanBianHao) == null)
                {
                    var louDongXiuZheng = _service.GetZhuZHaiLouDongXiuZhengByZhuangBianHao(entity.LouDongBianHao);
                    entity.LouPanBianHao = louDongXiuZheng.WuYeBianHao;
                    entity.LouPanMingCheng = louDongXiuZheng.WuYeMingCheng;
                    entity.OrgNamePath = GetOrganizationNamePath();
                    entity.CreatedBy = GetCurrentUserName();
                    entity.CreatedDate = DateTime.Now;

                    _service.AddZhuZhaiTaoFangZongHeXinXi(entity);
                    _service.Save();
                }
                else
                {
                    X.Msg.Alert(MsgInfo.TI_SHI, MsgInfo.INSERT_CHECK_EXIT).Show();
                    return this.Direct();
                }

                this.GetCmp<Window>("windowZhuZhaiTaoFangZongHeXinXi").Hide();
                this.GetCmp<Store>("storeZhuZhaiTaoFangZongHeXinXi").Reload();

                return this.Direct();
            }
            return this.Direct();
        }

        public ActionResult Update(ZhuZhaiTaoFangZongHeXinXi entity)
        {
            if (ModelState.IsValid)
            {
                //entity.ZhuZhaiLouDongXiuZheng = _service.GetZhuZHaiLouDongXiuZhengByZhuangBianHao(entity.LouDongBianHao);
                entity.OrgNamePath = GetOrganizationNamePath();
                entity.LastModifiedBy = GetCurrentUserName();
                entity.LastModifiedDate = DateTime.Now;

                _service.UpdateZhuZhaiTaoFangZongHeXinXi(entity);
                _service.Save();

                this.GetCmp<Window>("windowZhuZhaiTaoFangZongHeXinXi").Hide();
                this.GetCmp<Store>("storeZhuZhaiTaoFangZongHeXinXi").Reload();
                return this.Direct();
            }

            return this.Direct();
        }

        public ActionResult UpdateWuYeMingCheng(string louPanBianHao)
        {
            if (!string.IsNullOrEmpty(louPanBianHao))
            {
                this.GetCmp<TextField>("LouPanMingCheng").Value = _service.GetWuYeMingCheng(louPanBianHao);
                return this.Direct();
            }

            return this.Direct();
        }

        public ActionResult UpdateLouDongMingCheng(string louDongBianHao)
        {
            if (!string.IsNullOrEmpty(louDongBianHao))
            {
                this.GetCmp<TextField>("LouDongMingCheng").Value = _service.GetLouDongMingCheng(louDongBianHao);
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

            _service.DeleteZhuZhaiTaoFangZongHeXinXi(id);
            _service.Save();
            return View("Index", GetToolarPermission());
        }

        public ActionResult DeleteAll()
        {
            _service.DeleteAllZhuZhaiTaoFangZongHeXinXi();
            _service.Save();
            return View("Index", GetToolarPermission());
        }

        #region Overrides of BaseController

        protected override List<object> GetData()
        {
            return _service.GetAllZhuZhaiTaoFangZongHeXinXi().ToObjects<object>();
        }

        public override ActionResult Refresh()
        {
            this.GetCmp<Store>("storeZhuZhaiTaoFangZongHeXinXi").Reload();
            return this.Direct();
        }

        #endregion
    }
}
