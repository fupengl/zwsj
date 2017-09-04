using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
    public class SAResourceController : BaseController.BaseController
    {
        RbacService _service=new RbacService();
        //
        // GET: /DataDictionary/

        public ActionResult Index()
        {
            return View(GetToolarPermission());
        }

        //public PartialViewResult Create(string currentNode)
        //{
        //    SA_Resource curResource = JSON.Deserialize<SA_Resource>(currentNode);
        //    return new PartialViewResult
        //    {
        //        ViewName = "Create",
        //        Model = new SA_Resource {ParentId = curResource.Id}
        //    };
        //}

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
        //        Model = _bll.GetById(id)
        //    }; 
            
        //}        

        public ActionResult Delete(int id)
        {
            if (id == -1)
            {                
                return this.Direct();
            }
            
            var childs = _service.GetMenuItems(id);
            foreach (var saResource in childs)
            {
                _service.DeleteResource(saResource);
            }
            _service.DeleteResource(id);
            //SA_ResourceLinkBLL saResourceLinkBLL=new SA_ResourceLinkBLL();
            //var links = saResourceLinkBLL.GetByResourceId(id);
            //foreach (var saResourceLink in links)
            //{
            //    saResourceLinkBLL.Delete(saResourceLink);
            //}
            //saResourceLinkBLL.Save();
            //_bll.Save();
            return this.Direct();
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
            return EnumerableExtensions.ToList<object>(_service.GetAllResources().OrderBy(x=>x.SortIndex));
        }   

        public override ActionResult Refresh()
        {
            this.GetCmp<Store>("storeSA_Resource").Reload();
            return this.Direct();
        }

        public ActionResult Insert(SA_Resource entity)
        {
            if (ModelState.IsValid)
            {
                entity.Show = GetCheckBoxValue(entity.Show);
                entity.ResourceTypeId = 0;
                entity.Granted = "true";
                _service.AddResource(entity);
                _service.Save();

                RefreshTree(entity.ParentId);
                //this.GetCmp<Store>("storeSA_Resource").Reload();     
                
                return this.Direct();
            }
            return this.Direct();
        }

        private void RefreshTree(int parentId)
        {
            var parentNode = this.GetCmp<TreePanel>("treePanelSaResource").GetNodeById(parentId);
            parentNode.Set("leaf", false);
            parentNode.Reload();
            parentNode.ExpandChildren(true);
            //this.GetCmp<TreePanel>("treePanelSaResource").Update();
            this.GetCmp<Window>("windowSA_Resource").Hide();
        }

        public ActionResult Update(SA_Resource entity)
        {
            if (ModelState.IsValid)
            {
                entity.Show = GetCheckBoxValue(entity.Show);
                _service.UpdateResource(entity);
                _service.Save();
                
                var window = this.GetCmp<Window>("windowSA_Resource");
                window.Hide();
                this.GetCmp<Store>("storeSA_Resource").Reload();
                return this.Direct();
            }
            
            return this.Direct();
        }

        public ActionResult GetChildren(string node)
        {
            int parentId = Convert.ToInt32(node);

            var data = _service.ResourceTree(parentId);
            return Json(data.OrderBy(x=>x.SortIndex), JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateLink(string currentNode)
        {
            try
            {
                SA_Resource curResource = JSON.Deserialize<SA_Resource>(currentNode);
                return new PartialViewResult
                {
                    ViewName = "Create",
                    Model = new SA_Resource { ParentId = curResource.Id, ItemType = ResourceItemType.Link.ToString(), Show = "true" }
                };
            }
            catch (Exception)
            {
                return new PartialViewResult
                {
                    ViewName = "Create",
                    Model = new SA_Resource { ParentId = 0, ItemType = ResourceItemType.Link.ToString(), Show = "true" }
                };
            }                        
        }

        public ActionResult Edit(string currentNode)
        {
            SA_Resource curResource = JSON.Deserialize<SA_Resource>(currentNode);
            return new PartialViewResult
            {
                ViewName = "Edit",
                Model = curResource
            };

        }

        //public ActionResult AllocButton(string currentNode)
        //{
        //    SA_Resource curResource = JSON.Deserialize<SA_Resource>(currentNode);
        //    return new PartialViewResult
        //    {
        //        ViewName = "AllocateButton",
        //        Model = curResource
        //    };
        //}

        //public ActionResult SaveButtonAllocation(string resourceTypes)
        //{
        //    SA_ResourceLinkBLL resourceLinkBLL=new SA_ResourceLinkBLL();
            
        //    ChangeRecords<SA_ResourceLink> records =
        //        new StoreDataHandler(resourceTypes).BatchObjectData<SA_ResourceLink>();

        //    int parentId = 0;
        //    foreach (var resourceLink in records.Updated)
        //    {
        //        resourceLinkBLL.Update(resourceLink);
        //        var source=_bll.GetResource(resourceLink.ResourceId ?? 0, resourceLink.ResourceTypeId);
        //        if (source != null)
        //        {
        //            source.Granted = resourceLink.Granted;
        //        }
                
        //        parentId = resourceLink.ResourceId??0;
        //    }
        //    resourceLinkBLL.Save();
        //    _bll.Save();

        //    RefreshTree(parentId);
        //    return this.Direct();
        //}

        //public ActionResult GetResourceLink(string id)
        //{
        //    SA_ResourceTypeBLL resourceTypeBLL=new SA_ResourceTypeBLL();
        //    SA_ResourceLinkBLL resourceLinkBLL=new SA_ResourceLinkBLL();

        //    int resourceId = GetResourceId(id);
        //    var resourceTypes = resourceTypeBLL.GetAll().ToList();
        //    resourceLinkBLL.AddResourceType(resourceId, resourceTypes);
        //    _bll.AddResourceType(resourceId, resourceTypes);
        //    return this.Store(resourceLinkBLL.GetByResourceId(GetResourceId(id)));
        //}

        //private static int GetResourceId(string id)
        //{
        //    int resourceId;
        //    if (int.TryParse(id, out resourceId))
        //    {
        //        return resourceId;
        //    }

        //    return 0;
        //}

        public ActionResult GetResourceButton(string id)
        {
            return this.Store(_service.GetButtons(ConvertToInt(id)));
        }

        public ActionResult SaveButtonAllocation(string allocatedButtons)
        {
            ChangeRecords<SA_Resource> records =
                new StoreDataHandler(allocatedButtons).BatchObjectData<SA_Resource>();

            if (records.Updated.Count == 0) return this.Direct();

            foreach (SA_Resource saResource in records.Updated)
            {
                _service.InsertOrUpdateResource(saResource);
            }
            _service.Save();
            return this.Direct();
        }
    }
}
