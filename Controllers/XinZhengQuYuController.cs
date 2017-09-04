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
using Newtonsoft.Json.Linq;
using Service;
using ServiceStack.Text;
using PartialViewResult = Ext.Net.MVC.PartialViewResult;
using ServiceStack.Common.Extensions;

namespace ZhiDiExt.Controllers
{
    [Authorize]
    public class XinZhengQuYuController : BaseController.BaseController
    {
        readonly XinZhengQuYuService _service=new XinZhengQuYuService();

        //
        // GET: /DataDictionary/

        public ActionResult Index()
        {
            return View(GetToolarPermission());
        }

        public PartialViewResult Create(string parentId)
        {
            return new PartialViewResult
            {
                ViewName = "Create",
                Model = new XinZhengQuYu { ParentId = Convert.ToInt32(parentId) }
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
                Model = _service.GetXinZhengQuYuById(id)
            }; 
            
        }        

        public ActionResult Delete(int id)
        {
            if (id == -1)
            {                
                return this.Direct();
            }

            _service.DeleteXinZhengQuYu(id);
            _service.Save();


            //TreePanel treePanel = this.GetCmp<TreePanel>("xinZhengQuYuTreePanel");
            //treePanel.GetNodeById(parentId).Reload();
            return Refresh();            
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
            return EnumerableExtensions.ToList<object>(_service.GetAllXinZhengQuYu());
        }   


        public override ActionResult Refresh()
        {
            this.GetCmp<TreeStore>("xinZhengQuYuTreeStore").Reload();
            return this.Direct();
        }

        public ActionResult Insert(XinZhengQuYu entity)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(entity.RecordType))
                {
                    entity.RecordType = RecordType.User.ToString();
                }
                entity.CreatedBy = GetCurrentUserName();
                entity.CreatedDate = DateTime.Now;

                _service.AddXinZhengQuYu(entity);
                _service.Save();
                //this.GetCmp<Window>("windowXinZhengQuYu").Hide();

                //TreePanel treePanel = this.GetCmp<TreePanel>("xinZhengQuYuTreePanel");
                //treePanel.GetNodeById(entity.ParentId).Reload();
                var parentNode = this.GetCmp<TreePanel>("xinZhengQuYuTreePanel").GetNodeById(entity.ParentId);
                parentNode.Set("leaf", false);
                parentNode.Reload();
                parentNode.ExpandChildren(true);
                this.GetCmp<Window>("windowXinZhengQuYu").Hide();
            }
            return this.Direct();
        }

        public ActionResult Update(XinZhengQuYu entity)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(entity.RecordType))
                {
                    entity.RecordType = RecordType.User.ToString();
                }

                entity.LastModifiedBy = GetCurrentUserName();
                entity.LastModifiedDate = DateTime.Now;

                _service.UpdateXinZhengQuYu(entity);
                _service.Save();
                
                var window = this.GetCmp<Window>("windowXinZhengQuYu");
                window.Hide();

                TreePanel treePanel = this.GetCmp<TreePanel>("xinZhengQuYuTreePanel");
                treePanel.GetNodeById(entity.Id).ParentNode().Reload();
                
                return this.Direct();
            }
            
            return this.Direct();
        }

        public JsonResult GetChildren(string node)
        {
            int parentId = Convert.ToInt32(node);

            var allNodes = _service.GetAllXinZhengQuYu().ToList();
            var children =
                allNodes.Where(x => x.ParentId == parentId)
                    .Select(
                        x =>
                            new
                            {
                                x.Id,
                                x.ParentId,
                                x.Code,
                                x.Description,
                                x.RecordType,
                                x.TreePath,
                                x.QuYuLeiXing,
                                x.Px,
                                x.Py,
                                leaf = !allNodes.Any(o => o.ParentId == x.Id)
                            });
            return Json(children, JsonRequestBehavior.AllowGet);
            //var data = _bll.GetChildren(Convert.ToInt32(node)).ToList();
            //var query =
            //    data.Select(x => new {x.Id, x.ParentId, x.Code, x.Description, x.RecordType, x.TreePath, leaf = });
            //return Json(query, JsonRequestBehavior.AllowGet);
            //return Json(_bll.GetChildren(Convert.ToInt32(node)), JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetXinZhengQuYu(string path, string quYuLeiXing)
        //{
        //    return Json(_bll.GetXinZhengQuYu(path, quYuLeiXing), JsonRequestBehavior.AllowGet);
        //}

        public JsonResult GetChildrenByAreaPath(string node)
        {
            var allNodes = _service.GetAllXinZhengQuYu().ToList();
            var children =
                allNodes.Where(x => x.TreePath.StartsWith(node))
                    .Select(
                        x =>
                            new
                            {
                                x.Id,
                                x.ParentId,
                                x.Code,
                                x.Description,
                                x.RecordType,
                                x.TreePath,
                                x.QuYuLeiXing,
                                leaf = !allNodes.Any(o => o.ParentId == x.Id)
                            });
            return Json(children, JsonRequestBehavior.AllowGet);
        }

        public JArray GetCityList()
        {
            var query = _service.GetAllShi();
            var jArray = new JArray();
            foreach (var q in query)
            {
                jArray.Add(new JObject
                {
                    {"Id", q.Id},
                    {"Description", q.Description}
                });
            }
            return jArray;
        }
    }
}
