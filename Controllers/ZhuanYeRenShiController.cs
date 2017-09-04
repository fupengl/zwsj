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
using PartialViewResult = Ext.Net.MVC.PartialViewResult;

namespace ZhiDiExt.Controllers
{
    public class ZhuanYeRenShiController : BaseController.BaseController
    {
        private const string DefaultZhaoPianImage = "~/Images/zhuanyerenshi_photo.png";
        private readonly ZhuanYeRenShiService _zhuanYeRenShiService = new ZhuanYeRenShiService(GetActiveOrgFilter());

        //
        // GET: /DataDictionary/

        public ActionResult Index()
        {
            return View(GetToolarPermission());
            //return View();
        }

        public PartialViewResult Create()
        {
            return new PartialViewResult
            {
                ViewName = "Create",
                Model = new ZhuanYeRenShi
                {
                    ZhaoPianUrl = DefaultZhaoPianImage
                }
            };
        }

        public ActionResult Edit(int id)
        {
            if (id == -1)
            {
                X.Msg.Alert(MsgInfo.TI_SHI, MsgInfo.PLEASE_SELECT_ROW_TO_EDIT).Show();
                return this.Direct();
            }
            var model = _zhuanYeRenShiService.GetZhuanYeRenShiById(id);
            if (string.IsNullOrEmpty(model.ZhaoPianUrl))
            {
                model.ZhaoPianUrl = DefaultZhaoPianImage;
            }
            return new PartialViewResult
            {
                ViewName = "Edit",
                Model = model
            };

        }

        public ActionResult Delete(int id)
        {
            if (id == -1)
            {
                return this.Direct();
            }

            _zhuanYeRenShiService.DeleteZhuanYeRenShi(id);
            _zhuanYeRenShiService.Save();
            return View("Index");
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
            return _zhuanYeRenShiService.GetAllZhuanYeRenShi().ToObjects();// EnumerableExtensions.ToList<object>(_zhuanYeRenShiService.GetAllZhuanYeRenShi());
        }

        public override ActionResult Refresh()
        {
            this.GetCmp<Store>("storeZhuanYeRenShi").Reload();
            return this.Direct();
        }

        public ActionResult Insert(ZhuanYeRenShi entity)
        {
            if (ModelState.IsValid)
            {
                entity.CreatedBy = GetCurrentUserName();
                entity.CreatedDate = DateTime.Now;
                entity.AreaPath = GetOrganizationAreaPath();
                _zhuanYeRenShiService.AddZhuanYeRenShi(entity);
                _zhuanYeRenShiService.Save();

                this.GetCmp<Window>("windowZhuanYeRenShi").Hide();
                this.GetCmp<Store>("storeZhuanYeRenShi").Reload();

                return this.Direct();
            }
            return this.Direct();
        }

        public ActionResult Update(ZhuanYeRenShi entity)
        {
            if (ModelState.IsValid)
            {

                entity.LastModifiedBy = GetCurrentUserName();
                entity.LastModifiedDate = DateTime.Now;

                _zhuanYeRenShiService.UpdateZhuanYeRenShi(entity);
                _zhuanYeRenShiService.Save();

                var window = this.GetCmp<Window>("windowZhuanYeRenShi");
                window.Hide();
                this.GetCmp<Store>("storeZhuanYeRenShi").Reload();
                return this.Direct();
            }

            return this.Direct();
        }

        public PartialViewResult ImageUpload()
        {
            var model = new FileUploadModel
            {
                Title = "专业人士照片",
                ImportDataType = "ZhuanYeRenShi",
                AreaPath = GetOrganizationAreaPath(),
                ZhaoPianLeiXin = "ZhuanYeRenShi"
            };
            //return PartialView(model);
            return new PartialViewResult
            {
                ViewName = "ImageUpload",
                Model = model
            };
        }

        //----------------------------------自动补全--------------------------------------//
        //        public ActionResult AutoComplete(int start, int limit, int page, string query)
        //        {
        //            var data = _bll.GetAll().Where(x => x.WuYeBianHao.Contains(query)).OrderBy(x=>x.WuYeBianHao).ToPagedList(page, limit);
        //            return this.Store(data, data.Count);
        //        }
        //----------------------------------自动补全--------------------------------------//

    }
}
