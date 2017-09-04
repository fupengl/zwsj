using System;
using System.Collections.Generic;
using System.Web.Helpers;
using System.Web.Mvc;
using DataAccess;
using Ext.Net;
using Ext.Net.MVC;
using Service;
using ZhiDiExt.Models;
using EnumerableExtensions = ServiceStack.Common.Extensions.EnumerableExtensions;
using PartialViewResult = Ext.Net.MVC.PartialViewResult;

namespace ZhiDiExt.Controllers
{
    public class WuYeZhaoPianController : BaseController.BaseController
    {
        readonly DataDictionaryService _dataDictionaryService=new DataDictionaryService();
        readonly WuYeService _wuYeService=new WuYeService();
        
        //
        // GET: /DataDictionary/

        public ActionResult Index(string wuYeYongTu)
        {
            ViewBag.WuYeYongTu = wuYeYongTu;
            ViewBag.WuYeYongTuChn = GetWuYeYongTuChn(wuYeYongTu);
            return View(GetToolarPermission());
        }

        public PartialViewResult Create(string wuYeYongTu, string wuYeBianHao, string zhaoPianLeiXin)
        {
            var wuYe = _wuYeService.GetWuYe(GetWuYeYongTuChn(wuYeYongTu), wuYeBianHao);
            
            return new PartialViewResult
            {
                ViewName = "ImageUpload",
                Model =
                    new FileUploadModel
                    {
                        Title = "物业照片",
                        ImportDataType = "WuYeZhaoPian",
                        AreaPath = wuYe.AreaPath,
                        WuYeYongTu = GetWuYeYongTuChn(wuYeYongTu),
                        WuYeBianHao = wuYeBianHao,
                        WuYeMingCheng = wuYe.WuYeMingCheng,
                        ZhaoPianLeiXin = zhaoPianLeiXin
                    }
            };
        }

        public PartialViewResult CreateMulti(string wuYeYongTu, string wuYeBianHao, string zhaoPianLeiXin)
        {
            var wuYe = _wuYeService.GetWuYe(GetWuYeYongTuChn(wuYeYongTu), wuYeBianHao);

            return new PartialViewResult
            {
                ViewName = "ImageMultiUpload",
                Model =
                    new FileUploadModel
                    {
                        Title = "物业照片",
                        ImportDataType = "WuYeZhaoPian",
                        AreaPath = wuYe.AreaPath,
                        WuYeYongTu = GetWuYeYongTuChn(wuYeYongTu),
                        WuYeBianHao = wuYeBianHao,
                        WuYeMingCheng = wuYe.WuYeMingCheng,
                        ZhaoPianLeiXin = zhaoPianLeiXin
                    }
            };
        }

        //public ActionResult Edit(int id)
        //{
        //    if (id == -1)
        //    {
        //        X.Msg.Alert(MsgInfo.TI_SHI, MsgInfo.PLEASE_SELECT_ROW_TO_EDIT).Show();
        //        return this.Direct();
        //    }
        //    return new PartialViewResult
        //    {
        //        ViewName = "Edit",
        //        Model = _bll.GetWuYeZhaoPianById(id)
        //    }; 
            
        //}        

        public ActionResult Delete(int id)
        {
            if (id == -1)
            {                
                return this.Direct();
            }

            _wuYeService.DeleteWuYeZhaoPian(id);
            _wuYeService.Save();
            return View("Index", GetToolarPermission());
        }
        
        
        //public ActionResult GetAll(string wuYeYongTu,StoreRequestParameters parameters = null)
        //{
        //    var data = EnumerableExtensions.ToList<object>(_wuYeService.GetAllWuYeZhaoPian(GetWuYeYongTuChn(wuYeYongTu)));

        //    if (parameters == null)
        //    {
        //        return Json(data, JsonRequestBehavior.AllowGet);
        //    }
        //    return this.Store(FilterSortAndPaging(data, parameters), data.Count());
        //}

        protected override List<object> GetData()
        {
            //return EnumerableExtensions.ToList<object>(_bll.GetAll());
            throw new NotImplementedException();
        }   

        public override ActionResult Refresh()
        {
            this.GetCmp<Store>("storeWuYeZhaoPian").Reload();
            return this.Direct();
        }

        public ActionResult Insert(WuYeZhaoPian entity)
        {
            if (ModelState.IsValid)
            {
                entity.OrgNamePath = GetOrganizationNamePath();
                entity.CreatedBy = GetCurrentUserName();
                entity.CreatedDate = DateTime.Now;

                _wuYeService.AddWuYeZhaoPian(entity);
                _wuYeService.Save();
                
                this.GetCmp<Window>("windowWuYeZhaoPian").Hide();
                this.GetCmp<Store>("storeWuYeZhaoPian").Reload();     
                
                return this.Direct();
            }
            return this.Direct();
        }

        public ActionResult Update(WuYeZhaoPian entity)
        {
            if (ModelState.IsValid)
            {

                entity.LastModifiedBy = GetCurrentUserName();
                entity.LastModifiedDate = DateTime.Now;

                _wuYeService.UpdateWuYeZhaoPian(entity);
                _wuYeService.Save();
                
                var window = this.GetCmp<Window>("windowWuYeZhaoPian");
                window.Hide();
                this.GetCmp<Store>("storeWuYeZhaoPian").Reload();
                return this.Direct();
            }
            
            return this.Direct();
        }

        public ActionResult GetDataDictionary(string wuYeYongTu)
        {
            var datas = new List<DataDictionary> {new DataDictionary {KeyValue = "全部"}};
            datas.AddRange(_dataDictionaryService.GetZhaoPianLeiXin(wuYeYongTu));
            return this.Store(datas);
            //return this.Store(dataDictionaryService.GetZhaoPianLeiXin(wuYeYongTu));
        }

        public ActionResult UpdateWuYeMingCheng(string wuYeYongTu,string wuYeBianHao)
        {
            if (!string.IsNullOrEmpty(wuYeBianHao))
            {
                var wuye = _wuYeService.GetWuYe(wuYeYongTu, wuYeBianHao);
                this.GetCmp<TextField>("WuYeMingCheng").Value = wuye.WuYeMingCheng;
                this.GetCmp<TextField>("AreaPath").Value = wuye.AreaPath;
                return this.Direct();
            }

            return this.Direct();
        }

        public ActionResult CloseUpload()
        {
            this.GetCmp<Window>("windowWuYeZhaoPian").Hide();
            this.GetCmp<Store>("storeWuYeZhaoPian").Reload();

            return this.Direct();
        }

        public ActionResult GetAllByWuYeBianHao(string wuYeYongTu, string wuYeBianHao, string zhaoPianLeiXin)
        {
            return this.Store(_wuYeService.GetWuYeZhaoPian(wuYeYongTu, wuYeBianHao, zhaoPianLeiXin));
        }

        public ActionResult ImageThumbs(string id, int width, int height)
        {
            
            string fileName = _wuYeService.GetWuYeZhaoPianById(ConvertToInt(id)).FilePath;
            WebImage img=new WebImage(fileName).Resize(width, height);

            return File(img.GetBytes("image/jpeg"), "image/jpeg");
        }
    }
}
