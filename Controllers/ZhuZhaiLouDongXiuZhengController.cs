using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Business.Extension;
using DataAccess;
using Ext.Net;
using Ext.Net.MVC;
using Infrastructure;
using PagedList;
using Service;
using Service.DTOs;
using PartialViewResult = Ext.Net.MVC.PartialViewResult;
//using ServiceStack.Common.Extensions;
using ServiceStack.Common;

namespace ZhiDiExt.Controllers
{
    [Authorize]
    public class ZhuZhaiLouDongXiuZhengController : BaseController.BaseController
    {
        private readonly ZhuZhaiService _service = new ZhuZhaiService(GetActiveOrgFilter());
        //
        // GET: /DataDictionary/

        public ActionResult Index()
        {
            return View(GetToolarPermission());
        }

        public PartialViewResult Create(string wuYeBianHao = "")
        {
            if (string.IsNullOrEmpty(wuYeBianHao))
            {
                return new PartialViewResult
                {
                    ViewName = ExtHelper.Create.ToString(),
                    Model = new ZhuZhaiLouDongXiuZheng()
                };
            }
            return new PartialViewResult
            {
                ViewName = ExtHelper.CreateSh.ToString(),//"CreateSh",
                Model = new ZhuZhaiLouDongXiuZheng { WuYeBianHao = wuYeBianHao, WuYeMingCheng = _service.GetWuYeMingCheng(wuYeBianHao) }
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
                Model = _service.GetZhuZhaiLouDongXiuZhengById(id)
            };

        }

        public ActionResult Delete(int id)
        {
            if (id == -1)
            {
                return this.Direct();
            }

            _service.DeleteZhuZhaiLouDongXiuZheng(id);
            _service.Save();
            return View("Index", GetToolarPermission());
        }

        public ActionResult DeleteAll()
        {
            _service.DeleteAllZhuZhaiLouDongXiuZheng();
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
            return _service.GetAllZhuZhaiLouDongXiuZheng().ToObjects<object>();
        }


        public override ActionResult Refresh()
        {
            this.GetCmp<Store>(ExtHelper.storeZhuZhaiLouDongXiuZheng.ToString()).Reload();
            return this.Direct();
        }

        public ActionResult Insert(ZhuZhaiLouDongXiuZheng entity)
        {
            if (ModelState.IsValid)
            {
                if (_service.GetZhuZhaiLouDongXiuZhengByZhuangHao(entity.WuYeBianHao, entity.WuYeLeiXing, entity.LouDong) == null)
                {
                    entity.Convert();
                    entity.OrgNamePath = GetOrganizationNamePath();
                    entity.CreatedBy = GetCurrentUserName();
                    entity.CreatedDate = DateTime.Now;

                    _service.AddZhuZhaiLouDongXiuZheng(entity);
                    _service.Save();
                }
                else
                {
                    X.Msg.Alert(MsgInfo.TI_SHI, MsgInfo.INSERT_CHECK_EXIT_LOUDONG).Show();
                    return this.Direct();
                }

                this.GetCmp<Window>(ExtHelper.windowZhuZhaiLouDongXiuZheng.ToString()).Hide();
                this.GetCmp<Store>(ExtHelper.storeZhuZhaiLouDongXiuZheng.ToString()).Reload();

                return this.Direct();
            }
            return this.Direct();
        }

        public ActionResult Update(ZhuZhaiLouDongXiuZheng entity)
        {
            if (ModelState.IsValid)
            {
                entity.Convert();
                entity.LastModifiedBy = GetCurrentUserName();
                entity.LastModifiedDate = DateTime.Now;

                _service.UpdateZhuZhaiLouDongXiuZheng(entity);
                _service.Save();

                var window = this.GetCmp<Window>(ExtHelper.windowZhuZhaiLouDongXiuZheng.ToString());
                window.Hide();
                this.GetCmp<Store>(ExtHelper.storeZhuZhaiLouDongXiuZheng.ToString()).Reload();
                return this.Direct();
            }

            return this.Direct();
        }

        public ActionResult UpdateWuYeMingCheng(string wuYeBianHao)
        {
            if (!string.IsNullOrEmpty(wuYeBianHao))
            {
                this.GetCmp<TextField>("WuYeMingCheng").Value = _service.GetWuYeMingCheng(wuYeBianHao);
                return this.Direct();
            }

            return this.Direct();
        }

        public ActionResult GetByWuYeBianHao(string wuYeBianHao, StoreRequestParameters parameters = null)
        {
            var data =
                _service.GetAllZhuZhaiLouDongXiuZheng().Where(x => x.WuYeBianHao == wuYeBianHao).ToObjects<object>();

            if (parameters == null)
            {
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return this.Store(FilterSortAndPaging(data, parameters), data.Count());
        }

        public ActionResult GetLouDongByWuYeBianHao(string wuYeBianHao, string diZhi)
        {
            var data =
                _service.GetAllZhuZhaiLouDongXiuZheng().Where(x => x.WuYeBianHao == wuYeBianHao && x.Address == diZhi).Select(x => new LouDongDto { Id = x.Id, LouDong = x.LouDong }).ToList();


            return this.Store(data);
        }

        public ActionResult GetDiZhiByWuYeBianHao(string wuYeBianHao)
        {
            var data = _service.GetDiZhiByWuYeBianHao(wuYeBianHao).ToList();

            return this.Store(data);
        }

        public ActionResult AutoComplete(int start, int limit, int page, string query, string wuYeBianHao = "")
        {
            var data = _service.QueryLouDongMingCheng(query, wuYeBianHao, GetActiveOrgFilter().NamePath).ToPagedList(page, limit);
            if (data.Count == 0)
            {
                return this.Store(GetEmptyResult(), 0);
            }
            return this.Store(data, data.Count);

        }

        private static LouDongMingChengDto GetEmptyResult()
        {
            return new LouDongMingChengDto
            {
                WuYeMingCheng = "没有结果",
                LouDongMingCheng = "没有结果",
                LouDongBianHao = "没有结果",
                Address = "没有结果"
            };
        }
    }
}
