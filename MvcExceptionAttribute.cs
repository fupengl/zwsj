using System.Web.Mvc;

namespace ZhiDiExt
{
    public class MvcExceptionAttribute : HandleErrorAttribute
    {
        #region Overrides of HandleErrorAttribute

        /// <summary>
        /// �ڷ����쳣ʱ���á�
        /// </summary>
        /// <param name="filterContext">����-ɸѡ�������ġ�</param><exception cref="T:System.ArgumentNullException"><paramref name="filterContext"/> ����Ϊ null��</exception>
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