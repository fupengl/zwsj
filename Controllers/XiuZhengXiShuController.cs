using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DataAccess;
using Ext.Net;
using Ext.Net.MVC;
using Infrastructure;
//using EnumerableExtensions = ServiceStack.Common.Extensions.EnumerableExtensions;
using ServiceStack.Common;
using PartialViewResult = Ext.Net.MVC.PartialViewResult;
using Service;

namespace ZhiDiExt.Controllers
{
    public class XiuZhengXiShuController : BaseController.BaseController
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
                Model = new XiuZhengXiShu()
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
                Model = _service.GetXiuZhengXiShuById(id)
            }; 
            
        }        

        public ActionResult Delete(int id)
        {
            if (id == -1)
            {                
                return this.Direct();
            }

            _service.DeleteXiuZhengXiShu(id);
            _service.Save();
            return View("Index", GetToolarPermission());
        }

        public ActionResult DeleteAll()
        {
            _service.DeleteAllXiuZhengXiShu();
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
            return _service.GetAllXiuZhengXiShu().ToObjects<object>();
        }

        public override ActionResult Refresh()
        {
            this.GetCmp<Store>(ExtHelper.storeXiuZhengXiShu.ToString()).Reload();
            return this.Direct();
        }

        public ActionResult Insert(XiuZhengXiShu entity)
        {
            if (ModelState.IsValid)
            {
                entity.CreatedBy = GetCurrentUserName();
                entity.CreatedDate = DateTime.Now;

                _service.AddXiuZhengXiShu(entity);
                _service.Save();
                
                this.GetCmp<Window>(ExtHelper.windowXiuZhengXiShu.ToString()).Hide();
                this.GetCmp<Store>(ExtHelper.storeXiuZhengXiShu.ToString()).Reload();     
                
                return this.Direct();
            }
            return this.Direct();
        }

        public ActionResult Update(XiuZhengXiShu entity)
        {
            if (ModelState.IsValid)
            {

                entity.LastModifiedBy = GetCurrentUserName();
                entity.LastModifiedDate = DateTime.Now;

                _service.UpdateXiuZhengXiShu(entity);
                _service.Save();
                
                var window = this.GetCmp<Window>(ExtHelper.windowXiuZhengXiShu.ToString());
                window.Hide();
                this.GetCmp<Store>(ExtHelper.storeXiuZhengXiShu.ToString()).Reload();
                return this.Direct();
            }
            
            return this.Direct();
        }

        public ActionResult UpdateYinSuAndMiaoShu(string daiMa)
        {
            if (!string.IsNullOrEmpty(daiMa))
            {
                this.GetCmp<TextField>("YinSu").Value = _service.GetXiuZhengXiShuByDaiMa(daiMa).YinSu;
                this.GetCmp<TextField>("MiaoShu").Value = _service.GetXiuZhengXiShuByDaiMa(daiMa).MianShu;
                return this.Direct();
            }

            return this.Direct();
        }

//----------------------------------�Զ���ȫ--------------------------------------//
//        public ActionResult AutoComplete(int start, int limit, int page, string query)
//        {
//            var data = _bll.GetAll().Where(x => x.WuYeBianHao.Contains(query)).OrderBy(x=>x.WuYeBianHao).ToPagedList(page, limit);
//            return this.Store(data, data.Count);
//        }
//----------------------------------�Զ���ȫ--------------------------------------//

    }
}
