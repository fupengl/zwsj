using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DataAccess;
using Ext.Net;
using Ext.Net.MVC;
using Infrastructure;
using PagedList;
using Service;
using PartialViewResult = Ext.Net.MVC.PartialViewResult;
//using ServiceStack.Common.Extensions;
using ServiceStack.Common;

namespace ZhiDiExt.Controllers
{
    [Authorize]
    public class ZhuZhaiJiChuXinXiController : BaseController.BaseController
    {
       private readonly ZhuZhaiService _service=new ZhuZhaiService(GetActiveOrgFilter());

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
                ViewName = ExtHelper.Create.ToString(),// "Create",
                Model = new ZhuZhaiJiChuXinXi()
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
                Model = _service.GetZhuZhaiJiChuXinXiById(id)
            };

        }

        public ActionResult Delete(int id)
        {
            if (id == -1)
            {
                return this.Direct();
            }

            _service.DeleteZhuZhaiJiChuXinXi(id);
            _service.Save();
            return View("Index", GetToolarPermission());
        }

        public ActionResult DeleteAll()
        {
            _service.DeleteAllZhuZhaiChuXinXi();
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
            return _service.GetAllZhuZhaiJiChuXinXi().ToObjects<object>();
        }   

        public override ActionResult Refresh()
        {
            this.GetCmp<Store>(ExtHelper.storeZhuZhaiJiChuXinXi.ToString()).Reload();
            return this.Direct();
        }

        public ZhuZhaiJiChuXinXi CheckExits(string wuYeBianHao)
        {
            return _service.GetZhuZhaiJiChunXinXiByWuYeBianHao(wuYeBianHao);
        }

        public ActionResult Insert(ZhuZhaiJiChuXinXi entity)
        {
            if (ModelState.IsValid)
            {
                if (_service.GetZhuZhaiJiChunXinXiByWuYeBianHao(entity.WuYeBianHao) == null)
                {
                    entity.YunXuPingGu = GetCheckBoxValue(entity.YunXuPingGu);
                    entity.KeJian = GetCheckBoxValue(entity.KeJian);
                    entity.OrgNamePath = GetOrganizationNamePath();
                    entity.CreatedBy = GetCurrentUserName();
                    entity.CreatedDate = DateTime.Now;

                    _service.AddZhuZhaiJiChuXinXi(entity);
                    _service.Save();

                }
                else
                {
                    X.Msg.Alert(MsgInfo.TI_SHI, MsgInfo.INSERT_CHECK_EXIT).Show();
                    return this.Direct();
                }

                this.GetCmp<Window>(ExtHelper.windowZhuZhaiJiChuXinXi.ToString()).Hide();
                this.GetCmp<Store>(ExtHelper.storeZhuZhaiJiChuXinXi.ToString()).Reload();

                return this.Direct();
            }
            return this.Direct();
        }

        public ActionResult Update(ZhuZhaiJiChuXinXi entity)
        {
            if (ModelState.IsValid)
            {
                entity.YunXuPingGu = GetCheckBoxValue(entity.YunXuPingGu);
                entity.KeJian = GetCheckBoxValue(entity.KeJian);
                entity.OrgNamePath = GetOrganizationNamePath();
                entity.LastModifiedBy = GetCurrentUserName();
                entity.LastModifiedDate = DateTime.Now;

                _service.UpdateZhuZhaiJiChuXinXi(entity);
                _service.Save();

                var window = this.GetCmp<Window>(ExtHelper.windowZhuZhaiJiChuXinXi.ToString());
                window.Hide();
                this.GetCmp<Store>(ExtHelper.storeZhuZhaiJiChuXinXi.ToString()).Reload();
                return this.Direct();
            }

            return this.Direct();
        }

        public ActionResult AutoComplete(int start, int limit, int page, string query)
        {            
            var data =
                _service.GetAllZhuZhaiJiChuXinXi()
                    .Where(x => x.WuYeBianHao.Contains(query))
                    .OrderBy(x => x.WuYeBianHao)
                    .ToPagedList(page, limit);
            return this.Store(data, data.Count);
        }        
    }
}
