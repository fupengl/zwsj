using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business;
using Ext.Net.MVC;
using PagedList;
using Service;
using Service.DTOs;
using ZhiDiExt.Models;

namespace ZhiDiExt.Controllers
{
    [Authorize]
    public class WuYeMingChengChaXunController : BaseController.BaseController
    {
        WuYeService _service=new WuYeService();
        public WuYeMingChengChaXunController()
        {
            
        }

        public ActionResult AutoComplete(int start, int limit, int page, string query, string wuYeYongTu)
        {
            var data = _service.Query(query, wuYeYongTu, GetActiveOrgFilter().NamePath).ToPagedList(page, limit);
            if (data.Count == 0)
            {
                return this.Store(GetEmptyResult(wuYeYongTu), 0);
            }
            return this.Store(data, data.Count);

        }

        private static WuYeDto GetEmptyResult(string wuYeYongTu)
        {
            return new WuYeDto
            {
                //WuYeYongTu = wuYeYongTu,
                WuYeBianHao = "没有结果",
                WuYeMingCheng = "没有结果",
                X = 0,
                Y = 0
            };
        }

        protected override List<object> GetData()
        {
            throw new NotImplementedException();
        }

        public override ActionResult Refresh()
        {
            throw new NotImplementedException();
        }
    }
}
