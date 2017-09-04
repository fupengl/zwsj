using System;
using System.Collections.Generic;
using System.Web.Helpers;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Service;
using ZhiDiExt.Models;
using PartialViewResult = Ext.Net.MVC.PartialViewResult;

namespace ZhiDiExt.Controllers
{
    public class XiTongZhaoPianController : BaseController.BaseController
    {
        //
        // GET: /XiTongZhaoPian/
        private readonly DataDictionaryService _dataDictionaryService = new DataDictionaryService();
        private readonly XiTongZhaoPianService _xiTongZhaoPianService = new XiTongZhaoPianService();

        public ActionResult Index()
        {

            return View(GetToolarPermission());
        }

        public override ActionResult Refresh()
        {
            this.GetCmp<Store>("storeXiTongZhaoPian").Reload();
            return this.Direct();
        }

        public ActionResult GetAllByAreaPath(string zhaoPianLeiXing)
        {
            return this.Store(_xiTongZhaoPianService.GetAllXiTongZhaoPianByAreaPath(GetOrganizationAreaPath(), zhaoPianLeiXing));
        }

        protected override List<object> GetData()
        {
            throw new NotImplementedException();
        }

        public PartialViewResult Create(string zhaoPianLeiXin)
        {
            return new PartialViewResult
            {
                ViewName = "ImageUpload",
                Model =
                    new FileUploadModel
                    {
                        Title = "系统照片",
                        ImportDataType = "XiTongZhaoPian",
                        AreaPath = GetOrganizationAreaPath(),
                        ZhaoPianLeiXin = zhaoPianLeiXin
                    }
            };
        }

        public ActionResult Delete(int id)
        {
            if (id == -1)
            {
                return this.Direct();
            }

            _xiTongZhaoPianService.DeleteXiTongZhaoPian(id);
            _xiTongZhaoPianService.Save();
            return View("Index", GetToolarPermission());
        }

        public ActionResult GetDataDictionary(string wuYeYongTu)
        {
            return this.Store(_dataDictionaryService.GetZhaoPianLeiXin(wuYeYongTu));
        }

        public ActionResult XiTongImageThumbs(string id, int width, int height)
        {

            string fileName = _xiTongZhaoPianService.GetXiTongZhaoPianById(ConvertToInt(id)).FilePath;
            WebImage img = new WebImage(fileName).Resize(width, height);

            return File(img.GetBytes("image/jpeg"), "image/jpeg");
        }

    }
}
