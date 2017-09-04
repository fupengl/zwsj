using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using DataAccess;
using Ext.Net;
using Ext.Net.MVC;
using Infrastructure;
using EnumerableExtensions = ServiceStack.Common.Extensions.EnumerableExtensions;
using PartialViewResult = Ext.Net.MVC.PartialViewResult;
using Service;

namespace ZhiDiExt.Controllers
{
    public class GongSiXinXiController : BaseController.BaseController
    {
        //GongSiXinXiBLL _bll = new GongSiXinXiBLL();
        private readonly GongSiXinXiService _gongSiXinXiService = new GongSiXinXiService(GetActiveOrgFilter());
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
                Model = new GongSiXinXi()
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
                Model = _gongSiXinXiService.GetGongSiXinXiById(id)
            };

        }

        public ActionResult Delete(int id)
        {
            if (id == -1)
            {
                return this.Direct();
            }

            _gongSiXinXiService.DeleteGongSiXinXi(id);
            _gongSiXinXiService.Save();
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
            return EnumerableExtensions.ToList<object>(_gongSiXinXiService.GetAllGongSiXinXi());
        }

        public override ActionResult Refresh()
        {
            this.GetCmp<Store>("storeGongSiXinXi").Reload();
            return this.Direct();
        }

        private bool ValidSiteUrl(string siteUrl)
        {
            if (string.IsNullOrEmpty(siteUrl)) return true;
            return Regex.IsMatch(siteUrl, @"^(https?://)?([a-z0-9]+\.){2,}[a-z]+");
        }

        public ActionResult Insert(GongSiXinXi entity)
        {
            if (ModelState.IsValid)
            {
                if (!ValidSiteUrl(entity.SiteUrl))
                {
                    X.Msg.Alert(MsgInfo.TI_SHI, MsgInfo.SITE_URL).Show();
                    return this.Direct();
                }
                entity.CreatedBy = GetCurrentUserName();
                entity.CreatedDate = DateTime.Now;
                entity.AreaPath = GetOrganizationAreaPath();

                _gongSiXinXiService.AddGongSiXinXi(entity);
                _gongSiXinXiService.Save();

                this.GetCmp<Window>("windowGongSiXinXi").Hide();
                this.GetCmp<Store>("storeGongSiXinXi").Reload();

                return this.Direct();
            }
            return this.Direct();
        }

        public ActionResult Update(GongSiXinXi entity)
        {
            if (ModelState.IsValid)
            {
                if (!ValidSiteUrl(entity.SiteUrl))
                {
                    X.Msg.Alert(MsgInfo.TI_SHI, MsgInfo.SITE_URL).Show();
                    return this.Direct();
                }
                entity.LastModifiedBy = GetCurrentUserName();
                entity.LastModifiedDate = DateTime.Now;

                _gongSiXinXiService.UpdateGongSiXinXi(entity);
                _gongSiXinXiService.Save();

                var window = this.GetCmp<Window>("windowGongSiXinXi");
                window.Hide();
                this.GetCmp<Store>("storeGongSiXinXi").Reload();
                return this.Direct();
            }

            return this.Direct();
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
