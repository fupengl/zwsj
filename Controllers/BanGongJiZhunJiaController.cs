using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DataAccess;
using Ext.Net;
using Ext.Net.MVC;
using Infrastructure;
using Service;
using PartialViewResult = Ext.Net.MVC.PartialViewResult;
//using ServiceStack.Common.Extensions;

namespace ZhiDiExt.Controllers
{
    [Authorize]
    public class BanGongJiZhunJiaController : BaseController.BaseController
    {
        private readonly BanGongService _banGongService = new BanGongService(GetActiveOrgFilter());
        //
        // GET: /DataDictionary/

        public ActionResult Index()
        {
            return View(GetToolarPermission());
        }

        public PartialViewResult Create(string wuYeBianHao="")
        {
            if (string.IsNullOrEmpty(wuYeBianHao))
            {
                return new PartialViewResult
                {
                    ViewName = "Create",
                    Model = new BanGongJiZhunJia()
                };
            }
            return new PartialViewResult
            {
                ViewName = "CreateSh",
                Model = new BanGongJiZhunJia { WuYeBianHao = wuYeBianHao,WuYeMingCheng = _banGongService.GetWuYeMingCheng(wuYeBianHao)}
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
                Model = _banGongService.GetBanGongJiZhunJia(id)
            }; 
            
        }        

        public ActionResult Delete(int id)
        {
            if (id == -1)
            {                
                return this.Direct();
            }

            _banGongService.DeleteBanGongJiZhunJia(id);
            _banGongService.Save();
            return View("Index", GetToolarPermission());
        }

        public ActionResult DeleteAll()
        {
            _banGongService.DeleteAllBanGongJiZhunJia();
            _banGongService.Save();
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
            return ServiceStack.Common.EnumerableExtensions.ToObjects<object>(_banGongService.GetAllBanGongJiZhunJia());
        }   


        public override ActionResult Refresh()
        {
            this.GetCmp<Store>("storeBanGongJiZhunJia").Reload();
            return this.Direct();
        }

        public ActionResult Insert(BanGongJiZhunJia entity)
        {
            if (ModelState.IsValid)
            {
                entity.OrgNamePath = GetOrganizationNamePath();
                entity.AreaPath = GetOrganizationAreaPath();
                entity.CreatedBy = GetCurrentUserName();
                entity.CreatedDate = DateTime.Now;

                _banGongService.AddBanGongJiZhunJia(entity);
                _banGongService.Save();
                
                this.GetCmp<Window>("windowBanGongJiZhunJia").Hide();
                this.GetCmp<Store>("storeBanGongJiZhunJia").Reload();     
                
                return this.Direct();
            }
            return this.Direct();
        }

        public ActionResult Update(BanGongJiZhunJia entity)
        {
            if (ModelState.IsValid)
            {

                entity.LastModifiedBy = GetCurrentUserName();
                entity.LastModifiedDate = DateTime.Now;

                _banGongService.UpdateBanGongJiZhunJia(entity);
                _banGongService.Save();
                
                var window = this.GetCmp<Window>("windowBanGongJiZhunJia");
                window.Hide();
                this.GetCmp<Store>("storeBanGongJiZhunJia").Reload();
                return this.Direct();
            }
            
            return this.Direct();
        }

        public ActionResult UpdateWuYeMingCheng(string wuYeBianHao)
        {
            if (!string.IsNullOrEmpty(wuYeBianHao))
            {
                this.GetCmp<TextField>("WuYeMingCheng").Value = _banGongService.GetWuYeMingCheng(wuYeBianHao);
                return this.Direct();
            }

            return this.Direct(); 
        }


        public ActionResult GetByWuYeBianHao(string wuYeBianHao, StoreRequestParameters parameters = null)
        {
            var data =ServiceStack.Common.EnumerableExtensions.ToObjects<object>(
                    _banGongService.GetAllBanGongJiZhunJia().Where(x => x.WuYeBianHao == wuYeBianHao));

            if (parameters == null)
            {
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return this.Store(FilterSortAndPaging(data, parameters), data.Count());
        }
    }
}
