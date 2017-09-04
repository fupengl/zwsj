using System.Web.Mvc;

namespace ZhiDiExt
{
    public class MvcExceptionAttribute : HandleErrorAttribute
    {
        #region Overrides of HandleErrorAttribute

        /// <summary>
        /// 在发生异常时调用。
        /// </summary>
        /// <param name="filterContext">操作-筛选器上下文。</param><exception cref="T:System.ArgumentNullException"><paramref name="filterContext"/> 参数为 null。</exception>
        public override void OnException(ExceptionContext filterContext)
        {
            var controllerName = filterContext.RouteData.Values["controller"];//.ActionContext.ControllerContext.ControllerDescriptor.ControllerName;
            var actionName = filterContext.RouteData.Values["action"];//actionExecutedContext.ActionContext.ActionDescriptor.ActionName;
            var apiName = string.Format("api/mobile/{0}/{1}", controllerName, actionName);
            var log = log4net.LogManager.GetLogger("api error");
            log.Error(apiName, filterContext.Exception);

            base.OnException(filterContext);
        }

        #endregion
    }
}