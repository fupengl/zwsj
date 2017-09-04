using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Service;
using DataAccess;
using Ext.Net;
using Ext.Net.MVC;
using Infrastructure;
//using EnumerableExtensions = ServiceStack.Common.Extensions.EnumerableExtensions;
using ServiceStack.Common;
using PartialViewResult = Ext.Net.MVC.PartialViewResult;

namespace ZhiDiExt.Controllers
{
    public class ExtrenalUserController : BaseController.BaseController
    {
        private readonly ExtrenalUserService _extrenalUserService = new ExtrenalUserService(GetActiveOrgFilter());
        private readonly XinZhengQuYuService _xinZhengQuYuService = new XinZhengQuYuService();
        
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
                ViewName = "Create",
                Model = new ExtrenalUser ()
            };
        }

        public ActionResult Count()
        {
            var count = _extrenalUserService.GetAllExtrenalUser().Count(x => !string.IsNullOrEmpty(x.DeviceId));
            X.Msg.Alert(MsgInfo.TI_SHI, "已安装客户" + count + "个。").Show();
            return this.Direct();
        }

        public ActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                X.Msg.Alert(MsgInfo.TI_SHI, MsgInfo.PLEASE_SELECT_ROW_TO_EDIT).Show();
                return this.Direct();
            }
            return new PartialViewResult
            {
                ViewName = "Edit",
                Model = _extrenalUserService.GetById(id)
            }; 
            
        }        

        public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return this.Direct();
            }

            _extrenalUserService.DeleteExtrenalUser(id);
            _extrenalUserService.Save();
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
            return _extrenalUserService.GetAllExtrenalUser().ToObjects<object>();
        }

        public override ActionResult Refresh()
        {
            this.GetCmp<Store>("storeExtrenalUser").Reload();
            return this.Direct();
        }

        public ActionResult Insert(ExtrenalUser entity, int count)
        {

            if (ModelState.IsValid)
            {
                string quHao = _xinZhengQuYuService.GetByAreaName(GetOrganizationAreaPath()).QuHao;
                string city = GetOrganizationAreaPath();//.Substring(GetOrganizationAreaPath().LastIndexOf("/"); 
                var userNames = GetAllUserName();
                for (var i = 0; i < count; i++)
                {
                    entity.CreatedBy = GetCurrentUserName();
                    entity.CreatedDate = DateTime.Now;
                    string xuLieHao = GetRandomNumber(quHao, 5, userNames);
                    entity.Account = xuLieHao;
                    entity.Name = xuLieHao;
                    entity.City = city;
                    entity.Password = entity.Password ?? "123";
                    entity.DeviceId = "";
                    _extrenalUserService.AddExtrenalUser(entity);
                    _extrenalUserService.Save();
                }
                
                this.GetCmp<Window>("windowExtrenalUser").Hide();
                this.GetCmp<Store>("storeExtrenalUser").Reload();     
                
                return this.Direct();
            }
            return this.Direct();
        }


        public ActionResult Update(ExtrenalUser entity)
        {
            if (ModelState.IsValid)
            {

                entity.LastModifiedBy = GetCurrentUserName();
                //entity.LastModifiedDate = DateTime.Now;
                //entity.Account = entity.Name;
                _extrenalUserService.UpdateExtrenalUser(entity);
                _extrenalUserService.Save();
                
                var window = this.GetCmp<Window>("windowExtrenalUser");
                window.Hide();
                this.GetCmp<Store>("storeExtrenalUser").Reload();
                return this.Direct();
            }
            
            return this.Direct();
        }

        /// <summary>
        /// 查找数据库中是否已存在该用户名
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="extrenalUser">数据库中的用户列表</param>
        /// <returns>如果用户名在数据库中找到返回true,否则返回false</returns>
        private bool CheckUserNameExits(string userName, IEnumerable<string> extrenalUser)
        {
            return extrenalUser.Any(x => x == userName);
        }

        /// <summary>
        /// 自动生成用户名
        /// </summary>
        /// <param name="quHao">城市的区号</param>
        /// <param name="numberLength">随机数的长度</param>
        /// <param name="extrenalUser">数据库中的用户列表</param>
        /// <returns>返回用户名</returns>
        private string GetRandomNumber(string quHao, int numberLength, List<string> extrenalUser)
        {

            while (true)
            {
                string randomNumber = string.Format("{0}{1}", quHao, RandomNumber.GetRnd(numberLength, true, true, false, false));
                if (!CheckUserNameExits(randomNumber,extrenalUser))
                {
                    //将新的一条记录加入到用户名列表中
                    extrenalUser.Add(randomNumber);
                    return randomNumber;
                }
            }
                    
        }
        
        /// <summary>
        /// 查找用户名列表
        /// </summary>
        /// <returns>返回用户名列表</returns>
        private List<string> GetAllUserName()
        {
            return _extrenalUserService.GetAllExtrenalUser().Select(x=>x.Name).ToList();
        }


//----------------------------------自动补全--------------------------------------//
//        public ActionResult AutoComplete(int start, int limit, int page, string query)
//        {
//            var data = _bll.GetAll().Where(x => x.WuYeBianHao.Contains(query)).OrderBy(x=>x.WuYeBianHao).ToPagedList(page, limit);
//            return this.Store(data, data.Count);
//        }
//----------------------------------自动补全--------------------------------------//

    }
}
