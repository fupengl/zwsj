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
using ServiceStack.Common;//.Extensions;

namespace ZhiDiExt.Controllers
{
    public class XiuZhengTiXiController : BaseController.BaseController
    {
        private readonly XiuZhengTiXiService _service = new XiuZhengTiXiService(GetActiveOrgFilter());

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
                ViewName = ExtHelper.Create.ToString(),
                Model = new XiuZhengTiXi()
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
                Model = _service.GetXiuZhengTiXiById(id)
            }; 
            
        }        

        public ActionResult Delete(int id)
        {
            if (id == -1)
            {                
                return this.Direct();
            }

            _service.DeleteXiuZhengTiXi(id);
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
            return _service.GetAllXiuZhengTiXi().ToObjects<object>();
        }   

        public override ActionResult Refresh()
        {
            this.GetCmp<Store>(ExtHelper.storeXiuZhengTiXi.ToString()).Reload();
            return this.Direct();
        }

        public ActionResult Insert(XiuZhengTiXi entity)
        {
            if (ModelState.IsValid)
            {
                entity.CreatedBy = GetCurrentUserName();
                entity.CreatedDate = DateTime.Now;
                entity.AreaPath = GetOrganizationAreaPath();

                _service.AddXiuZhengTiXi(entity);
                _service.Save();
                
                this.GetCmp<Window>(ExtHelper.windowXiuZhengTiXi.ToString()).Hide();
                this.GetCmp<Store>(ExtHelper.storeXiuZhengTiXi.ToString()).Reload();     
                
                return this.Direct();
            }
            return this.Direct();
        }

        public ActionResult Update(XiuZhengTiXi entity)
        {
            if (ModelState.IsValid)
            {

                entity.LastModifiedBy = GetCurrentUserName();
                entity.LastModifiedDate = DateTime.Now;

                _service.UpdateXiuZhengTiXi(entity);
                _service.Save();
                
                var window = this.GetCmp<Window>(ExtHelper.windowXiuZhengTiXi.ToString());
                window.Hide();
                this.GetCmp<Store>(ExtHelper.storeXiuZhengTiXi.ToString()).Reload();
                return this.Direct();
            }
            
            return this.Direct();
        }

        public JsonResult GetAllByYongTuAndLeiXing(string yongTu, string leiBie)
        {
            this.GetCmp<Store>(ExtHelper.storeXiuZhengTiXi.ToString()).RemoveAll();
            this.GetCmp<ComboBox>("DaiMa").SetValue("");
            this.GetCmp<ComboBox>("DaiMa").SetRawValue("");
            //return this.Store(_service.GetAllByYongTuAndLeiXing(yongTu, leiBie));
            return Json(_service.GetAllByYongTuAndLeiXing(yongTu, leiBie), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllYinSu()
        {
            //this.GetCmp<Store>("storeXiuZhengTiXi").RemoveAll();
            //this.GetCmp<ComboBox>("DaiMa").SetValue("");
            //this.GetCmp<ComboBox>("DaiMa").SetRawValue("");
            //return this.Store(_service.GetAllByYongTuAndLeiXing(yongTu, leiBie));
            return Json(_service.GetAllXiuZhengXiShu(), JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetDataDictionary(string category, string subCategory)
        //{
        //    return Json(_service.GetDataDictionary(category, subCategory), JsonRequestBehavior.AllowGet);
        //}

//----------------------------------�Զ���ȫ--------------------------------------//
//        public ActionResult AutoComplete(int start, int limit, int page, string query)
//        {
//            var data = _bll.GetAll().Where(x => x.WuYeBianHao.Contains(query)).OrderBy(x=>x.WuYeBianHao).ToPagedList(page, limit);
//            return this.Store(data, data.Count);
//        }
//----------------------------------�Զ���ȫ--------------------------------------//

    }
}
