using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Helpers;
using Business;
using DataAccess;
using DataAccess.DocumentApprove;
using FileUploadManager;
using Newtonsoft.Json.Linq;
using Service;

namespace ZhiDiExt.Controllers
{
    /// <summary>
    /// Summary description for ImageMultiUpload
    /// </summary>
    public class ImageMultiUpload : IHttpHandler
    {

        public readonly string FilePath = ConfigurationManager.AppSettings["UploadPath"];

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }


        public void ProcessRequest(HttpContext context)
        {
            try
            {
                var areaPath = HttpContext.Current.Request.Params["areaPath"].ToString(CultureInfo.InvariantCulture);
                var wuYeYongTu = HttpContext.Current.Request.Params["wuYeYongTu"].ToString(CultureInfo.InvariantCulture);
                var wuYeBianHao = HttpContext.Current.Request.Params["wuYeBianHao"].ToString(CultureInfo.InvariantCulture);
                var wuYeMingCheng = HttpContext.Current.Request.Params["wuYeMingCheng"].ToString(CultureInfo.InvariantCulture);
                var zhaoPianLeiXin = HttpContext.Current.Request.Params["zhaoPianLeiXin"].ToString(CultureInfo.InvariantCulture);
                //var importDataType = HttpContext.Current.Request.Params["importDataType"].ToString(CultureInfo.InvariantCulture);

                string shi = GetCity(areaPath);
                for (int i = 0; i < context.Request.Files.Count; ++i)
                {
                    HttpPostedFile file = context.Request.Files[i];
                    if (file.ContentLength == 0 || string.IsNullOrEmpty(file.FileName)) continue;
                    string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    //string fileFullName = HttpContext.Current.Server.MapPath(filePath + "/" + fileName);
                    //file.SaveAs(fileFullName);
                    var imageAttachment =
                       new ImageAttachment(GetFilePath(ChinesePinYin.GetPinYin(shi),
                           ChinesePinYin.GetPinYin(wuYeYongTu), GetZhaoPianBaseFolder(), wuYeBianHao));


                    string url = GetUrl(ChinesePinYin.GetPinYin(shi), ChinesePinYin.GetPinYin(wuYeYongTu), wuYeBianHao,
                        GetZhaoPianMuLu(), fileName);

                    string imageFileFullPath = imageAttachment.GetFileName(fileName);

                    var img = new WebImage(file.InputStream);
                    SetImageDirection(img);
                    img.Save(imageFileFullPath);
                    SaveImageFilePathToDb(wuYeYongTu, shi, wuYeBianHao, wuYeMingCheng, zhaoPianLeiXin, imageFileFullPath,
                                           url, fileName, areaPath, img.GetBytes());

                    HttpContext.Current.Response.ContentType = "application/json";

                    var json = new JObject { { "fileName", fileName } };
                    HttpContext.Current.Response.Write(json.ToString());

                    context.Response.Flush();
                }
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                context.Response.Write(ex.Message);
                context.Response.End();
            }
            finally
            {

                context.Response.End();
            }
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

        protected string GetCurrentUserName()
        {
            return GetPrincipal().Identity.Name;
        }

        private string GetOrganizationNamePath()
        {
            var myIdentity = GetPrincipal().Identity as MyIdentity;
            if (myIdentity != null) return myIdentity.NamePath;
            return "";
        }

        private static MyPrincipal GetPrincipal()
        {
            return HttpContext.Current.User as MyPrincipal;
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

        private int ImageResizeToWidth()
        {
            return ConvertToInt(ConfigurationManager.AppSettings["ResizeToWidth"]);
        }

        private int ImageResizeToHeight()
        {
            return ConvertToInt(ConfigurationManager.AppSettings["ResizeToHeight"]);
        }

        private int ConvertToInt(string id)
        {
            int result;
            if (int.TryParse(id, out result))
            {
                return result;
            }
            return 0;
        }

        private static string GetCity(string areaPath)
        {
            var quYuService = new XinZhengQuYuService();
            return quYuService.GetShi(areaPath);
        }

        private string GetUrl(string city, string type, string wuYeBianHao, string muLu, string fileName)
        {
            return string.Format(@"~/{0}/{1}/{2}/{3}/{4}", muLu, city, type, wuYeBianHao, fileName);
        }

        private string GetFilePath(string city, string type, string baseFolder, string wuYeBianHao)
        {
            return string.Format(@"{0}\{1}\{2}\{3}", baseFolder, city, type, wuYeBianHao);
        }

        private string GetZhaoPianBaseFolder()
        {
            return HttpContext.Current.Server.MapPath("~/" + GetZhaoPianMuLu());
        }

        private string GetZhaoPianMuLu()
        {
            return ConfigurationManager.AppSettings["ZhaopianMuLu"];
        }
    }
}