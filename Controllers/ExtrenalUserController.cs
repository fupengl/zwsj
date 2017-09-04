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
            X.Msg.Alert(MsgInfo.TI_SHI, "�Ѱ�װ�ͻ�" + count + "����").Show();
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
        /// �������ݿ����Ƿ��Ѵ��ڸ��û���
        /// </summary>
        /// <param name="userName">�û���</param>
        /// <param name="extrenalUser">���ݿ��е��û��б�</param>
        /// <returns>����û��������ݿ����ҵ�����true,���򷵻�false</returns>
        private bool CheckUserNameExits(string userName, IEnumerable<string> extrenalUser)
        {
            return extrenalUser.Any(x => x == userName);
        }

        /// <summary>
        /// �Զ������û���
        /// </summary>
        /// <param name="quHao">���е�����</param>
        /// <param name="numberLength">������ĳ���</param>
        /// <param name="extrenalUser">���ݿ��е��û��б�</param>
        /// <returns>�����û���</returns>
        private string GetRandomNumber(string quHao, int numberLength, List<string> extrenalUser)
        {

            while (true)
            {
                string randomNumber = string.Format("{0}{1}", quHao, RandomNumber.GetRnd(numberLength, true, true, false, false));
                if (!CheckUserNameExits(randomNumber,extrenalUser))
                {
                    //���µ�һ����¼���뵽�û����б���
                    extrenalUser.Add(randomNumber);
                    return randomNumber;
                }
            }
                    
        }
        
        /// <summary>
        /// �����û����б�
        /// </summary>
        /// <returns>�����û����б�</returns>
        private List<string> GetAllUserName()
        {
            return _extrenalUserService.GetAllExtrenalUser().Select(x=>x.Name).ToList();
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
