using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web.Helpers;
using System.Web.Mvc;
using DataAccess;
using DataAccess.DocumentApprove;
using Ext.Net;
using Ext.Net.MVC;
using FileUploadManager;
using Infrastructure;
using Service;
using ZhiDiExt.Models;
using Business;
using cn.jpush.api;
using cn.jpush.api.push.mode;
using cn.jpush.api.push.notification;
using DataImport;
using ZhiDiExt.Mobile.Utility;
using Notification = Ext.Net.Notification;
using PartialViewResult = Ext.Net.MVC.PartialViewResult;

namespace ZhiDiExt.Controllers
{
    [Authorize]
    public class FileUploadController : BaseController.BaseController
    {

        private const string Success = "导入文件成功";
        private const string Fail = "导入文件失败";
        private const string TiShi = "提示";
        private const string NoFile = "没有文件";
        public readonly string FilePath = ConfigurationManager.AppSettings["UploadPath"];

        //public PartialViewResult ImageMultiUpload()
        //{
        //    return View()
        //}

        public ActionResult ImageMultiUpload(string areaPath, string wuYeYongTu, string wuYeBianHao, string wuYeMingCheng,
            string zhaoPianLeiXin, string imageDataType)
        {
            return new PartialViewResult
            {
                ViewName = "ImageMultiUpload",
                Model =
                    new FileUploadModel
                    {
                        Title = "图片上传",
                        ImportDataType = imageDataType,
                        AreaPath = areaPath,
                        WuYeYongTu = wuYeYongTu,
                        WuYeBianHao = wuYeBianHao,
                        WuYeMingCheng = wuYeMingCheng,
                        ZhaoPianLeiXin = zhaoPianLeiXin
                    }
            };
        }


        //public void FileUpload(object sender, FileUploadEventArgs e)
        //{

        //    X.Msg.Notify("File is uploaded", "Name: " + e.FileName).Show();
        //}

        public ActionResult ExcelUpload(string importDataType)
        {

            //BanGongChengJiao
            return new PartialViewResult
            {
                ViewName = "ExcelUpload",
                Model = new FileUploadModel { Title = "数据导入", ImportDataType = importDataType }
            };
        }

        public ActionResult SoftwareUpload(string version, string type, string plistUrl)
        {
            return new PartialViewResult
            {
                ViewName = "SoftwareUpload",
                Model = new SystemUpdateModel { Version = version, Type = type, PListUrl = plistUrl }
            };
        }

        public ActionResult ImageUpload(string areaPath, string wuYeYongTu, string wuYeBianHao, string wuYeMingCheng,
            string zhaoPianLeiXin, string imageDataType)
        {

            //BanGongChengJiao
            return new PartialViewResult
            {
                ViewName = "ImageUpload",
                Model =
                    new FileUploadModel
                    {
                        Title = "图片上传",
                        ImportDataType = imageDataType,
                        AreaPath = areaPath,
                        WuYeYongTu = wuYeYongTu,
                        WuYeBianHao = wuYeBianHao,
                        WuYeMingCheng = wuYeMingCheng,
                        ZhaoPianLeiXin = zhaoPianLeiXin
                    }
            };
        }



        private string GetPostedFileName(string fileName)
        {
            return fileName.Substring(fileName.LastIndexOf("\\", StringComparison.Ordinal) + 1);
        }

        #region GetUrl

        private string GetUrl(string city, string muLu, string fileName)
        {
            return string.Format(@"~/{0}/{1}/{2}", muLu, city, fileName);
        }

        private string GetUrl(string city, string type, string muLu, string fileName)
        {
            return string.Format(@"~/{0}/{1}/{2}/{3}", muLu, city, type, fileName);
        }

        private string GetUrl(string city, string type, string wuYeBianHao, string muLu, string fileName)
        {
            return string.Format(@"~/{0}/{1}/{2}/{3}/{4}", muLu, city, type, wuYeBianHao, fileName);
        }

        #endregion

        #region GetFilePath

        private string GetFilePath(string appType, string baseFolder)
        {
            return string.Format(@"{0}\{1}", baseFolder, appType);
        }

        private string GetFilePath(string city, string type, string baseFolder)
        {
            return string.Format(@"{0}\{1}\{2}", baseFolder, city, type);
        }

        private string GetFilePath(string city, string type, string baseFolder, string wuYeBianHao)
        {
            return string.Format(@"{0}\{1}\{2}\{3}", baseFolder, city, type, wuYeBianHao);
        }

        #endregion


        ////修改----------------------------------------------------
        //public string GetImageFilePath(string shi, string wuYeYongTu, string wuyeBianHao)
        //{
        //    string filePath;
        //    //系统照片（物业用途为空）
        //    if (wuYeYongTu == "")
        //    {
        //        //照片\苏州市
        //        filePath = string.Format(@"{0}\{1}", GetZhaoPianBaseFolder(), shi);
        //        return filePath;
        //    }
        //    //照片\住宅\苏州市\0001
        //    filePath = string.Format(@"{0}\{1}\{2}\{3}", GetZhaoPianBaseFolder(), wuYeYongTu,shi, wuyeBianHao);
        //    //string filePath = string.Format(@"{0}\{1}\{2}\{3}\{4}", GetZhaoPianBaseFolder(), GetAreaPath(areaPath), wuYeYongTu, wuYeMingCheng,
        //    //    zhaoPianLeiXin);
        //    return filePath;
        //}

        #region GetBaseFolder

        private string GetZhaoPianBaseFolder()
        {
            return Server.MapPath("~/" + GetZhaoPianMuLu());
        }

        private string GetDocumentBaseFolder()
        {
            return Server.MapPath("~/" + GetDocumentMuLu());
        }

        private string GetSoftwareBaseFolder()
        {
            return Server.MapPath("~/" + GetSoftwareMuLu());
        }

        #endregion

        #region GetMuLuFromConfig

        private string GetDocumentMuLu()
        {
            return ConfigurationManager.AppSettings["DocumentMuLu"];
        }

        private string GetZhaoPianMuLu()
        {
            return ConfigurationManager.AppSettings["ZhaopianMuLu"];
        }

        private string GetSoftwareMuLu()
        {
            return ConfigurationManager.AppSettings["SoftwareMuLu"];
        }

        #endregion

        #region ImageResize

        private int ImageResizeToWidth()
        {
            return ConvertToInt(ConfigurationManager.AppSettings["ResizeToWidth"]);
        }

        private int ImageResizeToHeight()
        {
            return ConvertToInt(ConfigurationManager.AppSettings["ResizeToHeight"]);
        }

        #endregion

        #region UploadClick



        public ActionResult ExcelUploadClick(string importDataType)
        {
            var result = new DirectResult();

            var fileUploadField = this.GetCmp<FileUploadField>("fileUploadField");
            if (fileUploadField.HasFile)
            {
                var excelImportManager = new ExcelImportManager();
                try
                {
                    string orgNamePath = GetOrganizationNamePath();
                    string createBy = GetCurrentUserName();
                    string areaPath = GetOrganizationAreaPath();
                    excelImportManager.ImportData(importDataType, fileUploadField.PostedFile.InputStream, areaPath, orgNamePath, createBy);
                    if (string.IsNullOrEmpty(excelImportManager.ErrorInfo))
                    {
                        X.Msg.Show(MessageBoxConfig(Success, TiShi, MessageBox.Icon.INFO));
                    }
                    else
                    {
                        X.Msg.Show(MessageBoxConfig(Success + "<br/>" + excelImportManager.ErrorInfo, TiShi, MessageBox.Icon.INFO));
                    }
                }
                catch (Exception ex)
                {
                    X.Msg.Show(MessageBoxConfig(ex.ToString(), Fail, MessageBox.Icon.INFO));
                }
            }
            else
            {
                result.IsUpload = true;
                X.Msg.Show(MessageBoxConfig(NoFile, TiShi, MessageBox.Icon.ERROR));
            }


            return result;
        }

        #region MessageBoxConfig

        private static MessageBoxConfig MessageBoxConfig(string message, string tiShi, MessageBox.Icon icon)
        {
            return new MessageBoxConfig
            {
                Buttons = MessageBox.Button.OK,
                Icon = icon,
                Title = tiShi,
                Message = message
            };
        }

        #endregion


        public ActionResult SoftwareUploadClick(string version, string type, string plistUrl)
        {
            var result = new DirectResult();
            var fileUploadField = this.GetCmp<FileUploadField>("fileUploadField");

            if (fileUploadField.HasFile)
            {
                try
                {
                    string postedFileName = GetPostedFileName(fileUploadField.PostedFile.FileName);
                    postedFileName = ChinesePinYin.GetPinYin(postedFileName);
                    var documentAttachment = new DocumentAttachment(GetFilePath(type, GetSoftwareBaseFolder()));
                    string filePath = documentAttachment.GetFileName(postedFileName);
                    string url = GetUrl(type, GetSoftwareMuLu(), postedFileName);
                    fileUploadField.PostedFile.SaveAs(filePath);
                    SaveSoftwareFilePathToDbstring(version, type, url, plistUrl);
                    X.Msg.Show(MessageBoxConfig(Success, TiShi, MessageBox.Icon.INFO));
                }
                catch (Exception ex)
                {
                    X.Msg.Show(MessageBoxConfig(ex.ToString(), Fail, MessageBox.Icon.INFO));
                }
                finally
                {
                    PushMessageToTag(type.ToLower(), version, "all", 1, -1, string.Empty, string.Empty, 0);
                }

            }
            else
            {
                result.IsUpload = true;
                X.Msg.Show(MessageBoxConfig(NoFile, TiShi, MessageBox.Icon.ERROR));
            }
            return result;
        }

        public ActionResult ImageUploadClick(string areaPath, string wuYeYongTu, string wuYeBianHao,
            string wuYeMingCheng, string zhaoPianLeiXin, string importDataType)
        {
            var result = new DirectResult();
            var fileUploadField = this.GetCmp<FileUploadField>("fileUploadField");

            if (fileUploadField.HasFile)
            {
                //string strFileName = Path.GetExtension(fileUploadField.PostedFile.FileName).ToUpper();//获取文件后缀
                //if (!(strFileName == ".BMP" || strFileName == ".GIF" || strFileName == ".JPG"))
                //{
                //    result.IsUpload = true;
                //    X.Msg.Show(MessageBoxErrorFormatConfig());
                //}
                try
                {
                    string shi = GetCity(areaPath).Description;
                    string postedFileName = GetPostedFileName(fileUploadField.PostedFile.FileName);
                    postedFileName = ChinesePinYin.GetPinYin(postedFileName);
                    //wuYeYongTu = ChinesePinYin.GetPinYin(wuYeYongTu);
                    var imageAttachment =
                        new ImageAttachment(GetFilePath(ChinesePinYin.GetPinYin(shi),
                            ChinesePinYin.GetPinYin(wuYeYongTu), GetZhaoPianBaseFolder(), wuYeBianHao));
                    string imageFileFullPath = imageAttachment.GetFileName(postedFileName);
                    var img = new WebImage(fileUploadField.PostedFile.InputStream);
                    SetImageDirection(img);
                    img.Save(imageFileFullPath);

                    //string url = GetUrl(areaPath, wuYeYongTu, wuYeMingCheng, zhaoPianLeiXin, postedFileName);
                    string url = GetUrl(ChinesePinYin.GetPinYin(shi), ChinesePinYin.GetPinYin(wuYeYongTu), wuYeBianHao,
                        GetZhaoPianMuLu(), postedFileName);
                    SaveImageFilePathToDb(wuYeYongTu, shi, wuYeBianHao, wuYeMingCheng, zhaoPianLeiXin, imageFileFullPath,
                        url, postedFileName, areaPath, img.GetBytes());

                    X.Msg.Show(MessageBoxConfig(Success, TiShi, MessageBox.Icon.INFO));
                }
                catch (Exception ex)
                {
                    X.Msg.Show(MessageBoxConfig(ex.ToString(), Fail, MessageBox.Icon.INFO));
                }

            }
            else
            {
                result.IsUpload = true;
                X.Msg.Show(MessageBoxConfig(NoFile, TiShi, MessageBox.Icon.ERROR));
            }
            return result;
        }

        public ActionResult DocumentUploadClick(string title, string areaPath, string documentType, string releaseDate)
        {
            log4net.LogManager.GetLogger("DocumentUploadClick").InfoFormat("推送周刊数据【{0}】-【{1}】-【{2}】-【{3}】", title, areaPath, documentType, releaseDate);
            var limitClick = Session["DocumentUploadClick"] as DateTime?;
            if (limitClick.HasValue)
            {
                if ((DateTime.Now - limitClick.Value).TotalMinutes < 1)
                {
                    X.Msg.Show(MessageBoxConfig("不允许一分钟内重复发布周刊", TiShi, MessageBox.Icon.ERROR));
                    return this.Direct();
                }
            }
            var result = new DirectResult();

            var fileUploadField = this.GetCmp<FileUploadField>("fileUploadField");
            var xingZhengQuYu = GetCity(areaPath);
            string shi = xingZhengQuYu.Description;
            if (fileUploadField.HasFile)
            {
                try
                {
                    string postedFileName = GetPostedFileName(fileUploadField.PostedFile.FileName);
                    postedFileName = ChinesePinYin.GetPinYin(postedFileName);
                    string type = ChinesePinYin.GetPinYin(documentType);
                    var documentAttachment = new DocumentAttachment(GetFilePath(ChinesePinYin.GetPinYin(shi), type, GetDocumentBaseFolder()));
                    string filePath = documentAttachment.GetFileName(postedFileName);
                    string url = GetUrl(ChinesePinYin.GetPinYin(shi), type, GetDocumentMuLu(), postedFileName);
                    fileUploadField.PostedFile.SaveAs(filePath);
                    SaveDocumentFilePathToDbstring(title, shi, postedFileName, fileUploadField.PostedFile.ContentType, documentType, releaseDate, filePath, url, areaPath);

                    switch (documentType)
                    {
                        case MsgInfo.ZhouKan:
                        case MsgInfo.ZhiDiFenXi:
                            //PushMessageToTag2(title, shi, 2, shi, documentType, xingZhengQuYu.Id);
                            PushMessageToTag2(title, shi, 2, shi, MsgInfo.ZhiDiFenXi, xingZhengQuYu.Id);
                            break;
                        case MsgInfo.TuDiGuaPai:
                        case MsgInfo.DiChanShaLong:
                        case MsgInfo.ZiChanChuZhi:
                            PushMessageToTag2(title, "all", 2, shi, documentType, xingZhengQuYu.Id);
                            break;
                    }
                    X.Msg.Show(MessageBoxConfig(Success, TiShi, MessageBox.Icon.INFO));
                }
                catch
                    (Exception ex)
                {
                    X.Msg.Show(MessageBoxConfig(ex.ToString(), Fail, MessageBox.Icon.INFO));
                }
            }
            else
            {
                result.IsUpload = true;
                X.Msg.Show(MessageBoxConfig(NoFile, TiShi, MessageBox.Icon.ERROR));
            }
            return result;
        }

        private void PushMessageToTag(string deviceType, string detail, string tag, int notificationType, int documentTypeId, string updateCity, string documentType, int updateCityId)
        {
            string message;
            if (!string.IsNullOrEmpty(updateCity) && !string.IsNullOrEmpty(documentType))
            {
                message = documentType == "地产沙龙" ? detail : string.Format("{0}更新了{1}。", updateCity, documentType);

            }
            else
            {
                message = string.Format("智地数据系统更新。新版本为{0}", detail);
            }
            //PushMessage.PushUpdateMessageToTag(deviceType, message, notificationType, tag, updateCity, documentType);
            PushMessageNew.PushMessage(message, notificationType, 0, tag, updateCity, documentType, updateCityId);
        }

        private void PushMessageToTag2(string detail, string tag, int notificationType, string updateCity, string documentType, int updateCityId)
        {
            string message;
            if (!string.IsNullOrEmpty(updateCity) && !string.IsNullOrEmpty(documentType))
            {
                message = documentType == "地产沙龙" ? detail : string.Format("{0}更新了{1}。", updateCity, documentType);

            }
            else
            {
                message = string.Format("智地数据系统更新。新版本为{0}", detail);
            }
            PushMessageNew.PushMessage(message, notificationType, 0, tag, updateCity, documentType, updateCityId);
        }


        public ActionResult GongYePianQuImageUploadClick(string areaPath, string gongYePianQuId)
        {
            var result = new DirectResult();

            var fileUploadField = this.GetCmp<FileUploadField>("fileUploadField");
            if (fileUploadField.HasFile)
            {
                try
                {
                    string shi = GetCity(areaPath).Description;
                    string postedFileName = GetPostedFileName(fileUploadField.PostedFile.FileName);
                    postedFileName = ChinesePinYin.GetPinYin(postedFileName);
                    const string type = "gongye";
                    var imageAttachment =
                        new ImageAttachment(GetFilePath(ChinesePinYin.GetPinYin(shi), type, GetZhaoPianBaseFolder()));
                    string imageFileFullPath = imageAttachment.GetFileName(postedFileName);
                    var img = new WebImage(fileUploadField.PostedFile.InputStream);
                    SetImageDirection(img);
                    img.Save(imageFileFullPath);
                    string url = GetUrl(ChinesePinYin.GetPinYin(shi), type, GetZhaoPianMuLu(), postedFileName);
                    SaveGongYePianQuImageFielPathToDb(Convert.ToInt32(gongYePianQuId), imageFileFullPath,
                        url, postedFileName, areaPath);

                    X.Msg.Show(MessageBoxConfig(Success, TiShi, MessageBox.Icon.INFO));
                }
                catch (Exception ex)
                {
                    X.Msg.Show(MessageBoxConfig(ex.ToString(), Fail, MessageBox.Icon.INFO));
                }

            }
            else
            {
                result.IsUpload = true;
                X.Msg.Show(MessageBoxConfig(NoFile, TiShi, MessageBox.Icon.ERROR));
            }
            return result;
        }

        private void SetImageDirection(WebImage img)
        {
            if (img.Width > img.Height)
            {
                img.Resize(ImageResizeToWidth(), ImageResizeToHeight());
            }
            else
            {
                img.Resize(ImageResizeToHeight(), ImageResizeToWidth());
            }
        }

        public ActionResult XiTongImageUploadClick(string areaPath, string zhaoPianLeiXin, string importDataType)
        {
            var result = new DirectResult();

            var fileUploadField = this.GetCmp<FileUploadField>("fileUploadField");
            if (fileUploadField.HasFile)
            {
                try
                {
                    var shi = GetCity(areaPath).Description;
                    string postedFileName = GetPostedFileName(fileUploadField.PostedFile.FileName);
                    postedFileName = ChinesePinYin.GetPinYin(postedFileName);
                    const string type = "system";
                    var imageAttachment =
                        new ImageAttachment(GetFilePath(ChinesePinYin.GetPinYin(shi), type, GetZhaoPianBaseFolder()));
                    string imageFileFullPath = imageAttachment.GetFileName(postedFileName);
                    var img = new WebImage(fileUploadField.PostedFile.InputStream);
                    SetImageDirection(img);
                    img.Save(imageFileFullPath);
                    string url = GetUrl(ChinesePinYin.GetPinYin(shi), type, GetZhaoPianMuLu(), postedFileName);
                    SaveXiTongImageFilePathToDb(shi, zhaoPianLeiXin, imageFileFullPath,
                        url, postedFileName, areaPath);

                    X.Msg.Show(MessageBoxConfig(Success, TiShi, MessageBox.Icon.INFO));
                }
                catch (Exception ex)
                {
                    X.Msg.Show(MessageBoxConfig(ex.ToString(), Fail, MessageBox.Icon.INFO));
                }

            }
            else
            {
                result.IsUpload = true;
                X.Msg.Show(MessageBoxConfig(NoFile, TiShi, MessageBox.Icon.ERROR));
            }
            return result;
        }

        private static XinZhengQuYu GetCity(string areaPath)
        {
            var quYuService = new XinZhengQuYuService();
            var entity = quYuService.GetShiEntity(areaPath);
            return entity;
            //return quYuService.GetShi(areaPath);
        }

        public ActionResult ZhuanYeRenShiUploadClick(string areaPath)
        {
            var result = new DirectResult();

            var fileUploadField = this.GetCmp<FileUploadField>("fileUploadField");
            if (fileUploadField.HasFile)
            {
                try
                {
                    var shi = GetCity(areaPath).Description;
                    string postedFileName = GetPostedFileName(fileUploadField.PostedFile.FileName);
                    postedFileName = ChinesePinYin.GetPinYin(postedFileName);
                    const string type = "zhuanyerenshi";
                    var folder = Path.Combine(GetZhaoPianMuLu(), ChinesePinYin.GetPinYin(shi), type);
                    var fileName = Path.GetFileNameWithoutExtension(postedFileName) + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(postedFileName);

                    var imageAttachment = new ImageAttachment(Server.MapPath("~/" + folder));
                    string imageFileFullPath = imageAttachment.GetFileName(fileName);
                    var img = new WebImage(fileUploadField.PostedFile.InputStream);
                    img.Save(imageFileFullPath);
                    X.Msg.Show(MessageBoxConfig(Success, TiShi, MessageBox.Icon.INFO));
                    X.GetCmp<TextField>("ZhaoPian").Value = imageFileFullPath;

                    var url = WebPathUtility.Path.ToURI(imageFileFullPath);// "/" + Path.Combine(folder, fileName).Replace("\\", "/");
                    X.GetCmp<Image>("ZhaoPianImage").ImageUrl = url;
                    X.GetCmp<Hidden>("ZhaoPianUrl").Value = url;
                }
                catch (Exception ex)
                {
                    X.Msg.Show(MessageBoxConfig(ex.ToString(), Fail, MessageBox.Icon.INFO));
                }

            }
            else
            {
                result.IsUpload = true;
                X.Msg.Show(MessageBoxConfig(NoFile, TiShi, MessageBox.Icon.ERROR));
            }
            return result;
        }

        #endregion

        #region SaveFilePathToDb

        private void SaveSoftwareFilePathToDbstring(string version, string type, string url, string plistUrl)
        {
            var systemUpdateService = new SystemUpdateService();
            var systemUpdate = new SystemUpdate
            {
                Version = version,
                Type = type,
                Url = url,
                UpdateTime = DateTime.Now,
                PlistUrl = plistUrl
            };
            systemUpdateService.AddSystemUpdate(systemUpdate);
            systemUpdateService.Save();
        }

        private void SaveDocumentFilePathToDbstring(string title, string shi, string uploadFileName,
            string contentType, string documentType, string releaseDate, string physicalPath, string url, string areaPath)
        {
            string keyVal = string.Format("/{0}/{1}/{2}", shi, documentType, uploadFileName);

            var documentService = new DocumentService();
            var doc = new DocumentManagement
            {
                Title = title,
                FileName = uploadFileName,
                ContentType = contentType,
                DocumentType = documentType,
                ReleaseDate = Convert.ToDateTime(releaseDate),
                CreatedBy = GetCurrentUserName(),
                CreatedDate = DateTime.Now,
                OrgNamePath = GetOrganizationNamePath(),
                AreaPath = areaPath,
                KeyValue = keyVal,
                FilePath = physicalPath,
                Url = url,

            };
            documentService.SaveToDb(doc);
        }


        private void SaveImageFilePathToDb(string wuYeYongTu, string shi, string wuYeBianHao, string wuYeMingCheng,
            string zhaoPianLeiXin, string physicalPath, string url, string uploadFileName, string areaPath, byte[] img)
        {
            string keyVal = string.Format("/{0}/{1}/{2}/{3}/{4}", shi, wuYeYongTu, wuYeBianHao, wuYeMingCheng,
                zhaoPianLeiXin);

            var wuYeService = new WuYeService();
            var zhaoPian = new WuYeZhaoPian
            {
                WuYeYongTu = wuYeYongTu,
                WuYeBianHao = wuYeBianHao,
                WuYeMingCheng = wuYeMingCheng,
                ZhaoPianLeiXin = zhaoPianLeiXin,
                KeyValue = keyVal,
                FilePath = physicalPath,
                AreaPath = areaPath,
                OrgNamePath = GetOrganizationNamePath(),
                CreatedBy = GetCurrentUserName(),
                CreatedDate = DateTime.Now,
                FileName = uploadFileName,
                Url = url,
                Base64Img = Convert.ToBase64String(img),
                Uploaded = StatusHelper.IsLocalModel() ? "N" : "Y"
            };
            wuYeService.AddWuYeZhaoPian(zhaoPian);
        }

        private void SaveXiTongImageFilePathToDb(string shi, string zhaoPianLeiXin, string physicalPath,
            string url, string uploadFileName, string areaPath)
        {
            string keyVal = string.Format("/{0}/{1}", shi, zhaoPianLeiXin);

            var xiTongZhaoPianService = new XiTongZhaoPianService();
            var zhaoPian = new XiTongZhaoPian
            {
                ZhaoPianLeiXing = zhaoPianLeiXin,
                KeyValue = keyVal,
                FilePath = physicalPath,
                AreaPath = areaPath,
                OrgNamePath = GetOrganizationNamePath(),
                CreatedBy = GetCurrentUserName(),
                CreatedDate = DateTime.Now,
                FileName = uploadFileName,
                Url = url
            };
            xiTongZhaoPianService.AddXiTongZhaoPian(zhaoPian);
        }

        private void SaveGongYePianQuImageFielPathToDb(int gongYePianQuId, string physicalPath,
            string url, string uploadFileName, string areaPath)
        {
            var gongYePianQuService = new GongYePianQuService();
            var zhaoPian = new GongYePianQuZhaoPian
            {
                GongYePianQuId = gongYePianQuId,
                FilePath = physicalPath,
                AreaPath = areaPath,
                OrgNamePath = GetOrganizationNamePath(),
                CreatedBy = GetCurrentUserName(),
                CreatedDate = DateTime.Now,
                FileName = uploadFileName,
                Url = url,
                Uploaded = StatusHelper.IsLocalModel() ? "N" : "Y"
            };
            gongYePianQuService.AddGongYePianQuZhaoPian(zhaoPian);
        }


        #endregion


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