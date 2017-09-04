using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Ext.Net;
using Ext.Net.MVC;
using Service;
using ZhiDiExt.Models;
using PartialViewResult = Ext.Net.MVC.PartialViewResult;

namespace ZhiDiExt.Controllers
{
    public class GongYePianQuZhaoPianController : BaseController.BaseController
    {
        private readonly GongYePianQuService _gongYePianQuService = new GongYePianQuService(); 
        //
        // GET: /GongYePianQuZhaoPian/

        public ActionResult Index()
        {
            return View(GetToolarPermission());
        }

        public ActionResult GetGongYePianQuZhaoPian(int gongYePianQuId)
        {
            return this.Store(_gongYePianQuService.GetGongYePianQuZhaoPianByGongYePianQuId(gongYePianQuId));
        }

        public override ActionResult Refresh()
        {
            this.GetCmp<Store>("storeGongYePianQuZhaoPian").Reload();
            return this.Direct();
        }

        protected override List<object> GetData()
        {
            throw new NotImplementedException();
        }

        public PartialViewResult Create(string gongYePianQuId)
        {
            return new PartialViewResult
            {
                ViewName = "ImageUpload",
                Model =
                    new FileUploadModel
                    {
                        Title = "工业片区照片",
                        ImportDataType = "GongYePianQuZhaoPian",
                        AreaPath = GetOrganizationAreaPath(),
                        ZhaoPianLeiXin = gongYePianQuId
                    }
            };
        }

        public ActionResult Delete(int id)
        {
            if (id == -1)
            {
                return this.Direct();
            }

            _gongYePianQuService.DeleteGongYePianQuZhaoPian(id);
            _gongYePianQuService.Save();
            return View("Index", GetToolarPermission());
        }

        public ActionResult GongYePianQuImageThumbs(string id, int width, int height)
        {

            string fileName = _gongYePianQuService.GetGongYePianQuZhaoPianById(ConvertToInt(id)).FilePath;
            WebImage img = new WebImage(fileName).Resize(width, height);

            return File(img.GetBytes("image/jpeg"), "image/jpeg");
        }

    }
}
