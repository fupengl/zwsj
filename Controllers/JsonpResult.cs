using System.Web.Mvc;

namespace ZhiDiExt.Controllers
{
    public class JsonpResult<T> : ActionResult
    {
        public T Obj { get; set; }
        public string CallbackName { get; set; }

        public JsonpResult(T obj, string callback)
        {
            Obj = obj;
            CallbackName = callback;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var js = new System.Web.Script.Serialization.JavaScriptSerializer();
            var jsonp = this.CallbackName + "(" + js.Serialize(Obj) + ")";


            context.HttpContext.Response.ContentType = "application/json";
            context.HttpContext.Response.Write(jsonp);
        }
    }
}
