using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business;
using DataAccess;
using Ext.Net;
using Ext.Net.MVC;
using Infrastructure;
using Service;
using PartialViewResult = Ext.Net.MVC.PartialViewResult;
using ServiceStack.Common.Extensions;

namespace ZhiDiExt.Controllers
{
    [Authorize]
    public class StatisticsController :  BaseController.BaseController
    {
        private readonly SystemLogService _systemLogService = new SystemLogService(GetActiveOrgFilter());
        public ActionResult Index()
        {
            return View(GetToolarPermission());
        }

        public ActionResult AllCity()
        {
            return View(GetToolarPermission());
        }

        public ActionResult GetAllCity(DateTime startTime, DateTime endTime)
        {
            return Json(_systemLogService.GetAllCityStatistics(startTime, endTime), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAll(DateTime startTime, DateTime endTime, string statisticsType)
        {
            //var data = GetData();
            switch (statisticsType)
            {
                case "Month":
                    return Json(_systemLogService.GetMonthStatistics(startTime, endTime), JsonRequestBehavior.AllowGet);
                case "Year":
                    return Json(_systemLogService.GetYearStatistics(startTime, endTime), JsonRequestBehavior.AllowGet);
                default:
                    return null;
            }
            //var data = _systemLogService.GetYearStatistics(startTime, endTime);
            //return Json(data, JsonRequestBehavior.AllowGet);
            //if (parameters == null)
            //{
            //    return Json(data, JsonRequestBehavior.AllowGet);
            //}
            //return this.Store()
        }


        public override ActionResult Refresh()
        {
            throw new NotImplementedException();
        }

        protected override List<object> GetData()
        {
            throw new NotImplementedException();
        }   
    }
}
