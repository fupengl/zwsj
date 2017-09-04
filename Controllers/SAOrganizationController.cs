using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Business;
using DataAccess;
using Ext.Net;
using Ext.Net.MVC;
using Infrastructure;
using Service;
using ServiceStack.Common.Extensions;
using PartialViewResult = Ext.Net.MVC.PartialViewResult;

namespace ZhiDiExt.Controllers
{
    [Authorize]
    public class SAOrganizationController : BaseController.BaseController
    {
        private readonly RbacService _service=new RbacService();
        private readonly XinZhengQuYuService _xinZhengQuYuService = new XinZhengQuYuService();
        
        //
        // GET: /DataDictionary/

        public ActionResult Index()
        {
            return View(GetToolarPermission());
        }

        public PartialViewResult Create()
        {
            return new PartialViewResult
            {
                ViewName = "Create",
                Model = new SA_Organization {}
            };
        }


        public ActionResult Delete(string id)
        {
            string parentId = _service.GetOrgById(id).ParentId;
            if (string.IsNullOrEmpty(id))
            {                
                return this.Direct();
            }

            _service.DeleteOrganization(id);
            _service.DeleteUser(id);
            _service.Save();
            
            //this.GetCmp<TreeStore>("storeSAOrganization").Reload();
            TreePanel treePanel = this.GetCmp<TreePanel>("treePanelSAOrganization");
            treePanel.GetNodeById(parentId).Reload();
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
            return EnumerableExtensions.ToList<object>(_service.GetAllOrganization());
        }   

        public override ActionResult Refresh()
        {
            this.GetCmp<Store>("storeSAOrganization").Reload();
            return this.Direct();
        }

        public ActionResult Insert(SA_Organization entity)
        {
            if (ModelState.IsValid)
            {
                entity.CreatedBy = GetCurrentUserName();
                entity.CreatedDate = DateTime.Now;

                _service.AddOrganization(entity);
                _service.Save();
                
                this.GetCmp<Window>("windowSA_Organization").Hide();

                TreePanel treePanel = this.GetCmp<TreePanel>("treePanelSAOrganization");
                treePanel.GetNodeById(entity.ParentId).Reload();   
                
                return this.Direct();
            }
            return this.Direct();
        }

        public ActionResult Update(SA_Organization entity)
        {
            if (ModelState.IsValid)
            {

                entity.LastModifiedBy = GetCurrentUserName();
                entity.LastModifiedDate = DateTime.Now;

                _service.UpdateOrganization(entity);
                _service.Save();
                
                var window = this.GetCmp<Window>("windowSA_Organization");
                window.Hide();

                TreePanel treePanel = this.GetCmp<TreePanel>("treePanelSAOrganization");
                treePanel.GetNodeById(entity.ParentId).Reload();   
                return this.Direct();
            }
            
            return this.Direct();
        }

        public ActionResult GetChildren(string node)
        {
            var data = _service.GetOrgTree(node);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult CreateCompany(string currentNode)
        {
            SA_Organization currentOrg = JSON.Deserialize<SA_Organization>(currentNode);
            PartialViewResult partialView =
                new PartialViewResult
                {
                    ViewName = "CreateCompany",
                    Model =
                        new SA_Organization()
                        {
                            ParentId = currentOrg.Id,
                            ParentCodePath = currentOrg.CodePath,
                            ParentNamePath = currentOrg.NamePath,
                            ParentIdPath = currentOrg.IdPath,
                            AreaPath = currentOrg.AreaPath,
                            OrgType = OrganizationType.Company.ToString(),
                            ValidState = ValidStates.Effective.ToString(),
                            Level = currentOrg.Level + 1,
                            AreaCode = currentOrg.AreaCode,
                            ParentAreaCode = currentOrg.AreaCode,
                            ParentAreaPath = currentOrg.AreaPath
                        }                        
                };
            return partialView;
        }

        public ActionResult CreateDepartment(string currentNode)
        {

            SA_Organization currentOrg = JSON.Deserialize<SA_Organization>(currentNode);
            PartialViewResult partialView =
                new PartialViewResult
                {
                    ViewName = "CreateDepartment",
                    Model =
                        new SA_Organization()
                        {
                            ParentId = currentOrg.Id,
                            ParentCodePath = currentOrg.CodePath,
                            ParentNamePath = currentOrg.NamePath,
                            ParentIdPath = currentOrg.IdPath,
                            AreaPath = currentOrg.AreaPath,
                            OrgType = OrganizationType.Department.ToString(),
                            ValidState = ValidStates.Effective.ToString(),
                            Level = currentOrg.Level + 1,
                            AreaCode = currentOrg.AreaCode,
                            ParentAreaCode = currentOrg.AreaCode,
                            ParentAreaPath = currentOrg.AreaPath
                        }
                };
            return partialView;
        }

        public ActionResult CreatePosition(string currentNode)
        {
            SA_Organization currentOrg = JSON.Deserialize<SA_Organization>(currentNode);
            PartialViewResult partialView =
                new PartialViewResult
                {
                    ViewName = "CreatePosition",
                    Model =
                        new SA_Organization()
                        {
                            ParentId = currentOrg.Id,
                            ParentCodePath = currentOrg.CodePath,
                            ParentNamePath = currentOrg.NamePath,
                            ParentIdPath = currentOrg.IdPath,
                            AreaPath = currentOrg.AreaPath,
                            OrgType = OrganizationType.Position.ToString(),
                            ValidState = ValidStates.Effective.ToString(),
                            Level = currentOrg.Level + 1,
                            AreaCode = currentOrg.AreaCode,
                            ParentAreaCode = currentOrg.AreaCode,
                            ParentAreaPath = currentOrg.AreaPath
                        }
                };
            return partialView;
        }

        public ActionResult CreateStaff(string currentNode)
        {
            SA_Organization currentOrg = JSON.Deserialize<SA_Organization>(currentNode);
            PartialViewResult partialView =
                new PartialViewResult
                {
                    ViewName = "CreateStaff",
                    Model =new SA_User
                    {
                        MainOrgId = currentOrg.Id
                    }

                };
            return partialView;
        }

        public ActionResult InsertCompany(SA_Organization entity)
        {
            if (ModelState.IsValid)
            {
                entity.CreatedBy = GetCurrentUserName();
                entity.CreatedDate = DateTime.Now;

                _service.AddOrganization(entity);
                _service.Save();

                var parentNode = this.GetCmp<TreePanel>("treePanelSAOrganization").GetNodeById(entity.ParentId);
                parentNode.Set("leaf", false);
                parentNode.Reload();
                parentNode.ExpandChildren(true);
                this.GetCmp<Window>("windowSA_Organization").Hide();

                //this.GetCmp<Window>("windowSA_Organization").Hide();
                //this.GetCmp<Store>("storeSAOrganization").Reload();

                return this.Direct();
            }
            return this.Direct();
        }

        public ActionResult UpdateCompany(SA_Organization entity)
        {
            if (ModelState.IsValid)
            {

                entity.LastModifiedBy = GetCurrentUserName();
                entity.LastModifiedDate = DateTime.Now;

                _service.UpdateOrganization(entity);
                _service.Save();

                var window = this.GetCmp<Window>("windowSA_Organization");
                window.Hide();

                TreePanel treePanel = this.GetCmp<TreePanel>("treePanelSAOrganization");
                treePanel.GetNodeById(entity.ParentId).Reload();
                return this.Direct();
            }

            return this.Direct();
        }

        public ActionResult InsertDepartment(SA_Organization entity)
        {
            if (ModelState.IsValid)
            {
                entity.CreatedBy = GetCurrentUserName();
                entity.CreatedDate = DateTime.Now;

                _service.AddOrganization(entity);
                _service.Save();

                var parentNode = this.GetCmp<TreePanel>("treePanelSAOrganization").GetNodeById(entity.ParentId);
                parentNode.Set("leaf", false);
                parentNode.Reload();
                parentNode.ExpandChildren(true);
                this.GetCmp<Window>("windowSA_Organization").Hide();
                //this.GetCmp<Window>("windowSA_Organization").Hide();
                //this.GetCmp<Store>("storeSAOrganization").Reload();

                return this.Direct();
            }
            return this.Direct();
        }

        public ActionResult UpdateDepartment(SA_Organization entity)
        {
            if (ModelState.IsValid)
            {

                entity.LastModifiedBy = GetCurrentUserName();
                entity.LastModifiedDate = DateTime.Now;

                _service.UpdateOrganization(entity);
                _service.Save();

                var window = this.GetCmp<Window>("windowSA_Organization");
                window.Hide();

                TreePanel treePanel = this.GetCmp<TreePanel>("treePanelSAOrganization");
                treePanel.GetNodeById(entity.ParentId).Reload();
                return this.Direct();
            }

            return this.Direct();
        }

        public ActionResult InsertPosition(SA_Organization entity)
        {
            if (ModelState.IsValid)
            {
                entity.CreatedBy = GetCurrentUserName();
                entity.CreatedDate = DateTime.Now;

                _service.AddOrganization(entity);
                _service.Save();

                var parentNode = this.GetCmp<TreePanel>("treePanelSAOrganization").GetNodeById(entity.ParentId);
                parentNode.Set("leaf", false);
                parentNode.Reload();
                parentNode.ExpandChildren(true);
                this.GetCmp<Window>("windowSA_Organization").Hide();
                //this.GetCmp<Window>("windowSA_Organization").Hide();
                //this.GetCmp<Store>("storeSAOrganization").Reload();

                return this.Direct();
            }
            return this.Direct();
        }

        public ActionResult UpdatePosition(SA_Organization entity)
        {
            if (ModelState.IsValid)
            {

                entity.LastModifiedBy = GetCurrentUserName();
                entity.LastModifiedDate = DateTime.Now;

                _service.UpdateOrganization(entity);
                _service.Save();

                var window = this.GetCmp<Window>("windowSA_Organization");
                window.Hide();

                TreePanel treePanel = this.GetCmp<TreePanel>("treePanelSAOrganization");
                treePanel.GetNodeById(entity.ParentId).Reload();
                return this.Direct();
            }

            return this.Direct();
        }

        public ActionResult InsertStaff(SA_User entity, int count)
        {
            string quHao = _xinZhengQuYuService.GetByAreaName(entity.City).QuHao;//).QuHao;
            string city = GetOrganizationAreaPath().Substring(GetOrganizationAreaPath().LastIndexOf("/") + 1);
            if (ModelState.IsValid)
            {
                for (int i = 0; i < count; i++)
                {
                    entity.CreatedBy = GetCurrentUserName();
                    entity.CreatedDate = DateTime.Today;
                    string xuLieHao = quHao + RandomNumber.GetRnd(5, true, true, false, false);
                    entity.Account = xuLieHao;
                    entity.Name = xuLieHao;
                    entity.City = city;
                    entity.Password = entity.Password ?? "123456";
                    _service.AddUser(entity);
                    _service.Save();
                }
               

                var parentNode = this.GetCmp<TreePanel>("treePanelSAOrganization").GetNodeById(entity.MainOrgId);
                parentNode.Set("leaf", false);
                parentNode.Reload();
                parentNode.ExpandChildren(true);
                this.GetCmp<Window>("windowSA_User").Hide();
                //this.GetCmp<Window>("windowSA_Organization").Hide();
                //this.GetCmp<Store>("storeSAOrganization").Reload();

                return this.Direct();
            }
            return this.Direct();
        }

        public ActionResult UpdateStaff(SA_User entity)
        {
            if (ModelState.IsValid)
            {

                entity.LastModifiedBy = GetCurrentUserName();
                entity.LastModifiedDate = DateTime.Now;
                var org = new SA_Organization();
                entity.Account = entity.Name;
                org = _service.GetOrgContainsId(entity.Id);
                org.Name = entity.Name;
                org.Description = entity.Name;
                org.Code = entity.Name;
                _service.UpdateOrganization(org);
                _service.UpdateUser(entity);
                _service.Save();

                var window = this.GetCmp<Window>("windowSA_User");
                window.Hide();

                TreePanel treePanel = this.GetCmp<TreePanel>("treePanelSAOrganization");
                treePanel.GetNodeById(entity.MainOrgId).Reload();
                return this.Direct();
            }

            return this.Direct();
        }

        public ActionResult Edit(string currentNode)
        {
            SA_Organization currentOrg = JSON.Deserialize<SA_Organization>(currentNode);
            OrganizationType orgType = EnumExtension<OrganizationType>.Parse(currentOrg.OrgType);
            switch (orgType)
            {
                case OrganizationType.Company:
                    return new PartialViewResult
                    {
                        ViewName = "EditCompany",
                        Model = _service.GetOrgById(currentOrg.Id)
                    };
                case OrganizationType.Department:
                    return new PartialViewResult
                    {
                        ViewName = "EditDepartment",
                        Model = _service.GetOrgById(currentOrg.Id)
                    };
                case OrganizationType.Position:
                    return new PartialViewResult
                    {
                        ViewName = "EditPosition",
                        Model = _service.GetOrgById(currentOrg.Id)
                    };
                case OrganizationType.Staff:
                    return new PartialViewResult
                    {
                        ViewName = "EditStaff",
                        Model = _service.GetUserByOrgId(currentOrg.Id)
                    };
                default:
                    throw new NotImplementedException();
            }           
        }

        
    }
}
