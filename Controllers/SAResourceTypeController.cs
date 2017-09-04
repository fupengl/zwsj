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
using EnumerableExtensions = ServiceStack.Common.Extensions.EnumerableExtensions;
using PartialViewResult = Ext.Net.MVC.PartialViewResult;

namespace ZhiDiExt.Controllers
{
    [Authorize]
    public class SAResourceTypeController : BaseController.BaseController
    {
        RbacService _service=new RbacService();
        
        //
        // GET: /DataDictionary/

        public ActionResult Index()
        {
            return View(GetToolarPermission());
        }

        public StoreResult GetIcon()
        {
            List<string> icons = Enum.GetNames(typeof(Icon)).ToList<string>();
            icons.Remove("None");

            List<object> data = new List<object>(icons.Count);

            ResourceManager resourceManager = new ResourceManager();
            icons.ForEach(icon => data.Add(
                new
                {
                    name = icon,
                    url = resourceManager.GetIconUrl((Icon)Enum.Parse(typeof(Icon), icon)),
                    iconCls = resourceManager.GetIconClass((Icon)Enum.Parse(typeof(Icon), icon))
                }
            ));

            return this.Store(data);
        }

        public PartialViewResult Create()
        {
            
            return new PartialViewResult
            {
                ViewName = "Create",
                Model = new SA_ResourceType {}
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
                Model = _service.GetResourceTypeById(id)
            }; 
            
        }        

        public ActionResult Delete(int id)
        {
            if (id == -1)
            {                
                return this.Direct();
            }

            _service.DeleteResourceType(id);
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
            return EnumerableExtensions.ToList<object>(_service.GetAllResourceTypes());
        }   

        public override ActionResult Refresh()
        {
            this.GetCmp<Store>("storeSA_Resource").Reload();
            return this.Direct();
        }

        public ActionResult Insert(SA_ResourceType entity)
        {
            if (ModelState.IsValid)
            {
                _service.AddResourceType(entity);
                _service.Save();
                
                this.GetCmp<Window>("windowSA_Resource").Hide();
                this.GetCmp<Store>("storeSA_Resource").Reload();     
                
                return this.Direct();
            }
            return this.Direct();
        }

        public ActionResult Update(SA_ResourceType entity)
        {
            if (ModelState.IsValid)
            {

                _service.UpdateResourceType(entity);
                _service.Save();
                
                var window = this.GetCmp<Window>("windowSA_Resource");
                window.Hide();
                this.GetCmp<Store>("storeSA_Resource").Reload();
                return this.Direct();
            }
            
            return this.Direct();
        }

    }
}
