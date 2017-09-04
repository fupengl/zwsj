using System;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ZhiDiExt.BaseController
{
    public class JsonNetResult : JsonResult
    {
        private const string _dateFormat = "yyyy/MM/dd HH:mm:ss";

        public JsonNetResult()
        {
            Settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Error
            };
        }

        public JsonSerializerSettings Settings { get; private set; }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (this.JsonRequestBehavior == JsonRequestBehavior.DenyGet &&
                string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("JSON GET is not allowed");

            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = string.IsNullOrEmpty(this.ContentType) ? "application/json" : this.ContentType;

            if (this.ContentEncoding != null)
                response.ContentEncoding = this.ContentEncoding;
            if (this.Data == null)
                return;

            // Using Json.NET serializer
            var isoConvert = new IsoDateTimeConverter();
            isoConvert.DateTimeFormat = _dateFormat;
            response.Write(JsonConvert.SerializeObject(Data, isoConvert));
            //var scriptSerializer = JsonSerializer.Create(this.Settings);
            //using (var sw = new StringWriter())
            //{
            //    scriptSerializer.Serialize(sw, this.Data);
            //    response.Write(sw.ToString());
            //}
        }
    }
}