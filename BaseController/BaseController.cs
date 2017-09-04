using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common.CommandTrees;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Helpers;
using System.Web.Mvc;
using Business;
using DataAccess.DocumentApprove;
using Ext.Net;
using Ext.Net.MVC;
using Ext.Net.Utilities;
using Infrastructure;
using Newtonsoft.Json.Serialization;
using Service;
using Service.DTOs;
using ZhiDiExt.Controllers;
using PartialViewResult = Ext.Net.MVC.PartialViewResult;

namespace ZhiDiExt.BaseController
{
    public abstract class BaseController : Controller
    {
        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonNetResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }

        private static MyPrincipal GetPrincipal()
        {
            return System.Web.HttpContext.Current.User as MyPrincipal;
        }



        protected string GetCurrentUserName()
        {
            return GetPrincipal().Identity.Name;
        }

        protected static OrgFilter GetActiveOrgFilter()
        {
            MyPrincipal principal = GetPrincipal();
            if (principal.ActiveFilter == null)
            {
                return new OrgFilter();
            }
            return principal.ActiveFilter;
        }

        protected static string GetOrganizationAreaPath()
        {
            return (GetPrincipal().Identity as MyIdentity).AreaPath;
        }

        protected string GetOrganizationNamePath()
        {
            return (GetPrincipal().Identity as MyIdentity).NamePath;
            //MyPrincipal principal = GetPrincipal();
            //if (principal.ActiveFilter == null)
            //{
            //    return "/";
            //}
            //return principal.ActiveFilter.NamePath;
        }

        protected string GetOrganizationIdPath()
        {
            return (GetPrincipal().Identity as MyIdentity).IdPath;
            //MyPrincipal principal = GetPrincipal();
            //if (principal.ActiveFilter == null)
            //{
            //    return "/";
            //}
            //return principal.ActiveFilter.OrgIdPath;
        }

        protected string GetOrganizationId()
        {
            return (GetPrincipal().Identity as MyIdentity).OrgId;
            //MyPrincipal principal = GetPrincipal();
            //if (principal.ActiveFilter == null)
            //{
            //    return "";
            //}
            //return principal.ActiveFilter.OrgId;
        }

        protected string GetCheckBoxValue(string val)
        {
            return string.IsNullOrEmpty(val) ? "false" : val;
        }

        protected bool CanBrowse()
        {
            var form = GetPrincipal().GetFormLink(RouteData.Values["controller"].ToString());
            if (form == null) return false;

            return form.CanBrowse;
        }

        private int ChartCount(string s, char c)
        {
            var query =
                s.ToCharArray()
                    .GroupBy(x => x)
                    .Select(x => new { x.Key, Cnt = x.Count() })
                    .Where(x => x.Key == c)
                    .FirstOrDefault();
            if (query == null)
            {
                return 0;
            }
            return query.Cnt;
        }

        public ToolbarPermissionDto GetToolarPermission()
        {
            string keyString;
            //string keyString = Request.RawUrl.TrimStart('/');

            //var form =
            //    GetPrincipal()
            //        .GetFormLink(string.IsNullOrEmpty(keyString) ? RouteData.Values["controller"].ToString() : keyString);
            //return form.ButtonStatus.ToToolbarPermission();
            string rawUrl = Request.RawUrl;
            if (ChartCount(rawUrl, '/') > 1)
            {
                keyString = rawUrl.TrimStart('/').Split('/').FirstOrDefault();
            }
            else
            {
                keyString = Request.RawUrl.TrimStart('/');
            }


            var form =
                GetPrincipal()
                    .GetFormLink(string.IsNullOrEmpty(keyString) ? RouteData.Values["controller"].ToString() : keyString);
            if (form == null)
            {
                return new ToolbarPermissionDto();
            }
            return form.ButtonStatus.ToToolbarPermission();
        }
        //public ActionResult GetButtonStatus()
        //{
        //    string keyString = Request.UrlReferrer.PathAndQuery.TrimStart('/');
        //    var form =
        //        GetPrincipal()
        //            .GetFormLink(string.IsNullOrEmpty(keyString) ? RouteData.Values["controller"].ToString() : keyString);
        //    return this.Direct(form.ButtonStatus);
        //}

        //public ActionResult SetButtonStatus()
        //{

        //    //string controllerName = RouteData.Values["controller"].ToString();
        //    //string queryStr = Request.UrlReferrer.Query;
        //    //string keyStr = string.Format("{0}/{1}", controllerName, queryStr);
        //    string keyString = Request.UrlReferrer.PathAndQuery.TrimStart('/');
        //    var form = GetPrincipal().GetFormLink(keyString);


        //    foreach (var status in form.ButtonStatus)
        //    {
        //        if(status.ButtonName=="browse") continue;
        //        if (status.Granted)
        //        {
        //            //this.GetCmp<Button>(status.ButtonName).Enable();
        //        }
        //        else
        //        {
        //            this.GetCmp<Button>(status.ButtonName).Enabled= false;
        //        }
        //    }
        //    return this.Direct();
        //}
        protected string GetSortExpression(StoreRequestParameters parameters)
        {
            string sort = "";
            for (int i = 0; i < parameters.Sort.Length; i++)
            {
                sort = sort + string.Format("{0} {1}, ", parameters.Sort[i].Property, parameters.Sort[i].Direction);
            }

            return string.IsNullOrEmpty(sort) ? "" : sort.Substring(0, sort.LastIndexOf(",", System.StringComparison.Ordinal));
        }

        protected List<object> FilterSortAndPaging(List<object> data, StoreRequestParameters parameters)
        {
            FilterConditions fc = null;
            try
            {
                fc = parameters.GridFilters;
            }
            catch (Exception)
            {
                return data;
            }
            //-- start filtering ------------------------------------------------------------
            FilterData(data, fc);
            //-- end filtering ------------------------------------------------------------

            //-- start sorting ------------------------------------------------------------
            SortData(data, parameters.Sort);
            //-- end sorting ------------------------------------------------------------


            //-- start paging ------------------------------------------------------------
            int limit = parameters.Limit;

            if ((parameters.Start + parameters.Limit) > data.Count)
            {
                limit = data.Count - parameters.Start;
            }

            List<object> rangeData = (parameters.Start < 0 || limit < 0) ? data : data.GetRange(parameters.Start, limit);
            //-- end paging ------------------------------------------------------------

            return rangeData;
        }

        private static void SortData(List<object> data, DataSorter[] sort)
        {
            if (sort.Length > 0)
            {
                DataSorter sorter = sort[0];
                data.Sort(delegate(object x, object y)
                {
                    object a;
                    object b;

                    int direction = sorter.Direction == Ext.Net.SortDirection.DESC ? -1 : 1;

                    a = x.GetType().GetProperty(sorter.Property).GetValue(x, null);
                    b = y.GetType().GetProperty(sorter.Property).GetValue(y, null);
                    return CaseInsensitiveComparer.Default.Compare(a, b) * direction;
                });
            }
        }

        private void FilterData(List<object> data, FilterConditions fc)
        {
            if (fc != null)
            {
                foreach (FilterCondition condition in fc.Conditions)
                {
                    Comparison comparison = condition.Comparison;
                    string field = condition.Field;
                    FilterType type = condition.Type;

                    object value;
                    switch (condition.Type)
                    {
                        case FilterType.Boolean:
                            value = condition.Value<bool>();
                            break;
                        case FilterType.Date:
                            value = condition.Value<DateTime>();
                            break;
                        case FilterType.List:
                            value = condition.List;
                            break;
                        case FilterType.Numeric:
                            if (data.Count > 0 && data[0].GetType().GetProperty(field).PropertyType == typeof(int))
                            {
                                value = condition.Value<int>();
                            }
                            else
                            {
                                value = condition.Value<double>();
                            }

                            break;
                        case FilterType.String:
                            value = condition.Value<string>();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    data.RemoveAll(
                        item =>
                        {
                            object oValue = item.GetType().GetProperty(field).GetValue(item, null);
                            IComparable cItem = oValue as IComparable;
                            if (cItem != null)
                                switch (comparison)
                                {
                                    case Comparison.Eq:

                                        switch (type)
                                        {
                                            case FilterType.List:
                                                return !(value as IList<string>).Contains(oValue.ToString());
                                            case FilterType.String:
                                                return !oValue.ToString().Contains(value.ToString());
                                            default:
                                                return !cItem.Equals(value);
                                        }
                                    case Comparison.Gt:
                                        var typeGt = cItem.GetType();
                                        switch (typeGt.Name)
                                        {
                                            case "Int32":
                                                return cItem.CompareTo(Convert.ToInt32(value)) < 1;
                                            case "Decimal":
                                                return cItem.CompareTo(Convert.ToDecimal(value)) < 1;
                                            default:
                                                return cItem.CompareTo(value) < 1;
                                        }
                                    case Comparison.Lt:
                                        var typeLt = cItem.GetType();
                                        switch (typeLt.Name)
                                        {
                                            case "Int32":
                                                return cItem.CompareTo(Convert.ToInt32(value)) > -1;
                                            case "Decimal":
                                                return cItem.CompareTo(Convert.ToDecimal(value)) > -1;
                                            default:
                                                return cItem.CompareTo(value) > -1;
                                        }
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                            return true;
                        });
                }
            }
        }

        protected string GetFilterExpression(StoreRequestParameters parameters)
        {
            string where = "";

            FilterConditions fc = parameters.GridFilters;
            if (fc == null) return where;

            foreach (FilterCondition condition in fc.Conditions)
            {
                Comparison comparison = condition.Comparison;
                string field = condition.Field;
                FilterType type = condition.Type;

                object value;
                switch (condition.Type)
                {
                    case FilterType.Numeric:
                        value = condition.Value<double>();
                        break;
                    case FilterType.List:
                        value = condition.List;
                        break;
                    case FilterType.Boolean:
                        value = condition.Value<bool>();
                        break;
                    case FilterType.String:
                        value = condition.Value<string>();
                        break;
                    case FilterType.Date:
                        value = condition.Value<DateTime>();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                string whereItem = "";
                switch (comparison)
                {
                    case Comparison.Eq:
                        switch (type)
                        {
                            case FilterType.String:
                                whereItem = string.Format("({0}.Contains(\"{1}\")) AND ", field, value);
                                break;
                            case FilterType.Date:
                                whereItem = string.Format("({0} = '{1}') AND ", field, value);
                                break;
                            case FilterType.Numeric:
                                whereItem = string.Format("({0} = {1}) AND ", field, value);
                                break;
                            case FilterType.Boolean:
                                whereItem = string.Format("({0} = '{1}') AND ", field, value);
                                break;
                            case FilterType.List:
                                string inList = "";
                                foreach (string s in value as List<string>)
                                {
                                    inList = inList + string.Format("'{0}',", s);
                                }
                                whereItem = string.Format("({0}  ({1})) AND ", field,
                                    string.IsNullOrEmpty(inList) ? "" : inList.Substring(0, inList.LastIndexOf(",")));
                                break;
                        }
                        break;
                    case Comparison.Gt:
                        switch (type)
                        {
                            case FilterType.String:
                                whereItem = string.Format("({0} > '{1}') AND ", field, value);
                                break;
                            case FilterType.Date:
                                whereItem = string.Format("({0} > '{1}') AND ", field, value);
                                break;
                            case FilterType.Numeric:
                                whereItem = string.Format("({0} > {1}) AND ", field, value);
                                break;
                            case FilterType.Boolean:
                                whereItem = string.Format("({0} > '{1}') AND ", field, value);
                                break;
                            case FilterType.List:
                                where = "";
                                break;
                        }
                        break;
                    case Comparison.Lt:
                        switch (type)
                        {
                            case FilterType.String:
                                whereItem = string.Format("({0} < '{1}') AND ", field, value);
                                break;
                            case FilterType.Date:
                                whereItem = string.Format("({0} < '{1}') AND ", field, value);
                                break;
                            case FilterType.Numeric:
                                whereItem = string.Format("({0} < {1}) AND ", field, value);
                                break;
                            case FilterType.Boolean:
                                whereItem = string.Format("({0} < '{1}') AND ", field, value);
                                break;
                            case FilterType.List:
                                where = "";
                                break;
                        }
                        break;
                }
                where = where + whereItem;
            }

            return string.IsNullOrEmpty(where) ? "" : where.Substring(0, where.LastIndexOf("AND"));
        }


        public FileResult ExportToExcel(string filters, string sorters, string columns)
        {
            List<object> dataToExport = GetData();
            FilterConditions condition;
            DataSorter[] sts;
            List<VisableColumn> cols;

            if (!string.IsNullOrEmpty(filters))
            {
                condition = new FilterConditions(filters);
                FilterData(dataToExport, condition);

            }
            if (!string.IsNullOrEmpty(sorters))
            {
                sts = JSON.Deserialize<DataSorter[]>(sorters,
                    (IContractResolver)new CamelCasePropertyNamesContractResolver());
                SortData(dataToExport, sts);

            }

            cols = JSON.Deserialize<List<VisableColumn>>(columns);

            DataTable dt = ConvertToDataTable(dataToExport, cols);

            using (var pck = new OfficeOpenXml.ExcelPackage())
            {
                int rowIndex = 1;
                var ws = pck.Workbook.Worksheets.Add("数据导出");
                for (int colIndex = 0; colIndex < cols.Count; colIndex++)
                {
                    ws.Cells[rowIndex, colIndex + 1].Value = cols[colIndex].text;
                }

                foreach (DataRow dr in dt.Rows)
                {
                    rowIndex++;
                    for (int colIndex = 0; colIndex < cols.Count; colIndex++)
                    {
                        ws.Cells[rowIndex, colIndex + 1].Value = dr[cols[colIndex].dataIndex];
                    }
                }
                return new FileContentResult(pck.GetAsByteArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }

        }

        private DataTable ConvertToDataTable(List<object> dataToExport, List<VisableColumn> cols)
        {
            var dt = new DataTable();
            if (dataToExport.Count > 0)
            {
                foreach (VisableColumn visableColumn in cols)
                {
                    PropertyInfo prop = dataToExport[0].GetType().GetProperty(visableColumn.dataIndex);
                    var dataColumn = new DataColumn(visableColumn.dataIndex,
                        Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                    //dataColumn.Caption = visableColumn.text;
                    dt.Columns.Add(dataColumn);
                }

                foreach (object o in dataToExport)
                {
                    DataRow dr = dt.NewRow();
                    foreach (VisableColumn visableColumn in cols)
                    {
                        var property = o.GetType().GetProperty(visableColumn.dataIndex);
                        var val = property.GetValue(o, null);
                        if (property.PropertyType == typeof(bool))
                        {
                            dr[visableColumn.dataIndex] = val ?? false;
                        }
                        else
                        {
                            dr[visableColumn.dataIndex] = val ?? DBNull.Value;
                        }
                    }
                    dt.Rows.Add(dr);
                }
            }

            return dt;
        }

        protected abstract List<object> GetData();

        protected int ConvertToInt(string id)
        {
            int result;
            if (int.TryParse(id, out result))
            {
                return result;
            }
            return 0;
        }

        protected string GetWuYeYongTuChn(string wuYeYongTu)
        {
            return WuYeYongTuHelper.ConvertToChn(wuYeYongTu);
        }

        protected ActionResult Download(string fileName, string contentType)
        {
            return File(fileName, contentType);
        }

        public ActionResult Approve(int id, string docType, string status)
        {
            return new PartialViewResult
            {
                ViewName = "DocApprove",
                Model = new DocModel { Id = id, DocType = docType, Status = status }
            };
        }

        public ActionResult DoApprove(DocModel docModel)
        {
            GetBll(docModel.DocType).Approve(docModel.Id, ApproveManager.Approve(docModel.Status), GetCurrentUserName(), docModel.Remark);

            var window = this.GetCmp<Window>("windowFormPanelDocApprove");
            window.Hide();

            Refresh();
            return this.Direct();
        }

        public ActionResult DoReject(DocModel docModel)
        {
            GetBll(docModel.DocType).Reject(docModel.Id, ApproveManager.Reject(docModel.Status), GetCurrentUserName(), docModel.Remark);

            var window = this.GetCmp<Window>("windowFormPanelDocApprove");
            window.Hide();

            Refresh();
            return this.Direct();
        }

        private BaseBLL GetBll(string docType)
        {
            switch (docType)
            {
                case "BanGongChengJiao": return new BanGongChengJiaoBLL();
                case "BanGongJiChuXinXi": return new BanGongJiChuXinXiBLL();
                case "BanGongJiZhunJia": return new BanGongJiZhunJiaBLL();
                case "BanGongLouCengXiuZheng": return new BanGongLouCengXiuZhengBLL();
                case "JingYingXingYongDiChengJiao": return new JingYingXingYongDiChengJiaoBLL();
                case "MingChengDuiZhai": return new MingChengDuiZhaiBLL();
                case "ShangYeChengJiao": return new ShangYeChengJiaoBLL();
                case "ShangYeJiChuXinXi": return new ShangYeJiChuXinXiBLL();
                case "ShangYeJiZhunJia": return new ShangYeJiZhunJiaBLL();
                case "XiuZhengXiShu": return new XiuZhengXiShuBLL();
                case "ZhuZhaiChengJiao": return new ZhuZhaiChengJiaoBLL();
                case "ZhuZhaiJiChuXinXi": return new ZhuZhaiJiChuXinXiBLL();
                case "ZhuZhaiJiZhunJia": return new ZhuZhaiJiZhunJiaBLL();
                case "ZhuZhaiLouDongXiuZheng": return new ZhuZhaiLouDongXiuZhengBLL();
                case "ZhuZhaiPingGu": return new ZhuZhaiPingGuBLL();
                case "GongYePianQu": return new GongYePianQuBLL();
                default:
                    return null;
            }
        }

        public abstract ActionResult Refresh();
    }
}